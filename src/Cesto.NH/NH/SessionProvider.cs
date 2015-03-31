using System;
using System.Data;
using System.Data.SqlClient;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHEnvironment = NHibernate.Cfg.Environment;

namespace Cesto.NH
{
    /// <summary>
    /// A session provider provides a thin layer of additional, convenient API calls that
    /// involve the NHibernate Session, Session factory and Current session context. In particular,
    /// the <see cref="SessionProvider.Transact(Action)"/> group of methods provide a convenient
    /// mechanism for invoking database transactions in a consistent way.
    /// </summary>
    public class SessionProvider
    {
        private readonly Lazy<ISessionFactory> _lazySessionFactory;

        /// <summary>
        /// Creates a <see cref="SessionProvider"/> based on an NHibernate <see cref="Configuration"/>.
        /// The associated <see cref="ISessionFactory"/> will be created when required.
        /// </summary>
        /// <param name="configuration">
        /// NHibernate <see cref="Configuration"/> object.
        /// </param>
        /// <remarks>
        /// If the current session context class has not been specified for the configuration, the
        /// configuration will be updated with a current session context class of "thread_static".
        /// </remarks>
        public SessionProvider(Configuration configuration)
        {
            Configuration = Verify.ArgumentNotNull(configuration, "configuration");
            _lazySessionFactory = new Lazy<ISessionFactory>(BuildSessionFactory);
        }

        /// <summary>
        /// The NHibernate <see cref="Configuration"/> object.
        /// </summary>
        public Configuration Configuration { get; private set; }

        /// <summary>
        /// The NHibernate <see cref="ISessionFactory"/> session factory object. This object
        /// is created when this property is first accessed by the calling application.
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get { return _lazySessionFactory.Value; }
        }

        /// <summary>
        /// Does the current session context have a session.
        /// </summary>
        public bool HasSession
        {
            get { return CurrentSessionContext.HasBind(SessionFactory); }
        }

        /// <summary>
        /// Returns the current session from the current session context.
        /// </summary>
        public ISession Session
        {
            get { return SessionFactory.GetCurrentSession(); }
        }

        /// <summary>
        /// Open a session. This method is overridable so that derived
        /// classes can set interceptors if desired.
        /// </summary>
        /// <returns>
        /// A new NHibernate session. Note that calling this method does not affect the
        /// current session context.
        /// </returns>
        public virtual ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        /// <summary>
        /// Open a stateless session.
        /// </summary>
        /// <returns></returns>
        public virtual IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        /// <summary>
        /// Perform the action inside a DB transaction.
        /// The session created for this transaction is stored in the current session context.
        /// </summary>
        /// <param name="action">Action to be performed</param>
        public void Transact(Action action)
        {
            Verify.ArgumentNotNull(action, "action");
            TransactHelper(action, session => session.BeginTransaction());
        }

        /// <summary>
        /// Perform the action inside a DB transaction, and return a result.
        /// The session created for this transaction is stored in the current session context.
        /// </summary>
        /// <param name="func">
        /// Action to be performed, which returns a result of type <typeparamref name="T"/>
        /// </param>
        public T Transact<T>(Func<T> func)
        {
            Verify.ArgumentNotNull(func, "func");
            T result = default(T);
            var action = new Action(() => result = func());
            Transact(action);
            return result;
        }

        /// <summary>
        /// Perform the action inside a DB transaction.
        /// The session created for this transaction is stored in the current session context.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level to use.</param>
        /// <param name="action">Action to be performed.</param>
        public void Transact(IsolationLevel isolationLevel, Action action)
        {
            Verify.ArgumentNotNull(action, "action");
            TransactHelper(action, session => session.BeginTransaction(isolationLevel));
        }

        /// <summary>
        /// Perform the action inside a DB transaction, and return a result.
        /// The session created for this transaction is stored in the current session context.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level to use.</param>
        /// <param name="func">
        /// Action to be performed, which returns a result of type <typeparamref name="T"/>
        /// </param>
        public T Transact<T>(IsolationLevel isolationLevel, Func<T> func)
        {
            Verify.ArgumentNotNull(func, "func");
            T result = default(T);
            var action = new Action(() => result = func());
            Transact(isolationLevel, action);
            return result;
        }

        private void TransactHelper(Action action, Func<ISession, ITransaction> beginTransaction)
        {
            for (int previousAttemptCount = 0;;++previousAttemptCount)
            {
                try
                {
                    var session = OpenSession();
                    CurrentSessionContext.Bind(session);
                    var tx = beginTransaction(session);
                    action();
                    if (tx.IsActive)
                    {
                        tx.Commit();
                    }
                    return;
                }
                catch (Exception ex)
                {
                    if (!ShouldRetry(ex, previousAttemptCount))
                    {
                        throw;
                    }
                }
                finally
                {
                    CurrentSessionContext.Unbind(SessionFactory);
                }
            }
        }

        /// <summary>
        /// Can be overridden by a derived class. Determines whether an exception thrown
        /// during a database transaction should be retried.
        /// </summary>
        /// <param name="ex">Exception thrown during a database transaction</param>
        /// <param name="previousAttemptCount">
        /// The number of times this operation has been attempted before. If this is
        /// the first attempt then the value is zero.
        /// </param>
        /// <returns>Returns <c>true</c> if the exception should be retried.</returns>
        protected virtual bool ShouldRetry(Exception ex, int previousAttemptCount)
        {
            var soEx = ex as StaleObjectStateException;
            if (soEx != null)
            {
                var logger = LoggerProvider.LoggerFor(typeof (SessionProvider));
                logger.WarnFormat("Stale object exception: Entity={0} Id={1}", soEx.EntityName, soEx.Identifier);

                // NHibernate optimistic locking exception
                return previousAttemptCount < 5;
            }

            var sqlEx = ex as SqlException;
            if (sqlEx != null && sqlEx.Number == 1205)
            {
                var logger = LoggerProvider.LoggerFor(typeof(SessionProvider));
                logger.Warn("SQL Server deadlock detected");

                // SQL Server deadlock exception
                return previousAttemptCount < 5;
            }

            return false;
        }

        private ISessionFactory BuildSessionFactory()
        {
            // Ensure that there is a value for current session context class.
            // By default it is thread static.
            // TODO: create a new CurrentSessionContextClass that uses context instead of current thread
            var cscc = Configuration.GetProperty(NHEnvironment.CurrentSessionContextClass);
            if (cscc == null)
            {
                Configuration.SetProperty(NHEnvironment.CurrentSessionContextClass, "thread_static");
            }

            return Configuration.BuildSessionFactory();
        }
    }
}
