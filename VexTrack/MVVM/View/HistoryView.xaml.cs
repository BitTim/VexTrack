using System.Windows.Controls;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View
{
	/// <summary>
	/// Interaction logic for HistoryView.xaml
	/// </summary>
	public partial class HistoryView : UserControl
	{
		public HistoryView()
		{
			InitializeComponent();
			this.DataContext = ViewModelManager.ViewModels["History"];
		}
	}
}
