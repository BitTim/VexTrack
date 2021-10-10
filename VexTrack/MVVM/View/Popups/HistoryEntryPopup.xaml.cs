using System.Windows.Controls;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View.Popups
{
	/// <summary>
	/// Interaction logic for HistoryEntryPopup.xaml
	/// </summary>
	public partial class HistoryEntryPopup : UserControl
	{
		public HistoryEntryPopup()
		{
			InitializeComponent();
			this.DataContext = ViewModelManager.ViewModels["HEPopup"];
		}
	}
}
