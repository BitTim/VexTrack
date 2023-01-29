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
			return tier switch
			{
				1 => 20000,
				2 => 30000,
				3 => 40000,
				4 => 50000,
				5 => 60000,
				6 => 75000,
				7 => 100000,
				8 => 150000,
				9 => 200000,
				10 => 250000,
				_ => 0
			};
		}

		public static int CalcMaxForSeason(bool epilogue)
		{
			var sum = CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XpPerLevel);
			if (epilogue) sum += Constants.EpilogueLevels * Constants.XpPerEpilogueLevel;

			return sum;
		}

		// TODO: Convert to double to decimal
		public static List<double> CalcStops(List<int> segments, bool proportional)
		{
			var total = segments.Sum();
			var stops = new List<double>();
			var prevVal = 0.0;
			
			foreach (var rawVal in segments.Select(segment => proportional ? Math.Round(segment * 100.0 / total, 2) : Math.Round(100.0 / segments.Count, 2)))
			{
				var val = Math.Round(rawVal / 100, 2);
				stops.Add(prevVal + val);
				prevVal += val;
			}

			return stops;
		}
	}
}
