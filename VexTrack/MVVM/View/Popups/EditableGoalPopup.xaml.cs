using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for EditableGoalPopup.xaml
/// </summary>
public partial class EditableGoalPopup
{
	public EditableGoalPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(EditableGoalPopupViewModel)];
	}
}