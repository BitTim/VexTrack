using System.Windows.Controls;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View.Popups
{
	/// <summary>
	/// Interaction logic for EditableHistoryEntryPopup.xaml
	/// </summary>
	public partial class EditableHistoryEntryPopup : UserControl
	{
		public EditableHistoryEntryPopup()
		{
			InitializeComponent();
			this.DataContext = ViewModelManager.ViewModels["EditableHEPopup"];
		}
	}
}
