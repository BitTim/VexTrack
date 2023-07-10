using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for DeleteGoalConfirmationPopup.xaml
/// </summary>
public partial class DeleteGoalConfirmationPopup
{
	public DeleteGoalConfirmationPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(DeleteGoalConfirmationPopupViewModel)];
	}
}