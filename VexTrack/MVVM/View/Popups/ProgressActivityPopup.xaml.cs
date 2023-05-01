using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups
{
	/// <summary>
	/// Interaction logic for ProgressActivityPopup.xaml
	/// </summary>
	public partial class ProgressActivityPopup
	{
		public ProgressActivityPopup()
		{
			InitializeComponent();
			DataContext = ViewModelManager.ViewModels[nameof(ProgressActivityPopupViewModel)];
		}
	}
}
