using System;

namespace Cesto
{
	/// <summary>
	///     Class that performs a specified action when it is disposed.
	/// </summary>
	/// <remarks>
	///     Useful for implementing 'using' patterns: where a cleanup action is performed
	///     at the end of a using statement.
	/// </remarks>
	public class DisposableAction : IDisposable
	{
		private readonly Action _action;

		/// <summary>
		///     Create an object that implements <see cref="IDisposable" />, and
		///     will perform a specific action when disposed.
		/// </summary>
		/// <param name="action">
		///     The action that will be performed when <see cref="Dispose" /> is called.
		/// </param>
		public DisposableAction(Action action)
		{
			_action = action;
		}

		/// <summary>
		///     <see cref="IDisposable.Dispose" />
		/// </summary>
		public void Dispose()
		{
			if (_action != null)
			{
				_action();
			}
		}
	}
}