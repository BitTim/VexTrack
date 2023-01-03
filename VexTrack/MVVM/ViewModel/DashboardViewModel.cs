using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	internal class DashboardViewModel : ObservableObject
	{
		public RelayCommand OnAddClicked { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }
		private MainViewModel MainVm { get; set; }

		private string _title;
		private string _username;
		private int _collected;
		private int _remaining;
		private int _total;
		private int _streak;
		private Brush _streakColor;
		private double _progress;
		private string _seasonName;
		private int _deviationIdeal;
		private int _deviationDaily;
		private int _daysFinished;
		private int _daysRemaining;
		private readonly PlotModel _graph;

		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		public string Username
		{
			get => _username;
			set
			{
				_username = value;
				OnPropertyChanged();
			}
		}

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

		public int Streak
		{
			get => _streak;
			set
			{
				_streak = value;
				OnPropertyChanged();
			}
		}

		public Brush StreakColor
		{
			get => _streakColor;
			set
			{
				_streakColor = value;
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

		public string SeasonName
		{
			get => _seasonName;
			set
			{
				_seasonName = value;
				OnPropertyChanged();
			}
		}

		public int DeviationIdeal
		{
			get => _deviationIdeal;
			set
			{
				_deviationIdeal = value;
				OnPropertyChanged();
			}
		}

		public int DeviationDaily
		{
			get => _deviationDaily;
			set
			{
				_deviationDaily = value;
				OnPropertyChanged();
			}
		}

		public int DaysFinished
		{
			get => _daysFinished;
			set
			{
				_daysFinished = value;
				OnPropertyChanged();
			}
		}

		public int DaysRemaining
		{
			get => _daysRemaining;
			set
			{
				_daysRemaining = value;
				OnPropertyChanged();
			}
		}

		public PlotModel Graph
		{
			get => _graph;
			init
			{
				_graph = value;
				OnPropertyChanged();
			}
		}

		public static List<double> SegmentStops => new List<double> { 0.25, 0.5, 1.0 };

		public DashboardViewModel()
		{
			Graph = new PlotModel
			{
				PlotMargins = new OxyThickness(0, 0, 0, 16),
				PlotAreaBorderColor = OxyColors.Transparent
			};

			ApplyAxes();

			Update(false);
		}

		private void ApplyAxes()
		{
			LinearAxis xAxis = new();
			LinearAxis yAxis = new();

			var foreground = (SolidColorBrush)Application.Current.FindResource("Foreground");
			var shade = (SolidColorBrush)Application.Current.FindResource("Shade");

			xAxis.Position = AxisPosition.Bottom;
			xAxis.AbsoluteMinimum = 0;
			xAxis.TickStyle = TickStyle.None;
			xAxis.MajorStep = 5;
			xAxis.MajorGridlineStyle = LineStyle.Dot;
			xAxis.MaximumPadding = 0;
			xAxis.MinimumPadding = 0;
			xAxis.AbsoluteMaximum = TrackingDataHelper.GetDuration(TrackingDataHelper.CurrentSeasonUUID);
			
			if (shade != null) xAxis.MajorGridlineColor = OxyColor.FromArgb(shade.Color.A, shade.Color.R, shade.Color.G, shade.Color.B);
			if (foreground != null) xAxis.TextColor = OxyColor.FromArgb(foreground.Color.A, foreground.Color.R, foreground.Color.G, foreground.Color.B);

			yAxis.Position = AxisPosition.Left;
			yAxis.AbsoluteMinimum = 0;
			yAxis.MinimumPadding = 0;
			yAxis.TickStyle = TickStyle.None;
			yAxis.TextColor = OxyColors.Transparent;

			Graph.Axes.Add(xAxis);
			Graph.Axes.Add(yAxis);
		}

		public void Update(bool epilogue)
		{
			MainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];

			Username = SettingsHelper.Data.Username;
			Title = Username != "" ? "Welcome back," : "Welcome back";

			var data = DashboardDataCalc.CalcDailyData(epilogue);

			Collected = data.Collected;
			Remaining = data.Remaining;
			Total = data.Total;
			Progress = data.Progress;
			Streak = data.Streak;
			StreakColor = StreakDataCalc.GetStreakColor(DateTimeOffset.Now.ToLocalTime().Date, epilogue);

			SeasonName = TrackingDataHelper.CurrentSeasonData.Name;
			(DeviationIdeal, DeviationDaily) = CalcGraph(epilogue);
			DaysRemaining = TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID);
			DaysFinished = DashboardDataCalc.CalcDaysFinished(epilogue);

			OnAddClicked = new RelayCommand(o =>
			{
				EditableHePopup.SetParameters("Create History Entry", false);
				MainVm.QueuePopup(EditableHePopup);
			});
		}

		private (int, int) CalcGraph(bool epilogue)
		{
			var graphIdealPoint = (SolidColorBrush)Application.Current.FindResource("GraphIdealPoint");

			var foreground = (SolidColorBrush)Application.Current.FindResource("Foreground");
			var background = (SolidColorBrush)Application.Current.FindResource("Background");
			var shade = (SolidColorBrush)Application.Current.FindResource("Shade");

			var ideal = GraphCalc.CalcIdealGraph(TrackingDataHelper.CurrentSeasonUUID, epilogue);
			var performance = GraphCalc.CalcPerformanceGraph(TrackingDataHelper.CurrentSeasonUUID);
			var dailyIdeal = DashboardDataCalc.CalcDailyIdeal(performance, epilogue);
			var average = DashboardDataCalc.CalcAverageGraph(performance, epilogue);

			if (graphIdealPoint != null)
			{
				var idealPoint = DashboardDataCalc.CalcGraphPoint(ideal, OxyColor.FromArgb(graphIdealPoint.Color.A, graphIdealPoint.Color.R, graphIdealPoint.Color.G, graphIdealPoint.Color.B));
				var performancePoint = DashboardDataCalc.CalcGraphPoint(performance, OxyColors.Maroon);
				var dailyIdealPoint = DashboardDataCalc.CalcGraphPoint(dailyIdeal, OxyColors.Navy);

				var bufferZone = GraphCalc.CalcBufferZone(TrackingDataHelper.CurrentSeasonUUID, epilogue);

				Graph.LegendPosition = LegendPosition.LeftTop;
				if (background != null) Graph.LegendBackground = OxyColor.FromArgb(background.Color.A, background.Color.R, background.Color.G, background.Color.B);
				if (foreground != null) Graph.LegendTextColor = OxyColor.FromArgb(foreground.Color.A, foreground.Color.R, foreground.Color.G, foreground.Color.B);
				if (shade != null) Graph.LegendBorder = OxyColor.FromArgb(shade.Color.A, shade.Color.R, shade.Color.G, shade.Color.B);

				Graph.Series.Clear();
				Graph.Axes.Clear();
				Graph.Annotations.Clear();

				AddGraphLevels(epilogue);
				AddGraphGoals();
				ApplyAxes();
				UpdateYAxis(performance, epilogue);

				Graph.Series.Add(ideal);
				Graph.Series.Add(dailyIdeal);
				Graph.Series.Add(average);
				Graph.Series.Add(performance);

				Graph.Series.Add(idealPoint);
				if (dailyIdeal.Points.Count > 0) Graph.Series.Add(dailyIdealPoint);
				Graph.Series.Add(performancePoint);

				Graph.Annotations.Add(bufferZone);
			}

			Graph.InvalidatePlot(true);

			var t = performance.Points.Count - 1;
			var deviationIdeal = (int)performance.Points[t].Y - (int)ideal.Points[t].Y;

			var deviationDaily = 0;
			if (dailyIdeal.Points.Count > 0) deviationDaily = (int)performance.Points[t].Y - (int)dailyIdeal.Points[1].Y;

			return (deviationIdeal, deviationDaily);
		}

		private void UpdateYAxis(DataPointSeries performance, bool epilogue)
		{
			var maximum = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue).Total * 2;
			if (performance.Points.Last().Y >= maximum) maximum = (int)performance.Points.Last().Y + 20000;

			Graph.Axes[1].AbsoluteMaximum = maximum;
		}

		private void AddGraphLevels(bool epilogue)
		{
			var levels = GraphCalc.CalcBattlepassLevels(TrackingDataHelper.CurrentSeasonUUID);
			foreach (var ls in levels) Graph.Series.Add(ls);

			if (!epilogue) return;

			var epilogueLevels = GraphCalc.CalcEpilogueLevels(TrackingDataHelper.CurrentSeasonUUID);
			foreach (var ls in epilogueLevels) Graph.Series.Add(ls);
		}

		private void AddGraphGoals()
		{
			var (levels, annotations) = GoalDataCalc.CalcGraphGoals(TrackingDataHelper.CurrentSeasonUUID);

			foreach (var ls in levels) Graph.Series.Add(ls);
			foreach (var ta in annotations) Graph.Annotations.Add(ta);
		}
	}
}
