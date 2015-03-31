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
using System.Threading;
using System.Windows.Forms;

namespace Cesto.WinForms
{
    /// <summary>
    ///     Simple mechanism for displaying the wait cursor
    /// </summary>
    /// <example>
    ///     <code>
    /// public void LongOperation() {
    ///     // about to start a long operation
    ///     using (WaitCursor.Show()) {
    ///         // do long operation here
    ///         .
    ///         .
    ///         .
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    ///     Multiple calls to <c>WaitCursor.Show</c> can be nested.
    /// </remarks>
    public static class WaitCursor
    {
        private static readonly IDisposable Disposable = new DisposableAction(RestoreCursor2);
        private static int _referenceCount;

        /// <summary>
        ///     Event raised with the current value of the cursor is changed.
        /// </summary>
        public static event EventHandler Changed;

        /// <summary>
        ///     This property specifies whether the wait cursor is visible.
        /// </summary>
        public static bool Visible { get; private set; }

        /// <summary>
        ///     Show the wait cursor.
        /// </summary>
        /// <returns>
        ///     Returns an <see cref="IDisposable" /> intended for use in a <c>using</c>
        ///     statement. When the <see cref="IDisposable" /> is disposed, then the
        ///     previous cursor is restored.
        /// </returns>
        public static IDisposable Show()
        {
            if (Interlocked.Increment(ref _referenceCount) == 1)
            {
                Cursor.Current = Cursors.WaitCursor;
                Visible = true;
                var changed = Changed;
                if (changed != null)
                {
                    changed(null, EventArgs.Empty);
                }
            }
            return Disposable;
        }

        private static void RestoreCursor2()
        {
            if (Interlocked.Decrement(ref _referenceCount) == 0)
            {
                Cursor.Current = Cursors.Default;
                Visible = false;
                var changed = Changed;
                if (changed != null)
                {
                    changed(null, EventArgs.Empty);
                }
            }
        }
    }
}