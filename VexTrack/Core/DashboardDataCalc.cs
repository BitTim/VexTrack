using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;

namespace VexTrack.Core
{
	public static class DashboardDataCalc
	{
		public static DailyData CalcDailyData(bool epilogue)
		{
			DailyData ret = new();

			var normalRemaining = CalcUtil.CalcMaxForSeason(false) - CalcUtil.CalcTotalCollected(TrackingData.CurrentSeasonData.ActiveBpLevel, TrackingData.CurrentSeasonData.Cxp);
			var epilogueRemaining = CalcUtil.CalcMaxForSeason(true) - CalcUtil.CalcTotalCollected(TrackingData.CurrentSeasonData.ActiveBpLevel, TrackingData.CurrentSeasonData.Cxp);

			var remaining = epilogue ? epilogueRemaining : normalRemaining;

			var bufferDays = TrackingData.CurrentSeasonData.BufferDays;
			var idealRemainingDays = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid) - bufferDays;

			if (idealRemainingDays <= -bufferDays && idealRemainingDays <= 0)
			{
				TrackingData.CreateSeasonInitPopup();
				return ret;
			}

			if (idealRemainingDays > -bufferDays && idealRemainingDays <= 0) idealRemainingDays = 1;

			var dailyCollected = TrackingData.CurrentSeasonData.History.GetRange(1, TrackingData.CurrentSeasonData.History.Count - 1)
				.Where(h => DateTimeOffset.FromUnixTimeSeconds(h.Time)
					.ToLocalTime().Date == DateTimeOffset.Now.ToLocalTime().Date).Sum(h => h.Amount);

			var total = (int)Math.Ceiling((remaining + dailyCollected) / (double)idealRemainingDays);
			if (total <= 0) total = 0;

			ret.Total = total;
			ret.Collected = dailyCollected;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);

			if (CalcUtil.CalcProgress((int)MathF.Round((epilogueRemaining + dailyCollected) / (float)idealRemainingDays), dailyCollected) >= 100 && dailyCollected > 0) StreakDataCalc.SetStreakEntry(DateTimeOffset.Now.ToLocalTime().Date, Constants.StreakStatusOrder.Keys.ElementAt(2));
			else if (CalcUtil.CalcProgress((int)MathF.Round((normalRemaining + dailyCollected) / (float)idealRemainingDays), dailyCollected) >= 100 && dailyCollected > 0) StreakDataCalc.SetStreakEntry(DateTimeOffset.Now.ToLocalTime().Date, Constants.StreakStatusOrder.Keys.ElementAt(1));
			else StreakDataCalc.SetStreakEntry(DateTimeOffset.Now.ToLocalTime().Date, Constants.StreakStatusOrder.Keys.ElementAt(0));

			ret.Streak = StreakDataCalc.CalcCurrentStreak(epilogue);
			return ret;
		}

		public static LiveCharts.Wpf.LineSeries CalcDailyIdeal(LiveCharts.Wpf.LineSeries performance) // TODO: Refactor this
		{
			var win = (Brush)Application.Current.FindResource("Win") ?? new SolidColorBrush();
			
			var ret = new LiveCharts.Wpf.LineSeries()
			{
				Title = "Daily Ideal",
				Values = new ChartValues<ObservablePoint>(),
				PointGeometry = null,
				Stroke = win,
				Fill = Brushes.Transparent,
				LineSmoothness = 0,
				StrokeDashArray = new DoubleCollection { 0.5, 0.5 },
			};
			
			List<int> amounts = new();
			var total = CalcUtil.CalcMaxForSeason(true);
			var bufferDays = TrackingData.CurrentSeasonData.BufferDays;
			var effectiveRemaining = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid) - bufferDays + 1;
			if (effectiveRemaining <= 0) effectiveRemaining = 1;
			
			var remainingDays = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid);
			var duration = TrackingData.GetDuration(TrackingData.CurrentSeasonData.Uuid);
			var graphOffset = duration - remainingDays - 1;
			
			var startOffset = 0;
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			DateTimeOffset lastEntryDate = DateTimeOffset.FromUnixTimeSeconds(TrackingData.GetLastHistoryEntry(TrackingData.CurrentSeasonData.Uuid).Time).ToLocalTime().Date;
			if (lastEntryDate == today) startOffset = 1;
			
			var initAmount = (int)((ObservablePoint)performance.Values[0]).Y;
			if (performance.Values.Count > 1) initAmount = (int)((ObservablePoint)performance.Values[performance.Values.Count - 1 - startOffset]).Y;
			var dailyTotal = (total - initAmount) / (double)effectiveRemaining;
			if (dailyTotal <= 0) return ret;

			amounts.Add(initAmount);
			
			for (var i = 1; i < remainingDays + 1; i++)
			{
				var amount = (int)Math.Ceiling(i * dailyTotal + initAmount);
				if (amount > total) amount = total;
				amounts.Add(amount);
			}

			var idx = 0;
			foreach (var amount in amounts) ret.Values.Add(new ObservablePoint(graphOffset + idx++, amount));

			return ret;
		}
		
		public static LineSeries CalcDailyIdealOld(LineSeries performance, bool epilogue)
		{
			LineSeries ret = new();

			List<int> amounts = new();
			var total = CalcUtil.CalcMaxForSeason(epilogue);
			var bufferDays = TrackingData.CurrentSeasonData.BufferDays;
			var effectiveRemaining = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid) - bufferDays + 1;
			if (effectiveRemaining <= 0) effectiveRemaining = 1;

			var remainingDays = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid);
			var duration = TrackingData.GetDuration(TrackingData.CurrentSeasonData.Uuid);
			var graphOffset = duration - remainingDays - 1;

			var startOffset = 0;
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			DateTimeOffset lastEntryDate = DateTimeOffset.FromUnixTimeSeconds(TrackingData.GetLastHistoryEntry(TrackingData.CurrentSeasonData.Uuid).Time).ToLocalTime().Date;
			if (lastEntryDate == today) startOffset = 1;

			var initAmount = (int)performance.Points.First().Y;
			if (performance.Points.Count > 1) initAmount = (int)performance.Points[performance.Points.Count - 1 - startOffset].Y;
			var dailyTotal = (total - initAmount) / (double)effectiveRemaining;
			if (dailyTotal <= 0) return ret;

			amounts.Add(initAmount);

			for (var i = 1; i < remainingDays + 2; i++)
			{
				var amount = (int)Math.Ceiling(i * dailyTotal + initAmount);
				if (amount > total) amount = total;
				amounts.Add(amount);
			}

			var idx = 0;
			foreach (var amount in amounts) ret.Points.Add(new DataPoint(graphOffset + idx++, amount));

			ret.Color = OxyColors.SteelBlue;
			ret.StrokeThickness = 2;
			ret.LineStyle = LineStyle.Dash;
			ret.Title = "Daily Ideal";
			return ret;
		}

		public static LiveCharts.Wpf.LineSeries CalcAverageGraph(LiveCharts.Wpf.LineSeries performance)
		{
			var loss = (Brush)Application.Current.FindResource("Loss") ?? new SolidColorBrush();
			
			var ret = new LiveCharts.Wpf.LineSeries()
			{
				Title = "Average",
				Values = new ChartValues<ObservablePoint>(),
				PointGeometry = null,
				Stroke = loss,
				Fill = Brushes.Transparent,
				LineSmoothness = 0,
				StrokeDashArray = new DoubleCollection { 0.1, 0.1 },
			};

			var total = CalcUtil.CalcMaxForSeason(true);
			var duration = TrackingData.GetDuration(TrackingData.CurrentSeasonData.Uuid);
			var daysPassed = duration - TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid);
			var totalCollected = CalcUtil.CalcTotalCollected(TrackingData.CurrentSeasonData.ActiveBpLevel, TrackingData.CurrentSeasonData.Cxp);
			var average = (int)MathF.Round((float)totalCollected / (daysPassed + 1));

			if (totalCollected >= total) return ret;

			ret.Values.Add(new ObservablePoint(daysPassed - 1, ((ObservablePoint)performance.Values[daysPassed - 1]).Y));

			for (var i = daysPassed; i < duration; i++)
			{
				var amount = (int)((ObservablePoint)ret.Values[^1]).Y + average;
				if (amount > total)
				{
					var x = (total - ((ObservablePoint)performance.Values[daysPassed - 1]).Y) / average + daysPassed - 1;
					ret.Values.Add(new DataPoint(x, total));
					break;
				}

				ret.Values.Add(new ObservablePoint(i, amount));
			}
			
			return ret;
		}
		
		public static LineSeries CalcAverageGraphOld(LineSeries performance, bool epilogue)
		{
			LineSeries ret = new();

			var total = CalcUtil.CalcMaxForSeason(epilogue);
			var duration = TrackingData.GetDuration(TrackingData.CurrentSeasonData.Uuid);
			var daysPassed = duration - TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid);
			var totalCollected = CalcUtil.CalcTotalCollected(TrackingData.CurrentSeasonData.ActiveBpLevel, TrackingData.CurrentSeasonData.Cxp);
			var average = (int)MathF.Round((float)totalCollected / (daysPassed + 1));

			if (totalCollected >= total) return ret;

			ret.Points.Add(new DataPoint(daysPassed - 1, performance.Points[daysPassed - 1].Y));

			for (var i = daysPassed; i < duration + 1; i++)
			{
				var amount = (int)ret.Points.Last().Y + average;
				if (amount > total)
				{
					var x = (total - performance.Points[daysPassed - 1].Y) / average + daysPassed - 1;
					ret.Points.Add(new DataPoint(x, total));
					break;
				}

				ret.Points.Add(new DataPoint(i, amount));
			}

			ret.Color = OxyColors.PaleVioletRed;
			ret.StrokeThickness = 2;
			ret.LineStyle = LineStyle.Dot;
			ret.Title = "Average";
			return ret;
		}

		public static LineSeries CalcGraphPoint(LineSeries series, OxyColor color)
		{
			LineSeries ret = new();

			var remainingDays = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid);
			var duration = TrackingData.GetDuration(TrackingData.CurrentSeasonData.Uuid);
			var xPos = duration - remainingDays;

			ret.Points.Add(new DataPoint(xPos, series.Points.Find(p => (int)p.X == xPos).Y));

			ret.MarkerType = MarkerType.Circle;
			ret.MarkerFill = color;
			ret.MarkerSize = 4;

			return ret;
		}

		public static int CalcDaysFinished(bool epilogue)
		{
			var daysFinished = 0;

			var total = CalcUtil.CalcMaxForSeason(epilogue);
			var duration = TrackingData.GetDuration(TrackingData.CurrentSeasonData.Uuid);
			var remainingDays = TrackingData.GetRemainingDays(TrackingData.CurrentSeasonData.Uuid);
			var daysPassed = duration - remainingDays;
			var totalCollected = CalcUtil.CalcTotalCollected(TrackingData.CurrentSeasonData.ActiveBpLevel, TrackingData.CurrentSeasonData.Cxp);
			var average = (int)MathF.Round((float)totalCollected / (daysPassed + 1));

			var val = totalCollected;
			for (var i = 0; i < remainingDays + 1; i++)
			{
				val += average;
				daysFinished++;

				if (val >= total) break;
			}

			if (daysFinished > remainingDays) daysFinished = -1;
			return daysFinished;
		}
	}

	public class DailyData
	{
		public double Progress { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }
		public int Streak { get; set; }

		public DailyData() { }
		public DailyData(double progress, int collected, int remaining, int total, int streak)
		{
			(Progress, Collected, Remaining, Total, Streak) = (progress, collected, remaining, total, streak);
		}
	}
}
