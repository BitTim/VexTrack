using System;
using System.Collections.Generic;
using System.Linq;
using VexTrack.Core.Model;
using VexTrack.Core.Model.Game.Templates;

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

	public static int CalcTotalCollected(int activeLevel, int cxp, List<GoalTemplate> goals)
	{
		if (activeLevel > goals.Count) return ApiData.ActiveSeasonTemplate.XpTotal + cxp;
		return ApiData.ActiveSeasonTemplate.Goals.GetRange(0, activeLevel - 1 < 2 ? 2 : activeLevel - 1).Sum(g => g.XpTotal) + cxp;
	}
		
	public static int CalcDaysFinished(string seasonUuid)
	{
		var seasonData = UserData.Seasons.FirstOrDefault(s => s.Uuid == seasonUuid);
		if (seasonData == null) return -1;
		
		var daysFinished = 0;
		if (seasonData.Average <= 0) return -1;

		var val = seasonData.Collected;
		while (val < seasonData.Total)
		{
			val += seasonData.Average;
			daysFinished++;
		}

		return daysFinished;
	}

	public static int CalcProgress(double total, double collected)
	{
		if (total <= 0) return 100;

		var ret = (int)Math.Floor(collected / total * 100);
		return ret;
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
		return (int)MathF.Round((float)collected / daysPassed);
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