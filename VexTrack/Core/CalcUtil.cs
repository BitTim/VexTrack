using System;

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
			else collected = CalcUtil.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XpPerLevel) + cxp + (activeLevel - Constants.BattlepassLevels - 1) * Constants.XpPerEpilogueLevel;

			return collected;
		}

		public static double CalcProgress(double total, double collected)
		{
			if (total <= 0) return 100;

			var ret = Math.Floor(collected / total * 100);
			return ret;
		}

		public static int CalcMaxForLevel(int level)
		{
			return Constants.Level2Offset + (level * Constants.XpPerLevel);
		}

		public static int CalcMaxForTier(int tier)
		{
			switch (tier)
			{
				case 1: return 20000;
				case 2: return 30000;
				case 3: return 40000;
				case 4: return 50000;
				case 5: return 60000;
				case 6: return 75000;
				case 7: return 100000;
				case 8: return 150000;
				case 9: return 200000;
				case 10: return 250000;
				default: return 0;
			}
		}
	}
}
