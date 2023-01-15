using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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
	}
}
