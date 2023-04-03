using OxyPlot;
using OxyPlot.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using OxyPlot.Axes;
using OxyPlot.Legends;
using AxisPosition = OxyPlot.Axes.AxisPosition;

namespace VexTrack.Core
{
	public static class GraphCalc
	{
		public static SeriesCollection CalcGraphs(int total, int startXp, int duration, int remainingDays, List<HistoryEntry> history)
		{
			var collection = new SeriesCollection();

			var mono = (Brush)Application.Current.FindResource("AccMono") ?? new SolidColorBrush();
			var accent = (Brush)Application.Current.FindResource("Accent") ?? new SolidColorBrush();

			var ideal = new LineSeries
			{
				Title = "Ideal",
				Values = new ChartValues<ObservablePoint>(),
				PointGeometry = null,
				Stroke = mono,
				Fill = Brushes.Transparent,
				LineSmoothness = 0,
			};
			
			var performance = new LineSeries
			{
				Title = "Performance",
				Values = new ChartValues<ObservablePoint>(),
				PointGeometry = null,
				Stroke = accent,
				StrokeThickness = 3,
				Fill = Brushes.Transparent,
				LineSmoothness = 0
			};
			
			var startRemaining = total - startXp;
			var totalDaily = startRemaining / (double)(duration - SettingsHelper.Data.BufferDays);
			var dailyAmounts = CalcUtil.CalcCollectedPerDay(history, duration);

			var prevPerformanceVal = 0;
			
			for (var i = 0; i < duration; i++)
			{
				// Ideal
				var idealVal = (int)Math.Ceiling(i * totalDaily + startXp);
				if (idealVal > total) idealVal = total;

				ideal.Values.Add(new ObservablePoint(i, idealVal));

				// Performance
				if(i >= duration - remainingDays) continue;
				
				var performanceVal = prevPerformanceVal + dailyAmounts[i];
				prevPerformanceVal = performanceVal;

				performance.Values.Add(new ObservablePoint(i, performanceVal));
			}

			collection.Add(ideal);
			collection.Add(performance);
			
			return collection;
		}
		
		public static OxyPlot.Series.LineSeries CalcIdealGraphOld(string sUuid, bool epilogue)
		{
			OxyPlot.Series.LineSeries ret = new();

			var foreground = (SolidColorBrush)Application.Current.FindResource("Foreground") ?? new SolidColorBrush();

			var total = CalcUtil.CalcMaxForSeason(epilogue);
			var bufferDays = SettingsHelper.Data.BufferDays;
			var duration = TrackingData.GetDuration(sUuid);

			var initCollected = TrackingData.GetFirstHistoryEntry(sUuid).Amount;
			var initRemaining = total - initCollected;
			var totalDaily = initRemaining / (double)(duration - bufferDays);

			ret.Points.Add(new DataPoint(0, initCollected));
			for (var i = 1; i < duration + 1; i++)
			{
				var value = (int)Math.Ceiling(i * totalDaily + initCollected);
				if (value > total) value = total;

				ret.Points.Add(new DataPoint(i, value));
			}

			ret.Color = OxyColor.FromArgb(foreground.Color.A, foreground.Color.R, foreground.Color.G, foreground.Color.B);
			ret.StrokeThickness = 2;
			ret.Title = "Ideal";
			return ret;
		}

		public static OxyPlot.Series.LineSeries CalcPerformanceGraphOld(string sUuid)
		{
			OxyPlot.Series.LineSeries ret = new();

			DateTimeOffset prevDate = DateTimeOffset.FromUnixTimeSeconds(TrackingData.GetFirstHistoryEntry(sUuid).Time).ToLocalTime().Date;
			List<int> dailyAmounts = new();

			// Construct list of daily amounts

			foreach (var h in TrackingData.GetSeason(sUuid).History)
			{
				DateTimeOffset currDate = DateTimeOffset.FromUnixTimeSeconds(h.Time).ToLocalTime().Date;
				if (currDate == prevDate && dailyAmounts.Count != 0)
				{
					dailyAmounts[^1] += h.Amount;
					continue;
				}

				var gapSize = (currDate - prevDate).Days;
				prevDate = currDate;

				var prevValue = 0;
				if (dailyAmounts.Count != 0) prevValue = dailyAmounts.Last();

				for (var i = 1; i < gapSize; i++) dailyAmounts.Add(prevValue);
				dailyAmounts.Add(h.Amount + prevValue);
			}

			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			DateTimeOffset seasonEndDate = DateTimeOffset.FromUnixTimeSeconds(TrackingData.GetSeason(sUuid).EndDate).ToLocalTime().Date;

			var inactiveDays = today < seasonEndDate ? (today - prevDate).Days : (seasonEndDate - prevDate).Days;
			for (var i = 0; i < inactiveDays; i++) dailyAmounts.Add(dailyAmounts.Last());

			// Translate list to data points

			var idx = 0;
			foreach (var amount in dailyAmounts) ret.Points.Add(new DataPoint(idx++, amount));

			ret.Color = OxyColors.Red;
			ret.StrokeThickness = 4;
			ret.Title = "Performance";
			return ret;
		}

		public static RectangleAnnotation CalcBufferZoneOld(string sUuid, bool epilogue)
		{
			RectangleAnnotation ret = new();

			var total = CalcUtil.CalcMaxForSeason(epilogue);
			var bufferDays = SettingsHelper.Data.BufferDays;
			var duration = TrackingData.GetDuration(sUuid);

			ret.MinimumX = duration - bufferDays;
			ret.MaximumX = duration;
			ret.MinimumY = 0;
			ret.MaximumY = total * 3;

			ret.Fill = OxyColor.FromAColor(64, OxyColors.Red);
			return ret;
		}

		public static List<OxyPlot.Series.LineSeries> CalcBattlepassLevelsOld(string sUuid)
		{
			List<OxyPlot.Series.LineSeries> ret = new();

			for (var i = 0; i < Constants.BattlepassLevels + 1; i++)
			{
				OxyPlot.Series.LineSeries ls = new();
				byte alpha = 128;
				var val = CalcUtil.CalcMaxForSeason(false);

				ls.Points.Add(new DataPoint(0, val));
				ls.Points.Add(new DataPoint(TrackingData.GetDuration(sUuid), val));

				if (CalcUtil.CalcTotalCollected(TrackingData.CurrentSeasonData.ActiveBpLevel, TrackingData.CurrentSeasonData.Cxp) >= val) alpha = 13;

				ls.Color = OxyColor.FromAColor(alpha, i % 5 == 0 ? OxyColors.LimeGreen : OxyColors.LightGray);

				ret.Add(ls);
			}

			return ret;
		}

		public static List<OxyPlot.Series.LineSeries> CalcEpilogueLevelsOld(string sUuid)
		{
			List<OxyPlot.Series.LineSeries> ret = new();

			for (var i = 1; i < Constants.EpilogueLevels + 1; i++)
			{
				OxyPlot.Series.LineSeries ls = new();
				byte alpha = 128;
				var val = CalcUtil.CalcMaxForSeason(true) + i * Constants.XpPerEpilogueLevel;

				ls.Points.Add(new DataPoint(0, val));
				ls.Points.Add(new DataPoint(TrackingData.GetDuration(sUuid), val));

				if (CalcUtil.CalcTotalCollected(TrackingData.CurrentSeasonData.ActiveBpLevel, TrackingData.CurrentSeasonData.Cxp) >= val) alpha = 13;

				ls.Color = OxyColor.FromAColor(alpha, OxyColors.Gold);

				ret.Add(ls);
			}

			return ret;
		}
		
		
		
		// ================================================================
		//  PlotModel
		// ================================================================
		
		public static PlotModel UpdateGraph(PlotModel model, bool epilogue, string seasonUuid)
		{
			model = ApplyGraphAxes(model, seasonUuid);
			
			var graphIdealPoint = (SolidColorBrush)Application.Current.FindResource("GraphIdealPoint") ?? new SolidColorBrush();

			var foreground = (SolidColorBrush)Application.Current.FindResource("Foreground") ?? new SolidColorBrush();
			var background = (SolidColorBrush)Application.Current.FindResource("Background") ?? new SolidColorBrush();
			var shade = (SolidColorBrush)Application.Current.FindResource("Shade") ?? new SolidColorBrush();

			var ideal = GraphCalc.CalcIdealGraphOld(seasonUuid, epilogue);
			var performance = GraphCalc.CalcPerformanceGraphOld(seasonUuid);
			
			var bufferZone = GraphCalc.CalcBufferZoneOld(seasonUuid, epilogue);

			var legend = new Legend
			{
				LegendPosition = LegendPosition.LeftTop,
				LegendBackground = OxyColor.FromArgb(background.Color.A, background.Color.R, background.Color.G, background.Color.B),
				LegendTextColor = OxyColor.FromArgb(foreground.Color.A, foreground.Color.R, foreground.Color.G, foreground.Color.B),
				LegendBorder = OxyColor.FromArgb(shade.Color.A, shade.Color.R, shade.Color.G, shade.Color.B)
			};

			model.Legends.Add(legend);
			model.Series.Clear();
			model.Annotations.Clear();

			model = AddGraphLevels(model, epilogue, seasonUuid);
			model = UpdateYAxis(model, performance, epilogue);

			model.Series.Add(ideal);
			model.Series.Add(performance);

			// Only add points to current season
			if (seasonUuid == TrackingData.CurrentSeasonData.Uuid)
			{
				var idealPoint = DashboardDataCalc.CalcGraphPoint(ideal, OxyColor.FromArgb(graphIdealPoint.Color.A, graphIdealPoint.Color.R, graphIdealPoint.Color.G, graphIdealPoint.Color.B));
				var performancePoint = DashboardDataCalc.CalcGraphPoint(performance, OxyColors.Maroon);

				model.Series.Add(idealPoint);
				model.Series.Add(performancePoint);
			}

			model.Annotations.Add(bufferZone);
			model.InvalidatePlot(true);
			return model;
		}
		
		private static PlotModel ApplyGraphAxes(PlotModel model, string seasonUuid)
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
			xAxis.AbsoluteMaximum = TrackingData.GetDuration(seasonUuid);
			xAxis.TextColor = OxyColor.FromArgb(foreground.Color.A, foreground.Color.R, foreground.Color.G, foreground.Color.B);

			yAxis.Position = AxisPosition.Left;
			yAxis.AbsoluteMinimum = 0;
			yAxis.MinimumPadding = 0;
			yAxis.TickStyle = TickStyle.None;
			yAxis.TextColor = OxyColors.Transparent;

			model.Axes.Clear();
			model.Axes.Add(xAxis);
			model.Axes.Add(yAxis);

			return model;
		}

		private static PlotModel AddGraphLevels(PlotModel model, bool epilogue, string seasonUuid)
		{
			var levels = CalcBattlepassLevelsOld(seasonUuid);
			foreach (var ls in levels) model.Series.Add(ls);

			if (!epilogue) return model;

			var epilogueLevels = CalcEpilogueLevelsOld(seasonUuid);
			foreach (var ls in epilogueLevels) model.Series.Add(ls);

			return model;
		}
		
		private static PlotModel UpdateYAxis(PlotModel model, OxyPlot.Series.DataPointSeries performance, bool epilogue)
		{
			var maximum = CalcUtil.CalcMaxForSeason(epilogue) * 2;
			if (performance.Points.Last().Y >= maximum) maximum = (int)performance.Points.Last().Y + 20000;

			model.Axes[1].AbsoluteMaximum = maximum;
			return model;
		}
	}
}
