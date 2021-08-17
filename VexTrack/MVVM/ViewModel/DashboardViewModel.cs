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
		private PlotModel _graph;

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

			Update(false);
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

		public void Update(bool epilogue)
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			EditableHEPopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];

			Username = SettingsHelper.Data.Username;
			if (Username != "") Title = "Welcome back,";
			else Title = "Welcome back";

			DailyData data = DashboardDataCalc.CalcDailyData(epilogue);

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
				EditableHEPopup.SetParameters("Create History Entry", false);
				MainVM.QueuePopup(EditableHEPopup);
			});
		}

		private (int, int) CalcGraph(bool epilogue)
		{
			SolidColorBrush GraphIdealPoint = (SolidColorBrush)Application.Current.FindResource("GraphIdealPoint");

			SolidColorBrush Foreground = (SolidColorBrush)Application.Current.FindResource("Foreground");
			SolidColorBrush Background = (SolidColorBrush)Application.Current.FindResource("Background");
			SolidColorBrush Shade = (SolidColorBrush)Application.Current.FindResource("Shade");

			LineSeries ideal = GraphCalc.CalcIdealGraph(TrackingDataHelper.CurrentSeasonUUID, epilogue);
			LineSeries performance = GraphCalc.CalcPerformanceGraph(TrackingDataHelper.CurrentSeasonUUID);
			LineSeries dailyIdeal = DashboardDataCalc.CalcDailyIdeal(performance, epilogue);
			LineSeries average = DashboardDataCalc.CalcAverageGraph(performance, epilogue);

			LineSeries idealPoint = DashboardDataCalc.CalcGraphPoint(ideal, OxyColor.FromArgb(GraphIdealPoint.Color.A, GraphIdealPoint.Color.R, GraphIdealPoint.Color.G, GraphIdealPoint.Color.B));
			LineSeries performancePoint = DashboardDataCalc.CalcGraphPoint(performance, OxyColors.Maroon);
			LineSeries dailyIdealPoint = DashboardDataCalc.CalcGraphPoint(dailyIdeal, OxyColors.Navy);

			RectangleAnnotation bufferZone = GraphCalc.CalcBufferZone(TrackingDataHelper.CurrentSeasonUUID, epilogue);

			Graph.LegendPosition = LegendPosition.LeftTop;
			Graph.LegendBackground = OxyColor.FromArgb(Background.Color.A, Background.Color.R, Background.Color.G, Background.Color.B);
			Graph.LegendTextColor = OxyColor.FromArgb(Foreground.Color.A, Foreground.Color.R, Foreground.Color.G, Foreground.Color.B);
			Graph.LegendBorder = OxyColor.FromArgb(Shade.Color.A, Shade.Color.R, Shade.Color.G, Shade.Color.B);

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

			Graph.InvalidatePlot(true);

			int t = performance.Points.Count - 1;
			int deviationIdeal = (int)performance.Points[t].Y - (int)ideal.Points[t].Y;

			int deviationDaily = 0;
			if(dailyIdeal.Points.Count > 0) deviationDaily = (int)performance.Points[t].Y - (int)dailyIdeal.Points[1].Y;

			return (deviationIdeal, deviationDaily);
		}

		private void UpdateYAxis(LineSeries performance, bool epilogue)
		{
			int maximum = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue).Total * 2;
			if (performance.Points.Last().Y >= maximum) maximum = (int)performance.Points.Last().Y + 20000;

			Graph.Axes[1].AbsoluteMaximum = maximum;
		}

		private void AddGraphLevels(bool epilogue)
		{
			List<LineSeries> levels = GraphCalc.CalcBattlepassLevels(TrackingDataHelper.CurrentSeasonUUID);
			foreach (LineSeries ls in levels) Graph.Series.Add(ls);

			if (!epilogue) return;

			List<LineSeries> epilogueLevels = GraphCalc.CalcEpilogueLevels(TrackingDataHelper.CurrentSeasonUUID);
			foreach (LineSeries ls in epilogueLevels) Graph.Series.Add(ls);
		}

		private void AddGraphGoals()
		{
			List<LineSeries> levels;
			List<TextAnnotation> annotations;
			(levels, annotations) = GoalDataCalc.CalcGraphGoals(TrackingDataHelper.CurrentSeasonUUID);

			foreach (LineSeries ls in levels) Graph.Series.Add(ls);
			foreach (TextAnnotation ta in annotations) Graph.Annotations.Add(ta);
		}
	}
}
