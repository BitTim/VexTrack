using System;
using System.Collections.Generic;
using System.Linq;
using VexTrack.Core.Model;

namespace VexTrack.Core.Helper;

public static class CalcHelper
{
	public static int CumulativeSum(int index, int offset, int amount)
	{
		var ret = 0;
		for (var i = 2; i <= index; i++)
		{
			ret += amount * i + offset;
		}

		return ret;
	}

	public static int CalcTotalCollected(int activeLevel, int cxp)
	{
		int collected;
		if (activeLevel - 1 <= Constants.BattlepassLevels) collected = CalcHelper.CumulativeSum(activeLevel - 1, Constants.Level2Offset, Constants.XpPerLevel) + cxp;
		else collected = CalcMaxForSeason(false) + cxp + (activeLevel - Constants.BattlepassLevels - 1) * Constants.XpPerEpilogueLevel;

		return collected;
	}
		
	public static List<int> CalcCollectedPerDay(long startTimestamp, List<HistoryEntry> history, int duration)
	{
		var dailyAmounts = new List<int>();
		history = history.OrderBy(he => he.Time).ToList();

		var historyIdx = 0;
		var startDate = TimeHelper.TimestampToDate(startTimestamp);

		for (var i = 0; i < duration + 1; i++)
		{
			var amount = 0;

			if(historyIdx < history.Count) {
				var currDate = startDate.AddDays(i).ToLocalTime();
				while(TimeHelper.TimestampToDate(history[historyIdx].Time) == currDate)
				{
					amount += history[historyIdx++].Amount;
					if (historyIdx >= history.Count) break;
				}
			}

			dailyAmounts.Add(amount);
		}

		return dailyAmounts;
	}
		
	public static int CalcDaysFinished(bool epilogue)
	{
		var daysFinished = 0;

		var total = CalcHelper.CalcMaxForSeason(epilogue);
		var duration = UserData.CurrentSeasonData.Duration;
		var remainingDays = UserData.CurrentSeasonData.RemainingDays;
		var daysPassed = duration - remainingDays;
		var totalCollected = CalcHelper.CalcTotalCollected(UserData.CurrentSeasonData.ActiveBpLevel, UserData.CurrentSeasonData.Cxp);
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

	public static int CalcProgress(double total, double collected)
	{
		if (total <= 0) return 100;

		var ret = (int)Math.Floor(collected / total * 100);
		return ret;
	}

	public static int CalcMaxForLevel(int level)
	{
		return level <= Constants.BattlepassLevels ? Constants.Level2Offset + level * Constants.XpPerLevel : Constants.XpPerEpilogueLevel;
	}

	public static int CalcMaxForTier(int tier)
	{
		if (tier < 1 || tier > Constants.TierTotals.Count) return 0;
		return Constants.TierTotals[tier - 1];
	}

	public static int CalcMaxForSeason(bool epilogue)
	{
		var sum = CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XpPerLevel);
		if (epilogue) sum += Constants.EpilogueLevels * Constants.XpPerEpilogueLevel;

		return sum;
	}

	public static int CalcAverage(int collected, int duration, int remainingDays)
	{
		var daysPassed = duration - remainingDays;
		return (int)MathF.Round((float)collected / (daysPassed + 1));
	}

	public static List<int> CalcSeasonSegments()
	{
		var seasonMaxEpilogue = CalcMaxForSeason(true);
		var seasonMaxGeneral = CalcMaxForSeason(false);

		return new List<int> { seasonMaxGeneral, seasonMaxEpilogue - seasonMaxGeneral };
	}

		
		
	// ================================================================
	//  Segments / Stops
	// ================================================================
		
	private static decimal CalcEqualizedOffset(decimal index, int total, int nSegments)
	{
		var factor = -((decimal)total / 100);
		return Math.Round(factor * index + (decimal)nSegments / 2 * -factor - factor / 2, 4);
	}
		
	public static List<decimal> CalcVisualStops(List<int> segments, bool proportional)
	{
		var total = segments.Sum();
		var stops = new List<decimal>();
		decimal prevVal = 0;

		if (total <= 0 || segments.Count <= 0) return stops;

		for (var i = 0; i < segments.Count; i++)
		{
			var val = proportional
				? (segments[i] + CalcEqualizedOffset(i + 1, total, segments.Count)) / total
				: 1 / (decimal)segments.Count;

			prevVal += val;
			stops.Add(prevVal);
		}

		return stops;
	}
		
	public static List<decimal> CalcLogicalStops(List<int> segments, bool proportional)
	{
		var total = segments.Sum();
		var stops = new List<decimal>();
		decimal prevVal = 0;

		if (total <= 0 || segments.Count <= 0) return stops;
		
		foreach (var val in segments.Select(segment => proportional ? (decimal)segment  / total : 1 / (decimal)segments.Count))
		{
			prevVal += val;
			stops.Add(prevVal);
		}

		return stops;
	}
}