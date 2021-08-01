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

namespace VexTrack.MVVM.ViewModel.Popups
{
	class SeasonPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnEditClicked { get; set; }
		public RelayCommand OnDeleteClicked { get; set; }
		//private EditableSeasonPopupViewModel EditableSeasonPopup { get; set; }

		private SeasonEntryData RawData { get; set; }
		public string UUID { get; set; }
		public string Title { get; set; }
		public double Progress { get; set; }
		public int Average { get; set; }
		public long EndDate { get; set; }
		public int StrongestAmount { get; set; }
		public long StrongestDate { get; set; }
		public int WeakestAmount { get; set; }
		public long WeakestDate { get; set; }

		public bool CanDelete { get; set; }
		public bool CanEdit { get; set; }

		private PlotModel _graph;

		public PlotModel Graph
		{
			get => _graph;
			set
			{
				_graph = value;
				OnPropertyChanged();
			}
		}

		public SeasonPopupViewModel()
		{
			//EditableSeasonPopup = (EditableSeasonPopupViewModel)ViewModelManager.ViewModels["EditableSeasonPopup"];
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { Close(); });
			OnEditClicked = new RelayCommand(o => {
				//EditableSeasonPopup.SetParameters("Edit Season", true);
				//EditableSeasonPopup.SetData(RawData);
				//MainVM.QueuePopup(EditableSeasonPopup);
			});
			OnDeleteClicked = new RelayCommand(o => {
				IsInitialized = false;
				//TrackingDataHelper.RemoveSeason(UUID);
			});

			SolidColorBrush Foreground = (SolidColorBrush)Application.Current.FindResource("Foreground");
			SolidColorBrush Background = (SolidColorBrush)Application.Current.FindResource("Background");
			SolidColorBrush Shade = (SolidColorBrush)Application.Current.FindResource("Shade");

			Graph = new PlotModel();
			Graph.PlotMargins = new OxyThickness(0, 0, 0, 16);
			Graph.PlotAreaBorderColor = OxyColors.Transparent;

			Graph.LegendPosition = LegendPosition.LeftTop;
			Graph.LegendBackground = OxyColor.FromArgb(Background.Color.A, Background.Color.R, Background.Color.G, Background.Color.B);
			Graph.LegendTextColor = OxyColor.FromArgb(Foreground.Color.A, Foreground.Color.R, Foreground.Color.G, Foreground.Color.B);
			Graph.LegendBorder = OxyColor.FromArgb(Shade.Color.A, Shade.Color.R, Shade.Color.G, Shade.Color.B);
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
			xAxis.AbsoluteMaximum = TrackingDataHelper.GetDuration(UUID);
			xAxis.TextColor = OxyColor.FromArgb(Foreground.Color.A, Foreground.Color.R, Foreground.Color.G, Foreground.Color.B);

			yAxis.Position = AxisPosition.Left;
			yAxis.AbsoluteMinimum = 0;
			yAxis.MinimumPadding = 0;
			yAxis.TickStyle = TickStyle.None;
			yAxis.TextColor = OxyColors.Transparent;

			Graph.Axes.Add(xAxis);
			Graph.Axes.Add(yAxis);
		}

		private void UpdateGraph(bool epilogue)
		{
			SolidColorBrush GraphIdealPoint = (SolidColorBrush)Application.Current.FindResource("GraphIdealPoint");

			LineSeries ideal = GraphCalc.CalcIdealGraph(UUID, epilogue);
			LineSeries performance = GraphCalc.CalcPerformanceGraph(UUID);

			LineSeries idealPoint = DashboardDataCalc.CalcGraphPoint(ideal, OxyColor.FromArgb(GraphIdealPoint.Color.A, GraphIdealPoint.Color.R, GraphIdealPoint.Color.G, GraphIdealPoint.Color.B));
			LineSeries performancePoint = DashboardDataCalc.CalcGraphPoint(performance, OxyColors.Maroon);

			RectangleAnnotation bufferZone = GraphCalc.CalcBufferZone(UUID, epilogue);

			Graph.Series.Clear();
			Graph.Annotations.Clear();

			AddGraphLevels(epilogue);
			UpdateYAxis(performance, epilogue);

			Graph.Series.Add(ideal);
			Graph.Series.Add(performance);

			Graph.Series.Add(idealPoint);
			Graph.Series.Add(performancePoint);

			Graph.Annotations.Add(bufferZone);

			Graph.InvalidatePlot(true);
		}

		private void UpdateYAxis(LineSeries performance, bool epilogue)
		{
			int maximum = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue).Total * 2;
			if (performance.Points.Last().Y >= maximum) maximum = (int)performance.Points.Last().Y + 20000;

			Graph.Axes[1].AbsoluteMaximum = maximum;
		}

		public void SetFlags(bool canDelete, bool canEdit)
		{
			CanDelete = canDelete;
			CanEdit = canEdit;
		}

		public void SetData(SeasonEntryData data, bool epilogue)
		{
			RawData = data;

			UUID = data.UUID;
			Title = data.Title;
			Progress = data.Progress;
			Average = data.Average;
			EndDate = data.EndDate;
			StrongestAmount = data.StrongestAmount;
			StrongestDate = data.StrongestDate.ToUnixTimeSeconds();
			WeakestAmount = data.WeakestAmount;
			WeakestDate = data.WeakestDate.ToUnixTimeSeconds();

			IsInitialized = true;
			ApplyAxes();
			UpdateGraph(epilogue);
		}

		private void AddGraphLevels(bool epilogue)
		{
			List<LineSeries> levels = GraphCalc.CalcBattlepassLevels(UUID);
			foreach (LineSeries ls in levels) Graph.Series.Add(ls);

			if (!epilogue) return;

			List<LineSeries> epilogueLevels = GraphCalc.CalcEpilogueLevels(UUID);
			foreach (LineSeries ls in epilogueLevels) Graph.Series.Add(ls);
		}

		public override void Close()
		{
			//EditableSeasonPopup.Close();
			base.Close();
		}
	}
}
