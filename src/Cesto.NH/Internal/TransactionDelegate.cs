#region License

//  Notice: Some of the code in this file may have been adapted from code
//  in the Castle Project.
//
// Copyright 2012 The Castle Project
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Data;
using System.Threading;
using Cesto;
using NHibernate;
using NHibernate.Transaction;

namespace Quokka.NH.Implementations
{
    /// <summary>
    /// Proxies an ITransaction so that the transaction will not be committed
    /// or rolled-back if the transaction was created by a scope higher
    /// up on the calling stack.
    /// </summary>
    [Serializable]
    public class TransactionDelegate : ITransaction
    {
        // Used to communicate that a transaction has been rolled back.
        // This allows a TransactionDelegate to know whether one of its
        // nested TransactionDelegates has rolled back, which means that
        // this TransactionDelegate should rollback only.
        private static event Action<Guid> RolledBack = delegate { };

        // The inner session, not a delegate.
        private readonly ISession _session;
        private readonly Guid _sessionId;
        private int _rollbackCount;
        private ITransaction _transaction;
        private bool _rollbackCalled;
        private bool _commitCalled;
        private bool _begun;
        private bool _completed;

        /// <summary>
        /// This event is raised after the transaction has completed.
        /// </summary>
        /// <remarks>
        /// A transaction completes the first time any of the following 
        /// methods are called: <see cref="Commit"/>, <see cref="Rollback"/> 
        /// or <see cref="Dispose"/>.
        /// </remarks>
        public event Action<TransactionDelegate> TransactionCompleted = delegate { };

        public TransactionDelegate(ISession session)
        {
            _session = Verify.ArgumentNotNull(session, "session");
            _sessionId = _session.GetSessionImplementation().SessionId;
            _transaction = _session.Transaction;
        }

        /// <summary>
        /// Can this <see cref="TransactionDelegate"/> commit the transaction.
        /// </summary>
        public bool CanCommit { get; private set; }

        /// <summary>
        /// Used for unit testing only.
        /// </summary>
        public ITransaction InnerTransaction
        {
            get { return _transaction; }
        }

        public void Dispose()
        {
            // Regardless of whether this transaction delegate can commit, if 
            // Dispose is called without calling Commit first, we need to roll back.
            if (_begun && !_commitCalled && !_rollbackCalled) {
                // Transaction is being disposed without Commit being called,
                // which means we should roll back the transaction. If this is
                // a nested transaction, it means notifying any other transaction
                // that is using the same underlying session.
                RolledBack(_sessionId);
            }

            // Don't need rolled back notifications anymore.
            UnsubscribeFromRollback();

            try {
                // Only dispose the inner transaction if we are the delegate that
                // is allowed to commit.
                if (CanCommit) {
                    _transaction.Dispose();
                }
            }
            finally {
                // Note that this will only do something if it has not already been called
                // from Commit() or Rollback()
                AfterTransactionComplete();
            }
        }

        public void Begin()
        {
            BeginHelper(() => _session.Transaction.Begin());
        }

        public void Begin(IsolationLevel isolationLevel)
        {
            BeginHelper(() => _session.Transaction.Begin(isolationLevel));
        }

        private void BeginHelper(Action beginTransactionAction)
        {
            if (!_begun) {
                var wasActive = _session.Transaction.IsActive;
                beginTransactionAction();
                _begun = true;
                _rollbackCount = 0;
                _transaction = _session.Transaction;
                CanCommit = !wasActive;
                SubscribeToRolledBack();
            }
        }

        public void Commit()
        {
            try {
                _commitCalled = true;
                if (CanCommit) {
                    var rollbackCount = Interlocked.Exchange(ref _rollbackCount, 0);
                    if (rollbackCount == 0) {
                        _transaction.Commit();
                    }
                    else {
                        if (_transaction.IsActive) {
                            _transaction.Rollback();
                        }
                        throw new TransactionException("Cannot commit transaction as it has been rolled-back in a nested transaction");
                    }
                }
            }
            finally {
                AfterTransactionComplete();
            }
        }

        public void Rollback()
        {
            try {
                _rollbackCalled = true;
                if (CanCommit) {
                    _transaction.Rollback();
                }
            }
            finally {
                RolledBack(_sessionId);
                AfterTransactionComplete();
            }
        }

        public void Enlist(IDbCommand command)
        {
            _transaction.Enlist(command);
        }

        public void RegisterSynchronization(ISynchronization synchronization)
        {
            _transaction.RegisterSynchronization(synchronization);
        }

        public bool IsActive
        {
            get { return _transaction.IsActive; }
        }

        public bool WasRolledBack
        {
            get { return _transaction.WasRolledBack; }
        }

        public bool WasCommitted
        {
            get { return _transaction.WasCommitted; }
        }

        private bool _subscribedToRolledBack;

        private void SubscribeToRolledBack()
        {
            if (CanCommit) {
                if (!_subscribedToRolledBack) {
                    RolledBack += HandleRolledBack;
                    _subscribedToRolledBack = true;
                }
            }
        }

        private void UnsubscribeFromRollback()
        {
            if (_subscribedToRolledBack) {
                RolledBack -= HandleRolledBack;
                _subscribedToRolledBack = false;
            }
        }

        private void HandleRolledBack(Guid sessionId)
        {
            // This can be called on a different thread, so we
            // only read from a readonly field, and increment
            // using a thread-safe mechanism.
            if (sessionId == _sessionId) {
                Interlocked.Increment(ref _rollbackCount);
            }
        }

        private void AfterTransactionComplete()
        {
            if (!_completed) {
                _completed = true;
                TransactionCompleted(this);
            }
        }
    }
}
