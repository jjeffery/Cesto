#region License

// Copyright 2004-2014 John Jeffery
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
using System.Windows.Forms;

namespace Quokka.WinForms.Config
{
	public class ListConfigGridView : DataGridView
	{
		public event EventHandler MoveUp;
		public event EventHandler EnterKeyPressed;

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (EnterKeyPressed != null)
				{
					EnterKeyPressed(this, EventArgs.Empty);
					return;
				}
			}
			if (e.KeyData == Keys.Up)
			{
				var currentRow = CurrentRow;
				if (currentRow != null && currentRow.Index == 0)
				{
					if (MoveUp != null)
					{
						MoveUp(this, EventArgs.Empty);
						ClearSelection();
						return;
					}
				}
			}

			base.OnKeyDown(e);
		}
	}
}
