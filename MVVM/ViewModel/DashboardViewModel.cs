using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
			Graph.PlotMargins = new OxyThickness(0, 0, 0, 16);
			Graph.PlotAreaBorderColor = OxyColors.Transparent;
			ApplyAxes();

			Update();
		}

		private void ApplyAxes()
		{
			LinearAxis xAxis = new();
			LinearAxis yAxis = new();

			SolidColorBrush Foreground = (SolidColorBrush)Application.Current.FindResource("Foreground");
			SolidColorBrush Shade = (SolidColorBrush)Application.Current.FindResource("Shade");

			xAxis.Position = AxisPosition.Bottom;
			xAxis.AbsoluteMinimum = 0;
			xAxis.TickStyle = TickStyle.None;
			xAxis.MajorStep = 5;
			xAxis.MajorGridlineStyle = LineStyle.Dot;
			xAxis.MajorGridlineColor = OxyColor.FromArgb(Shade.Color.A, Shade.Color.R, Shade.Color.G, Shade.Color.B);
			xAxis.MaximumPadding = 0;
			xAxis.MinimumPadding = 0;
			xAxis.AbsoluteMaximum = TrackingDataHelper.GetDuration(TrackingDataHelper.CurrentSeasonUUID);
			xAxis.TextColor = OxyColor.FromArgb(Foreground.Color.A, Foreground.Color.R, Foreground.Color.G, Foreground.Color.B);

			yAxis.Position = AxisPosition.Left;
			yAxis.AbsoluteMinimum = 0;
			yAxis.MinimumPadding = 0;
			yAxis.TickStyle = TickStyle.None;
			yAxis.TextColor = OxyColors.Transparent;

			Graph.Axes.Add(xAxis);
			Graph.Axes.Add(yAxis);
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

			LineSeries ideal = GraphCalc.CalcIdealGraph(TrackingDataHelper.CurrentSeasonUUID);
			LineSeries performance = GraphCalc.CalcPerformanceGraph(TrackingDataHelper.CurrentSeasonUUID);
			LineSeries dailyIdeal = DashboardDataCalc.CalcDailyIdeal(performance);

			LineSeries idealPoint = DashboardDataCalc.CalcGraphPoint(ideal, OxyColors.Silver);
			LineSeries performancePoint = DashboardDataCalc.CalcGraphPoint(performance, OxyColors.Maroon);
			LineSeries dailyIdealPoint = DashboardDataCalc.CalcGraphPoint(dailyIdeal, OxyColors.Navy);

			Graph.Series.Clear();

			AddGraphLevels();
			AddGraphGoals();

			Graph.Series.Add(ideal);
			Graph.Series.Add(dailyIdeal);
			Graph.Series.Add(performance);

			Graph.Series.Add(idealPoint);
			Graph.Series.Add(dailyIdealPoint);
			Graph.Series.Add(performancePoint);

			Graph.InvalidatePlot(true);

			OnAddClicked = new RelayCommand(o => {
				EditableHEPopup.SetParameters("Create History Entry", false);
				MainVM.QueuePopup(EditableHEPopup);
			});
		}

		private void AddGraphLevels()
		{
			List<LineSeries> levels = GraphCalc.CalcBattlepassLevels(TrackingDataHelper.CurrentSeasonUUID);
			foreach (LineSeries ls in levels) Graph.Series.Add(ls);
		}

		private void AddGraphGoals()
		{
			List<LineSeries> levels;
			List<PointAnnotation> annotations; // Fix Annotations
			(levels, annotations) = GoalDataCalc.CalcGraphGoals(TrackingDataHelper.CurrentSeasonUUID);

			foreach (LineSeries ls in levels) Graph.Series.Add(ls);
			foreach (PointAnnotation pa in annotations) Graph.Annotations.Add(pa);
		}
	}
}
