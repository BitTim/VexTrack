using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups
{
	/// <summary>
	/// Interaction logic for EditableHistoryEntryPopup.xaml
	/// </summary>
	public partial class EditableHistoryEntryPopup
	{
		public EditableHistoryEntryPopup()
		{
			InitializeComponent();
			DataContext = ViewModelManager.ViewModels[nameof(EditableHistoryEntryPopupViewModel)];
		}
	}
}
