using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class DashboardViewModel : ObservableObject
	{
		public RelayCommand OnAddClicked { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHEPopup { get; set; }
		private MainViewModel MainVM { get; set; }

		private int _collected;
		private int _remaining;
		private int _total;
		private double _progress;
		private PlotModel _graph;

		public int Collected
		{
			get => _collected;
			set
			{
				_collected = value;
				OnPropertyChanged();
			}
		}
		public int Remaining
		{
			get => _remaining;
			set
			{
				_remaining = value;
				OnPropertyChanged();
			}
		}
		public int Total
		{
			get => _total;
			set
			{
				_total = value;
				OnPropertyChanged();
			}
		}
		public double Progress
		{
			get => _progress;
			set
			{
				_progress = value;
				OnPropertyChanged();
			}
		}

		public PlotModel Graph
		{
			get => _graph;
			set
			{
				_graph = value;
				OnPropertyChanged();
			}
		}

		public DashboardViewModel()
		{
			Graph = new PlotModel();
			Graph.PlotAreaBorderColor = OxyColors.Transparent;

			Update();
		}

		public void Update()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			EditableHEPopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];

			DailyData data = DashboardDataCalc.CalcDailyData();

			Collected = data.Collected;
			Remaining = data.Remaining;
			Total = data.Total;
			Progress = data.Progress;

			Graph.Series.Clear();

			Graph.Series.Add(GraphCalc.CalcIdealGraph(TrackingDataHelper.CurrentSeasonUUID));
			Graph.Series.Add(GraphCalc.CalcPerformanceGraph(TrackingDataHelper.CurrentSeasonUUID));

			Graph.InvalidatePlot(true);

			OnAddClicked = new RelayCommand(o => {
				EditableHEPopup.SetParameters("Create History Entry", false);
				MainVM.QueuePopup(EditableHEPopup);
			});
		}
	}
}
