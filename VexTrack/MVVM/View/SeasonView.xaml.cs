using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
