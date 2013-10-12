using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Cesto
{
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