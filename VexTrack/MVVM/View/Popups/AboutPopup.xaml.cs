using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VexTrack.MVVM.View.Popups
{
	/// <summary>
	/// Interaction logic for AboutPopup.xaml
	/// </summary>
	public partial class AboutPopup : UserControl
	{
		public AboutPopup()
		{
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
			e.Handled = true;
		}
	}
}
