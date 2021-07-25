using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	public static class GoalDataCalc
	{
		public static GoalEntryData CalcTotalGoal(int index, string uuid, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(index, uuid);

			ret.Title = "Total XP";

			ret.Total = CalcUtil.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XPPerLevel);
			ret.Collected = CalcUtil.CalcTotalCollected(activeLevel, cxp);
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);

			return ret;
		}

		public static GoalEntryData CalcBattlepassGoal(int index, string uuid, int activeLevel)
		{
			GoalEntryData ret = new(index, uuid);

			ret.Title = "Battlepass";

			ret.Total = Constants.BattlepassLevels;
			ret.Collected = activeLevel - 1;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = activeLevel;

			return ret;
		}

		public static GoalEntryData CalcLevelGoal(int index, string uuid, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(index, uuid);

			int total;
			if (activeLevel - 1 <= Constants.BattlepassLevels) total = Constants.Level2Offset + Constants.XPPerLevel * activeLevel;
			else total = Constants.XPPerEpilogueLevel;

			ret.Title = "Active Level";

			ret.Total = total;
			ret.Collected = cxp;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);

			return ret;
		}

		public static GoalEntryData CalcUserGoal(int index, Goal goalData, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(index, goalData.UUID);

			int totalCollected = CalcUtil.CalcTotalCollected(activeLevel, cxp);

			ret.Title = goalData.Name;

			ret.Total = totalCollected + goalData.Remaining - goalData.StartXP;
			ret.Collected = totalCollected - goalData.StartXP;
			ret.Remaining = goalData.Remaining;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Color = goalData.Color;

			return ret;
		}
	}

	public class GoalEntryData
	{
		public int Index { get; set; }
		public string UUID { get; set; }
		public string Title { get; set; }
		public double Progress { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }
		public string Color { get; set; }
		public int Active { get; set; }

		public GoalEntryData(int index, string uuid)
		{
			(Index, UUID) = (index, uuid);
		}

		public GoalEntryData(int index, string uuid, string title, double progress, int collected, int remaining, int total, string color, int active = -1)
		{
			(Index, UUID, Title, Progress, Collected, Remaining, Total, Color, Active) = (index, uuid, title, progress, collected, remaining, total, color, active);
		}
	}
}
