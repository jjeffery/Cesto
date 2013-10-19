using System.Windows.Forms;

namespace Cesto.WinForms
{
	/// <summary>
	///     A callback for obtaining the value to display in a single <see cref="DataGridViewCell" />.
	/// </summary>
	/// <typeparam name="T">
	///     The type of object in the associated <see cref="IVirtualDataSource{T}" />
	/// </typeparam>
	/// <param name="obj">
	///     The object in the virtual data source associated with the <see cref="DataGridViewRow" />
	///     that the <see cref="DataGridViewCell" /> belongs to.
	/// </param>
	/// <returns>
	///     An object whose string value will be displayed in the <see cref="DataGridViewCell" />.
	/// </returns>
	public delegate object DataGridViewCellValueCallback<in T>(T obj);
}