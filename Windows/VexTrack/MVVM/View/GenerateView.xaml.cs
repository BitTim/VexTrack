using System.Windows.Controls;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View
{
	/// <summary>
	/// Interaction logic for SeasonView.xaml
	/// </summary>
	public partial class GenerateView : UserControl
	{
		public GenerateView()
		{
			InitializeComponent();
			this.DataContext = ViewModelManager.ViewModels["Season"];
		}
	}
}
