using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for ResetDataConfirmationPopup.xaml
/// </summary>
public partial class ResetDataConfirmationPopup
{
	public ResetDataConfirmationPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(ResetDataConfirmationPopupViewModel)];
	}
}