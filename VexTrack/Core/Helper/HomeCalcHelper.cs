using System;
using System.Collections.Generic;
using VexTrack.Core.Model;

namespace VexTrack.Core.Helper;

public static class HomeCalcHelper
{
	public static DailyData CalcDailyData()
	{
		DailyData ret = new();

		var currentSeasonData = UserData.CurrentSeasonData;
		if (currentSeasonData == null) return ret;

		var idealRemainingDays = currentSeasonData.RemainingDays - currentSeasonData.BufferDays + 1;
		if (idealRemainingDays > -currentSeasonData.BufferDays && idealRemainingDays <= 0) idealRemainingDays = 1;

		var today = TimeHelper.TodayDate;
		var dayIndex = (today - TimeHelper.TimestampToDate(currentSeasonData.StartTimestamp)).Days;
		if (dayIndex < 0 || dayIndex >= currentSeasonData.Duration) return ret;

		var collectedPerDay = HistoryHelper.CalcDailyCollectedFromSeason(UserData.CurrentSeasonData.Uuid);
		var totalToday = (int)Math.Ceiling((currentSeasonData.Remaining + collectedPerDay[dayIndex]) /
		                                   (double)idealRemainingDays);
		var totalTodayMin =
			(int)Math.Ceiling(((double)currentSeasonData.RemainingMin + collectedPerDay[dayIndex]) / idealRemainingDays);
		if (totalToday <= 0) totalToday = 0;
		if (totalTodayMin <= 0) totalTodayMin = 0;

		var segments = new List<int> { totalTodayMin, totalToday - totalTodayMin };

		ret.Total = totalToday;
		ret.Collected = collectedPerDay[dayIndex];
		ret.Remaining = ret.Total - ret.Collected;
		ret.Progress = CalcHelper.CalcProgress(ret.Total, ret.Collected);
		ret.Segments = segments;

		if ((today - TimeHelper.TimestampToDate(UserData.LastStreakUpdateTimestamp)).Days > 1)
			UserData.Streak = 0;
		else if (collectedPerDay[dayIndex] > 0 &&
		         !TimeHelper.TimestampToDate(UserData.LastStreakUpdateTimestamp)
			         .Equals(today)) // When streak was not fulfilled today, but is now fulfilled, update
		{
			UserData.Streak++;
			UserData.LastStreakUpdateTimestamp = TimeHelper.TodayTimestamp;
		}
		else if (collectedPerDay[dayIndex] <= 0 &&
		         TimeHelper.TimestampToDate(UserData.LastStreakUpdateTimestamp)
			         .Equals(today)) // If streak was fulfilled today, but is no longer, update
		{
			UserData.Streak--;
			UserData.LastStreakUpdateTimestamp = today.AddDays(-1).ToUnixTimeSeconds();
		}

		ret.Streak = UserData.Streak;
		return ret;
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