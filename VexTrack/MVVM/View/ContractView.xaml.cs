using System.Windows.Controls;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.MVVM.View
{
	/// <summary>
	/// Interaction logic for GoalView.xaml
	/// </summary>
	public partial class GoalView : UserControl
	{
		public GoalView()
		{
			InitializeComponent();
			this.DataContext = ViewModelManager.ViewModels["Goal"];
		}
	}
}
