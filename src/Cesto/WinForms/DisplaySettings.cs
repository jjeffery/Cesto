﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Cesto.WinForms
{
	/// <summary>
	///     Allows convenient persistant storage of display settings, such as
	///     form sizes and positions.
	/// </summary>
	/// <remarks>
	///     <para>
	///         There are many published solutions to this problem, and many of them
	///         are significantly more sophisticated than this one. In particular, this
	///         implementation makes use of the Windows Registry
	///     </para>
	///     <para>
	///         Each <c>DisplaySetting</c> object reads and writes to a registry key
	///         under HKEY_CURRENT_USER. The path of the key is determined by:
	///         <list type="number">
	///             <item>
	///                 <description>
	///                     Obtain the base path from <see cref="RegistryUtils.BasePath" />
	///                 </description>
	///             </item>
	///             <item>
	///                 <description>
	///                     Append the key name in <see cref="BaseName" />
	///                 </description>
	///             </item>
	///             <item>
	///                 <description>
	///                     Append the <see cref="DisplaySettings.Name" />
	///                 </description>
	///             </item>
	///         </list>
	///     </para>
	/// </remarks>
	public class DisplaySettings : IDisposable
	{
		/// <summary>
		///     Name of the registry Key under which all other display setting keys are stored.
		///     Normally there is not need to change this field.
		/// </summary>
		public static string BaseName = "DisplaySettings";

		private readonly string _name;
		private RegistryKey _key;
		private readonly Form _form;

		/// <summary>
		///     This field only makes sense if the <c>DisplaySettings</c> object was constructed
		///     with a <see cref="Form" />. Specifies whether the form's size and position should
		///     be auto-loaded and auto-saved. Defaults to <c>true</c>. Only change this if for
		///     some reason you do not want to save and record the size and position of your form.
		/// </summary>
		public bool AutoSaveFormPosition = true;

		/// <summary>
		///     Deletes the entire display settings tree for all forms. Useful for testing.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void DeleteAll()
		{
			string keyPath = BuildRegistryKeyPath(null);
			Registry.CurrentUser.DeleteSubKeyTree(keyPath);
		}

		#region Construction, disposal

		/// <summary>
		///     Create a <see cref="DisplaySettings" /> object with the specified name.
		/// </summary>
		/// <param name="name">Name of the Registry key where settings will be stored.</param>
		public DisplaySettings(string name)
		{
			_name = Verify.ArgumentNotNull(name, "name").Trim();
			if (_name.Length == 0)
			{
				throw new ArgumentException("name cannot be blank", "name");
			}

			string keyPath = BuildRegistryKeyPath(_name);
			_key = Registry.CurrentUser.CreateSubKey(keyPath);
		}

		/// <summary>
		///     Create a <see cref="DisplaySettings" /> object named after a <see cref="Type" />.
		/// </summary>
		/// <param name="type">
		///     The name of the <see cref="DisplaySettings" /> object will be obtained from <see cref="Type.FullName" />.
		/// </param>
		public DisplaySettings(Type type)
			: this(type.FullName)
		{}

		/// <summary>
		///     Create a <see cref="DisplaySettings" /> object for a <see cref="Component" />. This object will be disposed
		///     when the <c>Component</c> object is disposed. Note that a <c>Component</c> can be a Windows Form
		///     <see cref="Control" />.
		/// </summary>
		/// <param name="component">
		///     The <see cref="Component" />, which is typically a Windows Form <see cref="Control" />.
		/// </param>
		/// <param name="name">
		///     Optional name. If not specified, is determined from the full type name of the <paramref name="component" />.
		/// </param>
		public DisplaySettings(Component component, string name = null) : this(name ?? GetNameFromComponent(component))
		{
			Verify.ArgumentNotNull(component, "component");
			this.DisposeWith(component);
		}

		private static string GetNameFromComponent(Component component)
		{
			Verify.ArgumentNotNull(component, "component");
			return component.GetType().FullName;
		}

		/// <summary>
		///     Create a <see cref="DisplaySettings" /> object for a Windows Forms <see cref="Form" />.
		///     Will remember the size and position of the form automatically.
		/// </summary>
		/// <param name="form">
		///     The Windows Forms <see cref="Form" />
		/// </param>
		/// <param name="name">
		///     Optional name for the <see cref="DisplaySettings" /> object.
		///     If not specified, the name will be based on the full type name of <paramref name="form" />.
		/// </param>
		public DisplaySettings(Form form, string name = null) : this((Component) form, name)
		{
			_form = Verify.ArgumentNotNull(form, "form");
			_form.Load += HandleFormLoad;
			_form.FormClosed += HandleFormClosed;
		}

		public void Dispose()
		{
			if (_key != null)
			{
				_key.Close();
				_key = null;
			}
		}

		private void CheckDisposed()
		{
			if (_key == null)
			{
				throw new ObjectDisposedException(_name);
			}
		}

		#endregion

		/// <summary>
		///     Name of the registry key where this object saves its settings.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		private void HandleFormClosed(object sender, FormClosedEventArgs e)
		{
			if (AutoSaveFormPosition)
			{
				SaveFormPosition();
			}
		}

		private void HandleFormLoad(object sender, EventArgs e)
		{
			if (AutoSaveFormPosition)
			{
				LoadFormPosition();
			}
		}

		/// <summary>
		///     This method only does something if the <c>DisplaySettings</c> was created with a <see cref="Form" />.
		///     Loads the form position and size from the <c>DisplaySettings</c>.
		/// </summary>
		/// <remarks>
		///     There is usually no need to call this method, as it is automatically called if <see cref="AutoSaveFormPosition" /> is <c>true</c>.
		/// </remarks>
		public void LoadFormPosition()
		{
			if (_form == null)
			{
				return;
			}

			CheckDisposed();

			object windowStateObject = _key.GetValue("WindowState");
			object xObject = _key.GetValue("DesktopBounds.X");
			object yObject = _key.GetValue("DesktopBounds.Y");
			object widthObject = _key.GetValue("DesktopBounds.Width");
			object heightObject = _key.GetValue("DesktopBounds.Height");

			Point location;
			Size size;

			if (GetLocation(xObject, yObject, out location) && GetSize(widthObject, heightObject, out size))
			{
				_form.DesktopBounds = new Rectangle(location, size);
			}

			if (windowStateObject != null)
			{
				try
				{
					var intValue = Convert.ToInt32(windowStateObject);
					switch (intValue)
					{
						case (int) FormWindowState.Maximized:
						case (int) FormWindowState.Normal:
							_form.WindowState = (FormWindowState) intValue;
							break;

							// Note how minimized is not restored. If a program was closed
							// when the form was minimized, we want to see it restored.
						default:
							_form.WindowState = FormWindowState.Normal;
							break;
					}
				}
				catch (FormatException)
				{}
				catch (OverflowException)
				{}
				catch (InvalidCastException)
				{}
			}
		}

		private static bool GetLocation(object x, object y, out Point location)
		{
			try
			{
				if (x != null && y != null)
				{
					location = new Point(Convert.ToInt32(x), Convert.ToInt32(y));

					// check that the location fits on one of the available screens
					foreach (Screen screen in Screen.AllScreens)
					{
						if (screen.Bounds.Contains(location))
						{
							return true;
						}
					}
				}
			}
			catch (FormatException)
			{}
			catch (OverflowException)
			{}
			catch (InvalidCastException)
			{}

			location = Point.Empty;
			return false;
		}

		private static bool GetSize(object width, object height, out Size size)
		{
			try
			{
				if (width != null && height != null)
				{
					size = new Size(Convert.ToInt32(width), Convert.ToInt32(height));

					// Don't return a size if it is too small to see. The choice of minimum
					// size is a bit arbitrary.
					if (size.Width >= 50 && size.Height >= 50)
					{
						return true;
					}
				}
			}
			catch (FormatException)
			{}
			catch (OverflowException)
			{}
			catch (InvalidCastException)
			{}

			size = Size.Empty;
			return false;
		}

		private static string BuildRegistryKeyPath(string formName)
		{
			return RegistryUtils.SubKeyPath(BaseName, formName);
		}

		/// <summary>
		///     This method only does something if the <c>DisplaySettings</c> was created with a <see cref="Form" />.
		///     Saves the form position and size to the <c>DisplaySettings</c>.
		/// </summary>
		/// <remarks>
		///     There is usually no need to call this method, as it is automatically called if <see cref="AutoSaveFormPosition" /> is <c>true</c>.
		/// </remarks>
		public void SaveFormPosition()
		{
			if (_form == null)
			{
				return;
			}

			CheckDisposed();

			_key.SetValue("WindowState", (int) _form.WindowState);
			_key.SetValue("DesktopBounds.Y", _form.DesktopBounds.Y);
			_key.SetValue("DesktopBounds.X", _form.DesktopBounds.X);
			_key.SetValue("DesktopBounds.Width", _form.DesktopBounds.Width);
			_key.SetValue("DesktopBounds.Height", _form.DesktopBounds.Height);
		}

		public string GetString(string valueName, string defaultValue)
		{
			CheckDisposed();
			return Convert.ToString(_key.GetValue(valueName, defaultValue));
		}

		public void SetString(string valueName, string value)
		{
			CheckDisposed();
			if (value == null)
			{
				_key.DeleteValue(valueName);
			}
			else
			{
				_key.SetValue(valueName, value);
			}
		}

		public int GetInt32(string valueName, int defaultValue)
		{
			CheckDisposed();
			var s = GetString(valueName, null);
			if (s == null)
			{
				return defaultValue;
			}

			int value;
			if (!Int32.TryParse(s, out value))
			{
				value = defaultValue;
			}
			return value;
		}

		public void SetInt32(string valueName, int value)
		{
			CheckDisposed();
			SetString(valueName, value.ToString(CultureInfo.InvariantCulture));
		}

		public decimal GetDecimal(string valueName, decimal defaultValue)
		{
			CheckDisposed();
			var s = GetString(valueName, null);
			if (s == null)
			{
				return defaultValue;
			}

			decimal value;
			if (!decimal.TryParse(s, out value))
			{
				value = defaultValue;
			}
			return value;
		}

		public void SetDecimal(string valueName, decimal value)
		{
			CheckDisposed();
			SetString(valueName, value.ToString(CultureInfo.InvariantCulture));
		}

		public bool GetBoolean(string valueName, bool defaultValue)
		{
			CheckDisposed();
			var s = GetString(valueName, null);
			if (s == null)
			{
				return defaultValue;
			}

			bool value;
			if (!bool.TryParse(s, out value))
			{
				// have another go at converting to an integer
				int intValue;
				if (int.TryParse(s, out intValue))
				{
					value = intValue != 0;
				}
				else
				{
					value = defaultValue;	
				}
			}

			return value;
		}

		public void SetBoolean(string valueName, bool value)
		{
			CheckDisposed();
			SetString(valueName, value.ToString(CultureInfo.InvariantCulture));
		}

		public void Remove(string valueName)
		{
			CheckDisposed();
			_key.DeleteValue(valueName, false);
		}
	}
}