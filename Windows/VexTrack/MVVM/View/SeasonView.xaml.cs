using System.Windows.Controls;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View
{
	/// <summary>
	/// Interaction logic for SeasonView.xaml
	/// </summary>
	public partial class SeasonView : UserControl
	{
		public SeasonView()
		{
			InitializeComponent();
			this.DataContext = ViewModelManager.ViewModels["Season"];
		}
	}
}
