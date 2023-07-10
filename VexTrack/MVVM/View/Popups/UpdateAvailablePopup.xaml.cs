using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for UpdateAvailablePopup.xaml
/// </summary>
public partial class UpdateAvailablePopup
{
	public UpdateAvailablePopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(UpdateAvailablePopupViewModel)];
	}
}