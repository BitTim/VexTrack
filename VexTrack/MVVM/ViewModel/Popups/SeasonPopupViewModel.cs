using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;
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
		private EditableSeasonPopupViewModel EditableSeasonPopup { get; set; }

		private Season RawData { get; set; }
		public string Uuid { get; set; }
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
			EditableSeasonPopup = (EditableSeasonPopupViewModel)ViewModelManager.ViewModels["EditableSeasonPopup"];
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { Close(); });
			OnEditClicked = new RelayCommand(o =>
			{
				EditableSeasonPopup.SetParameters("Edit Season", true);
				EditableSeasonPopup.SetData(RawData);
				MainVm.QueuePopup(EditableSeasonPopup);
			});
			OnDeleteClicked = new RelayCommand(o =>
			{
				IsInitialized = false;
				TrackingData.RemoveSeason(Uuid);
			});

			Graph = new PlotModel
			{
				PlotMargins = new OxyThickness(0, 0, 0, 16),
				PlotAreaBorderColor = OxyColors.Transparent
			};
		}

		private void ApplyAxes()
		{
			LinearAxis xAxis = new();
			LinearAxis yAxis = new();

			var foreground = (SolidColorBrush)Application.Current.FindResource("Foreground") ?? new SolidColorBrush();
			var shade = (SolidColorBrush)Application.Current.FindResource("Shade") ?? new SolidColorBrush();

			xAxis.Position = AxisPosition.Bottom;
			xAxis.AbsoluteMinimum = 0;
			xAxis.TickStyle = TickStyle.None;
			xAxis.MajorStep = 5;
			xAxis.MajorGridlineStyle = LineStyle.Dot;
			xAxis.MajorGridlineColor = OxyColor.FromArgb(shade.Color.A, shade.Color.R, shade.Color.G, shade.Color.B);
			xAxis.MaximumPadding = 0;
			xAxis.MinimumPadding = 0;
			xAxis.AbsoluteMaximum = TrackingData.GetDuration(Uuid);
			xAxis.TextColor = OxyColor.FromArgb(foreground.Color.A, foreground.Color.R, foreground.Color.G, foreground.Color.B);

			yAxis.Position = AxisPosition.Left;
			yAxis.AbsoluteMinimum = 0;
			yAxis.MinimumPadding = 0;
			yAxis.TickStyle = TickStyle.None;
			yAxis.TextColor = OxyColors.Transparent;

			Graph.Axes.Clear();
			Graph.Axes.Add(xAxis);
			Graph.Axes.Add(yAxis);
		}

		private void UpdateGraph(bool epilogue)
		{
			var graphIdealPoint = (SolidColorBrush)Application.Current.FindResource("GraphIdealPoint") ?? new SolidColorBrush();

			var foreground = (SolidColorBrush)Application.Current.FindResource("Foreground") ?? new SolidColorBrush();
			var background = (SolidColorBrush)Application.Current.FindResource("Background") ?? new SolidColorBrush();
			var shade = (SolidColorBrush)Application.Current.FindResource("Shade") ?? new SolidColorBrush();

			var ideal = GraphCalc.CalcIdealGraph(Uuid, epilogue);
			var performance = GraphCalc.CalcPerformanceGraph(Uuid);
			
			var bufferZone = GraphCalc.CalcBufferZone(Uuid, epilogue);

			Graph.LegendPosition = LegendPosition.LeftTop;
			Graph.LegendBackground = OxyColor.FromArgb(background.Color.A, background.Color.R, background.Color.G, background.Color.B);
			Graph.LegendTextColor = OxyColor.FromArgb(foreground.Color.A, foreground.Color.R, foreground.Color.G, foreground.Color.B);
			Graph.LegendBorder = OxyColor.FromArgb(shade.Color.A, shade.Color.R, shade.Color.G, shade.Color.B);

			Graph.Series.Clear();
			Graph.Annotations.Clear();

			AddGraphLevels(epilogue);
			UpdateYAxis(performance, epilogue);

			Graph.Series.Add(ideal);
			Graph.Series.Add(performance);

			// Only add points to current season
			if (Uuid == TrackingData.CurrentSeasonData.Uuid)
			{
				var idealPoint = DashboardDataCalc.CalcGraphPoint(ideal, OxyColor.FromArgb(graphIdealPoint.Color.A, graphIdealPoint.Color.R, graphIdealPoint.Color.G, graphIdealPoint.Color.B));
				var performancePoint = DashboardDataCalc.CalcGraphPoint(performance, OxyColors.Maroon);

				Graph.Series.Add(idealPoint);
				Graph.Series.Add(performancePoint);
			}

			Graph.Annotations.Add(bufferZone);

			Graph.InvalidatePlot(true);
		}

		private void UpdateYAxis(DataPointSeries performance, bool epilogue)
		{
			var maximum = CalcUtil.CalcMaxForSeason(epilogue) * 2;
			if (performance.Points.Last().Y >= maximum) maximum = (int)performance.Points.Last().Y + 20000;

			Graph.Axes[1].AbsoluteMaximum = maximum;
		}

		public void SetFlags(bool canDelete, bool canEdit)
		{
			CanDelete = canDelete;
			CanEdit = canEdit;
		}

		public void SetData(Season data, bool epilogue)
		{
			RawData = data;

			Uuid = data.Uuid;
			Title = data.Name;
			Progress = data.Progress;
			Average = data.Average;
			EndDate = data.EndDate;
				
			var extremes = data.Extremes;
			StrongestAmount = extremes.StrongestDayAmount;
			StrongestDate = extremes.StrongestDayTimestamp;
			WeakestAmount = extremes.WeakestDayAmount;
			WeakestDate = extremes.WeakestDayTimestamp;
			
			IsInitialized = true;
			ApplyAxes();
			UpdateGraph(epilogue);
		}

		private void AddGraphLevels(bool epilogue)
		{
			var levels = GraphCalc.CalcBattlepassLevels(Uuid);
			foreach (var ls in levels) Graph.Series.Add(ls);

			if (!epilogue) return;

			var epilogueLevels = GraphCalc.CalcEpilogueLevels(Uuid);
			foreach (var ls in epilogueLevels) Graph.Series.Add(ls);
		}

		public override void Close()
		{
			EditableSeasonPopup.Close();
			base.Close();
		}
	}
}
