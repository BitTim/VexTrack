using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VexTrack.Core
{
	public static class CalcUtil
	{
		public static int CumulativeSum(int index, int offset, int amount)
		{
			var ret = 0;
			for (var i = 2; i <= index; i++)
			{
				ret += (amount * i) + offset;
			}

			return ret;
		}

		public static int CalcTotalCollected(int activeLevel, int cxp)
		{
			int collected;
			if (activeLevel - 1 <= Constants.BattlepassLevels) collected = CalcUtil.CumulativeSum(activeLevel - 1, Constants.Level2Offset, Constants.XpPerLevel) + cxp;
			else collected = CalcMaxForSeason(false) + cxp + (activeLevel - Constants.BattlepassLevels - 1) * Constants.XpPerEpilogueLevel;

			return collected;
		}
		
		public static List<int> CalcCollectedPerDay(List<HistoryEntry> history, int duration)
		{
			var dailyAmounts = new List<int>();

			var historyIdx = 0;
			var startDate = DateTimeOffset.FromUnixTimeSeconds(history.First().Time).ToLocalTime().Date; // TODO: Change with #69

			for (var i = 0; i < duration + 1; i++)
			{
				var amount = 0;

				if(historyIdx < history.Count) {
					var currDate = startDate.AddDays(i).ToLocalTime().Date;
					while (DateTimeOffset.FromUnixTimeSeconds(history[historyIdx].Time).ToLocalTime().Date == currDate)
					{
						amount += history[historyIdx++].Amount;
						if (historyIdx >= history.Count) break;
					}
				}

				dailyAmounts.Add(amount);
			}

			return dailyAmounts;
		}

		public static int CalcProgress(double total, double collected)
		{
			if (total <= 0) return 100;

			var ret = (int)Math.Floor(collected / total * 100);
			return ret;
		}

		public static int CalcMaxForLevel(int level)
		{
			return Constants.Level2Offset + (level * Constants.XpPerLevel);
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

		public static int CalcAverage(int activeLevel, int cxp, int duration, int remainingDays)
		{
			var totalCollected = CalcTotalCollected(activeLevel, cxp);
			var daysPassed = duration - remainingDays;
			return (int)MathF.Round((float)totalCollected / (daysPassed + 1));
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

			foreach (var val in segments.Select(segment => proportional ? (decimal)segment  / total : 1 / (decimal)segments.Count))
			{
				prevVal += val;
				stops.Add(prevVal);
			}

			return stops;
		}
	}
}
