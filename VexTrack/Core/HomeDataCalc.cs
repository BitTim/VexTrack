using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace VexTrack.Core
{
	public static class HomeDataCalc
	{
		public static DailyData CalcDailyData()
		{
			DailyData ret = new();
			
			var currentSeasonData = TrackingData.CurrentSeasonData;
			
			var idealRemainingDays = currentSeasonData.RemainingDays - currentSeasonData.BufferDays + 1;
			if (idealRemainingDays > -currentSeasonData.BufferDays && idealRemainingDays <= 0) idealRemainingDays = 1;

			var today = DateTimeOffset.Now.ToLocalTime().Date;
			var dayIndex = (today - DateTimeOffset.FromUnixTimeSeconds(currentSeasonData.StartDate)).Days;
			if (dayIndex < 0 || dayIndex >= currentSeasonData.Duration) return ret;

			var collectedPerDay = CalcUtil.CalcCollectedPerDay(currentSeasonData.StartDate, currentSeasonData.History, currentSeasonData.Duration);
			var totalToday = (int)Math.Ceiling((currentSeasonData.Remaining + collectedPerDay[dayIndex]) / (double)idealRemainingDays);
			var totalTodayMin = (int)Math.Ceiling((currentSeasonData.RemainingMin + collectedPerDay[dayIndex]) / idealRemainingDays);
			if (totalToday <= 0) totalToday = 0;
			if (totalTodayMin <= 0) totalTodayMin = 0;

			var segments = new List<int> { totalTodayMin, totalToday - totalTodayMin };
			
			ret.Total = totalToday;
			ret.Collected = collectedPerDay[dayIndex];
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Segments = segments;

			if((today - DateTimeOffset.FromUnixTimeSeconds(TrackingData.LastStreakUpdateTimestamp)).Days > 1) TrackingData.Streak = 0;
			else if (collectedPerDay[dayIndex] > 0 && !DateTimeOffset.FromUnixTimeSeconds(TrackingData.LastStreakUpdateTimestamp).Equals(today)) // When streak was not fulfilled today, but is now fulfilled, update
			{
				TrackingData.Streak++;
				TrackingData.LastStreakUpdateTimestamp = ((DateTimeOffset)today).ToUnixTimeSeconds();
			}
			else if(collectedPerDay[dayIndex] <= 0 && DateTimeOffset.FromUnixTimeSeconds(TrackingData.LastStreakUpdateTimestamp).Equals(today)) // If streak was fulfilled today, but is no longer, update
			{
				TrackingData.Streak--;
				TrackingData.LastStreakUpdateTimestamp = ((DateTimeOffset)today.AddDays(-1)).ToUnixTimeSeconds();
			}

			ret.Streak = TrackingData.Streak;
			return ret;
		}

		public static int CalcDaysFinished(bool epilogue) // TODO: Move to CalcUtil
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
