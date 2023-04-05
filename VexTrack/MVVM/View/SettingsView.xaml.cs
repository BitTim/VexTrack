using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View
{
	/// <summary>
	/// Interaction logic for SettingsView.xaml
	/// </summary>
	public partial class SettingsView
	{
		public SettingsView()
		{
			InitializeComponent();
			DataContext = ViewModelManager.ViewModels[nameof(SettingsViewModel)];
		}
	}
}
