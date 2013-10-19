using System.Windows.Forms;

namespace Cesto.WinForms
{
	/// <summary>
	///     A callback for setting the checked status of a <see cref="DataGridViewCell" />.
	/// </summary>
	/// <typeparam name="T">
	///     The type of object in the associated <see cref="IVirtualDataSource{T}" />
	/// </typeparam>
	/// <param name="obj">
	///     The object in the virtual data source associated with the <see cref="DataGridViewRow" />
	///     that the <see cref="DataGridViewCell" /> belongs to.
	/// </param>
	/// <param name="isChecked">
	///     The check status. Either checked or unchecked.
	/// </param>
	public delegate void DataGridViewCellCheckCallback<in T>(T obj, bool isChecked);
}