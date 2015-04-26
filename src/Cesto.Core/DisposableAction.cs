#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
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
