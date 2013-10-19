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