using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for DataInitPopup.xaml
/// </summary>
public partial class DataInitPopup
{
	public DataInitPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(DataInitPopupViewModel)];
	}
}