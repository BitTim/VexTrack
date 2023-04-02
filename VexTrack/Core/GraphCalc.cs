using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using OxyPlot.Axes;
using OxyPlot.Legends;

namespace VexTrack.Core
{
	public static class GraphCalc
	{
		public static LineSeries CalcIdealGraph(string sUuid, bool epilogue)
		{
			LineSeries ret = new();

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

		public static LineSeries CalcPerformanceGraph(string sUuid)
		{
			LineSeries ret = new();

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

		public static RectangleAnnotation CalcBufferZone(string sUuid, bool epilogue)
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

		public static List<LineSeries> CalcBattlepassLevels(string sUuid)
		{
			List<LineSeries> ret = new();

			for (var i = 0; i < Constants.BattlepassLevels + 1; i++)
			{
				LineSeries ls = new();
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

		public static List<LineSeries> CalcEpilogueLevels(string sUuid)
		{
			List<LineSeries> ret = new();

			for (var i = 1; i < Constants.EpilogueLevels + 1; i++)
			{
				LineSeries ls = new();
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

			var ideal = GraphCalc.CalcIdealGraph(seasonUuid, epilogue);
			var performance = GraphCalc.CalcPerformanceGraph(seasonUuid);
			
			var bufferZone = GraphCalc.CalcBufferZone(seasonUuid, epilogue);

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
			var levels = CalcBattlepassLevels(seasonUuid);
			foreach (var ls in levels) model.Series.Add(ls);

			if (!epilogue) return model;

			var epilogueLevels = CalcEpilogueLevels(seasonUuid);
			foreach (var ls in epilogueLevels) model.Series.Add(ls);

			return model;
		}
		
		private static PlotModel UpdateYAxis(PlotModel model, DataPointSeries performance, bool epilogue)
		{
			var maximum = CalcUtil.CalcMaxForSeason(epilogue) * 2;
			if (performance.Points.Last().Y >= maximum) maximum = (int)performance.Points.Last().Y + 20000;

			model.Axes[1].AbsoluteMaximum = maximum;
			return model;
		}
	}
}
