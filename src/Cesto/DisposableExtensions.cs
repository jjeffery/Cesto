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
using System.ComponentModel;
using System.Windows.Forms;

namespace Cesto
{
	/// <summary>
	///     Extension methods for <see cref="IDisposable" /> objects.
	/// </summary>
	public static class DisposableExtensions
	{
		/// <summary>
		///     Specify that the <see cref="IDisposable" /> should be disposed when the <see cref="Component" />
		///     is disposed.
		/// </summary>
		/// <param name="disposable">
		///     An <see cref="IDisposable" /> that should be disposed at the same
		///     time as the <paramref name="component" />.
		/// </param>
		/// <param name="component">
		///     A <see cref="Component" />, which includes Windows Forms <see cref="Control" />  <see cref="Form" /> objects.
		/// </param>
		public static void DisposeWith(this IDisposable disposable, Component component)
		{
			if (disposable != null && component != null)
			{
				component.Disposed += (sender, args) => disposable.Dispose();
			}
		}
	}
}