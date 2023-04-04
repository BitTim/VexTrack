using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace VexTrack.Core
{
	public static class DashboardDataCalc
	{
		public static DailyData CalcDailyData()
		{
			DailyData ret = new();
			
			var currentSeasonData = TrackingData.CurrentSeasonData;
			
			var idealRemainingDays = currentSeasonData.RemainingDays - currentSeasonData.BufferDays;
			if (idealRemainingDays > -currentSeasonData.BufferDays && idealRemainingDays <= 0) idealRemainingDays = 1;

			var today = DateTimeOffset.Now.ToLocalTime().Date;
			var dayIndex = (today - DateTimeOffset.FromUnixTimeSeconds(currentSeasonData.StartDate)).Days + 1;
			if (dayIndex < 0 || dayIndex >= currentSeasonData.Duration) return ret;

			var collectedPerDay = CalcUtil.CalcCollectedPerDay(currentSeasonData.History, currentSeasonData.Duration);
			var totalToday = (int)Math.Ceiling(currentSeasonData.Remaining / (double)idealRemainingDays);
			var totalTodayMin = (int)Math.Ceiling(currentSeasonData.RemainingMin / (double)idealRemainingDays);
			if (totalToday <= 0) totalToday = 0;
			if (totalTodayMin <= 0) totalTodayMin = 0;

			var segments = new List<int> { totalTodayMin, totalToday - totalTodayMin };
			
			ret.Total = totalToday;
			ret.Collected = collectedPerDay[dayIndex];
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Segments = segments;

			if((today - DateTimeOffset.FromUnixTimeSeconds(TrackingData.LastStreakUpdateTimestamp)).Days > 1) TrackingData.Streak = 0;
			else if (collectedPerDay[dayIndex] > 0 && !DateTimeOffset.FromUnixTimeSeconds(TrackingData.LastStreakUpdateTimestamp).Equals(today))
			{
				TrackingData.Streak++;
				TrackingData.LastStreakUpdateTimestamp = ((DateTimeOffset)today).ToUnixTimeSeconds();
			}

			ret.Streak = TrackingData.Streak;
			return ret;
		}

		public static LineSeries CalcDailyIdeal(LineSeries performance) // TODO: Refactor this
		{
			var win = (Brush)Application.Current.FindResource("Win") ?? new SolidColorBrush();
			
			var ret = new LineSeries()
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
			var remainingDays = TrackingData.CurrentSeasonData.RemainingDays;
			var duration = TrackingData.CurrentSeasonData.Duration;
			
			var effectiveRemaining = remainingDays - bufferDays + 1;
			if (effectiveRemaining <= 0) effectiveRemaining = 1;
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

		public static LineSeries CalcAverageGraph(LineSeries performance)
		{
			var loss = (Brush)Application.Current.FindResource("Loss") ?? new SolidColorBrush();
			
			var ret = new LineSeries()
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
			var duration = TrackingData.CurrentSeasonData.Duration;
			var daysPassed = duration - TrackingData.CurrentSeasonData.RemainingDays;
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
					ret.Values.Add(new ObservablePoint(x, total));
					break;
				}

				ret.Values.Add(new ObservablePoint(i, amount));
			}
			
			return ret;
		}

		public static int CalcDaysFinished(bool epilogue)
		{
			var daysFinished = 0;

			var total = CalcUtil.CalcMaxForSeason(epilogue);
			var duration = TrackingData.CurrentSeasonData.Duration;
			var remainingDays = TrackingData.CurrentSeasonData.RemainingDays;
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
		public List<int> Segments { get; set; }

		public DailyData() { }
		public DailyData(double progress, int collected, int remaining, int total, int streak, List<int> segments)
		{
			(Progress, Collected, Remaining, Total, Streak, Segments) = (progress, collected, remaining, total, streak, segments);
		}
	}
}
