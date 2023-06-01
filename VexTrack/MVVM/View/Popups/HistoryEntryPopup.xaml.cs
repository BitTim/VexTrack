using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for HistoryEntryPopup.xaml
/// </summary>
public partial class HistoryEntryPopup
{
	public HistoryEntryPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(HistoryEntryPopupViewModel)];
	}
}