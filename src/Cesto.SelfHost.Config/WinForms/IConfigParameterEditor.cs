using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cesto.Config;

namespace Cesto.WinForms
{
	public interface IConfigParameterEditor
	{
		string TextValue { get; }
		Control Control { get; }
		IConfigParameter Parameter { get; }

		void Initialize(IConfigParameter parameter);
	}
}
