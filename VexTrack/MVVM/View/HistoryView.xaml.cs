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
using VexTrack.Core;
using VexTrack.MVVM.Model;
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
