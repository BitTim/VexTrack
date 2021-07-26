using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	public static class GoalDataCalc
	{
		public static GoalEntryData CalcTotalGoal(string uuid, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(uuid);

			ret.Title = "Total XP";

			ret.Total = CalcUtil.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XPPerLevel);
			ret.Collected = CalcUtil.CalcTotalCollected(activeLevel, cxp);
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.StartXP = -1;

			return ret;
		}

		public static GoalEntryData CalcBattlepassGoal(string uuid, int activeLevel)
		{
			GoalEntryData ret = new(uuid);

			ret.Title = "Battlepass";

			ret.Total = Constants.BattlepassLevels;
			ret.Collected = activeLevel - 1;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = activeLevel > Constants.BattlepassLevels + Constants.EpilogueLevels ? -1 : activeLevel;
			ret.StartXP = -1;

			return ret;
		}

		public static GoalEntryData CalcLevelGoal(string uuid, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(uuid);

			int total;
			if (activeLevel - 1 <= Constants.BattlepassLevels) total = Constants.Level2Offset + Constants.XPPerLevel * activeLevel;
			else total = Constants.XPPerEpilogueLevel;

			ret.Title = "Level " + (activeLevel > Constants.BattlepassLevels + Constants.EpilogueLevels ? (Constants.BattlepassLevels + Constants.EpilogueLevels).ToString() : activeLevel.ToString());

			ret.Total = total;
			ret.Collected = cxp;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.StartXP = -1;

			return ret;
		}

		public static GoalEntryData CalcUserGoal(Goal goalData, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(goalData.UUID);

			ret.Title = goalData.Name;

			ret.Total = goalData.Total;
			ret.Collected = goalData.Collected;
			ret.Remaining = goalData.Total - goalData.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.Color = goalData.Color;

			return ret;
		}
	}

	public class GoalEntryData
	{
		public string UUID { get; set; }
		public string Title { get; set; }
		public double Progress { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }
		public string Color { get; set; }
		public int StartXP { get; set; }
		public int Active { get; set; }

		public GoalEntryData(string uuid)
		{
			UUID = uuid;
		}

		public GoalEntryData(string uuid, string title, double progress, int collected, int remaining, int total, string color, int startXP = -1, int active = -1)
		{
			(UUID, Title, Progress, Collected, Remaining, Total, Color, StartXP, Active) = (uuid, title, progress, collected, remaining, total, color, startXP, active);
		}
	}
}
