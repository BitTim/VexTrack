using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View
{
	/// <summary>
	/// Interaction logic for HomeView.xaml
	/// </summary>
	public partial class HomeView
	{
		public HomeView()
		{
			InitializeComponent();
			DataContext = ViewModelManager.ViewModels[nameof(HomeViewModel)];
		}
	}
}
