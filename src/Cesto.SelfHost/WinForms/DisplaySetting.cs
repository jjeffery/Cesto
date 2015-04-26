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

namespace Cesto.WinForms
{
	/// <summary>
	///     Abstract class for a Display Setting that loads and saves a particular data type.
	/// </summary>
	/// <typeparam name="T">Type that is saved to the display setting.</typeparam>
	/// <remarks>
	///     Derive from this class to load and save arbitrary types.
	/// </remarks>
	public abstract class DisplaySetting<T>
	{
		/// <summary>
		///     The <see cref="DisplaySettings" /> that this object belongs to.
		/// </summary>
		public readonly DisplaySettings DisplaySettings;

		/// <summary>
		///     The value name.
		/// </summary>
		public readonly string Name;

		/// <summary>
		///     The default value if the value is not present in the
		///     registry.
		/// </summary>
		public readonly T DefaultValue;

		/// <summary>
		///     Creates a new <see cref="DisplaySetting{T}" /> object.
		/// </summary>
		/// <param name="displaySettings">
		///     The associated <see cref="DisplaySettings" /> object.
		/// </param>
		/// <param name="name">
		///     The name of the value that this <see cref="DisplaySetting{T}" /> will read from and write to.
		/// </param>
		/// <param name="defaultValue">
		///     The default value to return in <see cref="GetValue" /> when the value does not exist.
		/// </param>
		protected DisplaySetting(DisplaySettings displaySettings, string name, T defaultValue = default(T))
		{
			DisplaySettings = Verify.ArgumentNotNull(displaySettings, "displaySettings");
			Name = Verify.ArgumentNotNull(name, "name");
			DefaultValue = defaultValue;
		}

		/// <summary>
		///     Get the value from the Registry. If the value is not present then
		///     <see cref="DefaultValue" /> is returned.
		/// </summary>
		/// <returns></returns>
		public abstract T GetValue();

		/// <summary>
		///     Set the value in the registry. If <paramref name="value" /> is equal to
		///     <see cref="DefaultValue" />, then the value is removed from the Registry.
		/// </summary>
		/// <param name="value">
		///     The value to store in the Registry for this display setting.
		/// </param>
		public abstract void SetValue(T value);
	}
}
