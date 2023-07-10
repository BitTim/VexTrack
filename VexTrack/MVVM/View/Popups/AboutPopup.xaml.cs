using System.Diagnostics;
using System.Windows.Navigation;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.View.Popups;

/// <summary>
/// Interaction logic for AboutPopup.xaml
/// </summary>
public partial class AboutPopup
{
	public AboutPopup()
	{
		InitializeComponent();
		DataContext = ViewModelManager.ViewModels[nameof(AboutPopupViewModel)];
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
		e.Handled = true;
	}
}