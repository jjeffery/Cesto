using System;
using System.Data;
using System.Threading;
using NHibernate;

namespace Cesto.NH
{
    public static class DB
    {
        public static ISessionFactory SessionFactory;
        private static readonly ThreadLocal<SessionHolder> ThreadLocalSessionHolder = new ThreadLocal<SessionHolder>();

        /// <summary>
        /// Perform the action inside a DB transaction.
        /// </summary>
        /// <param name="action">Action to be performed</param>
        public static void Transact(Action action)
        {
            Verify.ArgumentNotNull(action, "action");
            TransactHelper(action, session => session.BeginTransaction());
        }

        /// <summary>
        /// Perform the action inside a DB transaction, and return a result.
        /// </summary>
        /// <param name="func">
        /// Action to be performed, which returns a result of type <typeparamref name="T"/>
        /// </param>
        public static T Transact<T>(Func<T> func)
        {
            Verify.ArgumentNotNull(func, "func");
            T result = default(T);
            var action = new Action(() => result = func());
            Transact(action);
            return result;
        }

        /// <summary>
        /// Perform the action inside a DB transaction.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level to use.</param>
        /// <param name="action">Action to be performed.</param>
        public static void Transact(IsolationLevel isolationLevel, Action action)
        {
            Verify.ArgumentNotNull(action, "action");
            TransactHelper(action, session => session.BeginTransaction(isolationLevel));
        }

        /// <summary>
        /// Perform the action inside a DB transaction, and return a result.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level to use.</param>
        /// <param name="func">
        /// Action to be performed, which returns a result of type <typeparamref name="T"/>
        /// </param>
        public static T Transact<T>(IsolationLevel isolationLevel, Func<T> func)
        {
            Verify.ArgumentNotNull(func, "func");
            T result = default(T);
            var action = new Action(() => result = func());
            Transact(isolationLevel, action);
            return result;
        }

        private static void TransactHelper(Action action, Func<ISession, ITransaction> beginTransaction)
        {
            CheckSessionFactory();
            if (ThreadLocalSessionHolder.Value != null) {
                throw new InvalidOperationException("Another session is already current for this thread");
            }
            try
            {
                // Don't create the session yet, just create the holder.
                // This way if the action never accesses DB.Session, there
                // is no need to go through the expense of a DB transaction
                // that does nothing.
                ThreadLocalSessionHolder.Value = new SessionHolder { BeginTransactionCallback = beginTransaction };
                action();
                var tx = ThreadLocalSessionHolder.Value.Transaction;
                if (tx != null && tx.IsActive)
                {
                    tx.Commit();
                }
            }
            finally
            {
                // Make sure that the thread local storage is set to null prior to 
                // anything else so that at the end of the finally block there is
                // definitely no remains of the session/transaction in TLS.
                var holder = ThreadLocalSessionHolder.Value;
                ThreadLocalSessionHolder.Value = null;

                if (holder != null && holder.Session != null)
                {
                    // Disposing of the session will rollback the transaction
                    // if the transaction is still active.
                    holder.Session.Dispose();
                }
            }
        }

        public static ISession Session
        {
            get {
                var holder = ThreadLocalSessionHolder.Value;
                if (holder == null)
                {
                    throw new InvalidOperationException("No current session");
                }

                if (holder.Session == null)
                {
                    // This is the first time that DB.Session has been accesssed.
                    // Now that we know that we actually have to do some work against
                    // the DB we will create the session and begin a transaction.
                    holder.Session = SessionFactory.OpenSession();
                    holder.Transaction = holder.BeginTransactionCallback(holder.Session);
                }
                return holder.Session;
            }
        }

        public static bool HasSession
        {
            get { return ThreadLocalSessionHolder.Value != null; }
        }

        public static ISession OpenSession()
        {
            CheckSessionFactory();
            return SessionFactory.OpenSession();
        }

        public static IStatelessSession OpenStatelessSession()
        {
            CheckSessionFactory();
            return SessionFactory.OpenStatelessSession();
        }

        private static void CheckSessionFactory()
        {
            if (SessionFactory == null)
            {
                throw new InvalidOperationException("DB.SessionFactory is null");
            }
        }

        // Stored in thread local storage, holds the information needed about the current session.
        private class SessionHolder
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public ISession Session;
            public ITransaction Transaction;
            public Func<ISession, ITransaction> BeginTransactionCallback;
        }
    }
}