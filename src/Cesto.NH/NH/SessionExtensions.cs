#region License

// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
using Cesto;
using NHibernate.Transaction;

// ReSharper disable once CheckNamespace

namespace NHibernate
{
	/// <summary>
	/// Useful extension methods for NHibernate <see cref="ISession"/>
	/// </summary>
	public static class SessionExtensions
	{
		/// <summary>
		/// Create an <see cref="IDbCommand"/> from the session's database
		/// connection, and enlist it in the session's transaction if it has one.
		/// </summary>
		/// <param name="session"></param>
		/// <returns>
		/// Returns an <see cref="IDbConnection"/> ready to use in the session's transaction.
		/// </returns>
		/// <remarks>
		/// Use this extension method with caution. Any updates performed directly against the
		/// database might cause the NHibernate first level cache to be inconsistent. For example,
		/// using an ADO.NET command to delete a large number of rows directly from the database
		/// while there are corresponding NHibernate objects in the session's cache will cause
		/// inconsistent results.
		/// </remarks>
		public static IDbCommand CreateCommand(this ISession session)
		{
			Verify.ArgumentNotNull(session, "session");
			var command = session.Connection.CreateCommand();
			if (session.Transaction != null)
			{
				session.Transaction.Enlist(command);
			}
			return command;
		}

		/// <summary>
		/// Schedule an action to perform immediately after the transaction has committed.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">Action to perform</param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void AfterCommit(this ISession session, System.Action action)
		{
			Verify.ArgumentNotNull(session, "session");
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization {AfterCommitAction = action});
		}

		/// <summary>
		/// Schedule an action to perform immediately after the transaction
		/// has been rolled back.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">Action to perform</param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void AfterRollback(this ISession session, System.Action action)
		{
			Verify.ArgumentNotNull(session, "session");
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization {AfterRollbackAction = action});
		}

		/// <summary>
		/// Schedule an action to perform immediately after the transaction has completed.
		/// This action will be performed regardless of whether the transaction was committed
		/// or rolled-back. The boolean parameter passed to the action indicates whether the
		/// transaction succeeded or not.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">
		/// Action to perform. This action accepts a boolean parameter, whose value is
		/// <c>true</c> if the transaction was committed, or <c>false</c> if the transaction
		/// was rolled back.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void AfterCompletion(this ISession session, Action<bool> action)
		{
			Verify.ArgumentNotNull(session, "session");
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization {AfterCompletionAction = action});
		}

		/// <summary>
		/// Schedule an action to perform immediately prior to the transaction is complete.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">Action to perform</param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void BeforeCompletion(this ISession session, System.Action action)
		{
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization {BeforeCompletionAction = action});
		}

		private class Synchronization : ISynchronization
		{
			public System.Action AfterCommitAction;
			public System.Action AfterRollbackAction;
			public System.Action BeforeCompletionAction;
			public Action<bool> AfterCompletionAction;

			public void BeforeCompletion()
			{
				if (BeforeCompletionAction != null)
				{
					BeforeCompletionAction();
				}
			}

			public void AfterCompletion(bool success)
			{
				if (AfterCompletionAction != null)
				{
					AfterCompletionAction(success);
				}

				if (success)
				{
					if (AfterCommitAction != null)
					{
						AfterCommitAction();
					}
				}
				else
				{
					if (AfterRollbackAction != null)
					{
						AfterRollbackAction();
					}
				}
			}
		}
	}
}
