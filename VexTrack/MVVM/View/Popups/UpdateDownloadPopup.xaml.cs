using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for UpdateDownloadPopup.xaml
/// </summary>
public partial class UpdateDownloadPopup
{
	public UpdateDownloadPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(UpdateDownloadPopupViewModel)];
	}
}