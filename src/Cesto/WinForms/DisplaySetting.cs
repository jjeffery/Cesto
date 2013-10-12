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
		public readonly DisplaySettings DisplaySettings;
		public readonly string Name;
		public readonly T DefaultValue;

		protected DisplaySetting(DisplaySettings displaySettings, string name, T defaultValue = default(T))
		{
			DisplaySettings = Verify.ArgumentNotNull(displaySettings, "displaySettings");
			Name = Verify.ArgumentNotNull(name, "name");
			DefaultValue = defaultValue;
		}

		public abstract T GetValue();
		public abstract void SetValue(T value);
	}
}