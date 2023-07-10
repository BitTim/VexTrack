using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for UpdateFailedPopup.xaml
/// </summary>
public partial class UpdateFailedPopup
{
	public UpdateFailedPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(UpdateFailedPopupViewModel)];
	}
}