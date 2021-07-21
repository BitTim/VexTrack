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
		public bool AreCommandsSet { get; set; }
		private RelayCommand HistoryButtonClick { get; set; }

		public HistoryView()
		{
			InitializeComponent();

			var vm = (HistoryViewModel)DataContext;
			vm.RegisterView(this);
		}

		public void SetCommands(Action<object> historyButtonClick)
		{
			HistoryButtonClick = new RelayCommand(historyButtonClick);
			AreCommandsSet = true;
		}

		public void AddHistoryEntryButton(string description, int amount, string backgroundKey = "", string foregroundKey = "")
		{
			HistoryEntryButtonModel child = new(description, amount);
			if (backgroundKey != "") child.Background = (Brush)FindResource(backgroundKey);
			if (foregroundKey != "") child.Foreground = (Brush)FindResource(foregroundKey);

			child.Command = HistoryButtonClick;
			child.CommandParameter = ContentContainer.Children.Count;
			ContentContainer.Children.Insert(0, child);
		}
	}
}
