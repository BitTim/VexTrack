using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace VexTrack.Core
{
	public static class GoalDataCalc
	{
		/*public static GoalEntryData CalcTotalGoal(string uuid, int activeLevel, int cxp, bool epilogue)
		{
			GoalEntryData ret = new(uuid);

			ret.Title = "Total XP";

			ret.Total = CalcUtil.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XpPerLevel);
			if (epilogue) ret.Total += Constants.EpilogueLevels * Constants.XpPerEpilogueLevel;

			ret.Collected = CalcUtil.CalcTotalCollected(activeLevel, cxp);
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.StartXp = -1;
			ret.Paused = false;
			if (ret.Progress >= 100) ret.CompletionStatus = "Done";

			return ret;
		}

		public static GoalEntryData CalcBattlepassGoal(string uuid, int activeLevel, bool epilogue)
		{
			GoalEntryData ret = new(uuid);

			ret.Title = "Battlepass Levels";

			ret.Total = Constants.BattlepassLevels;
			if (epilogue) ret.Total += Constants.EpilogueLevels;

			ret.Collected = activeLevel - 1;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = activeLevel > Constants.BattlepassLevels + Constants.EpilogueLevels ? -1 : activeLevel;
			ret.StartXp = -1;
			ret.Paused = false;
			if (ret.Progress >= 100) ret.CompletionStatus = "Done";

			return ret;
		}

		public static GoalEntryData CalcLevelGoal(string uuid, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(uuid);

			int total;
			if (activeLevel - 1 < Constants.BattlepassLevels) total = Constants.Level2Offset + Constants.XpPerLevel * activeLevel;
			else if (activeLevel > Constants.BattlepassLevels + Constants.EpilogueLevels) total = 0;
			else total = Constants.XpPerEpilogueLevel;

			if (activeLevel > Constants.BattlepassLevels + Constants.EpilogueLevels) ret.Title = "Post Completion";
			else ret.Title = "Level " + activeLevel.ToString();

			ret.Total = total;
			ret.Collected = cxp;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.StartXp = -1;
			ret.Paused = false;
			if (ret.Progress >= 100) ret.CompletionStatus = "Done";

			return ret;
		}*/

		public static GoalEntryData CalcGoal(string groupUuid, Goal goalData)
		{
			GoalEntryData ret = new(goalData.Uuid)
			{
				Title = goalData.Name,
				GroupUuid = groupUuid,
				//ret.DepUuid = goalData.Dependency;
				Total = goalData.Total,
				Collected = goalData.Collected,
				Remaining = goalData.Total - goalData.Collected
			};

			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			//ret.Active = -1;
			//ret.Color = goalData.Color;
			//ret.Paused = goalData.Paused;

			//if (ret.DepUuid != null && ret.DepUuid != "") ret.ActivityStatus = "Linked";

			// if (ret.Paused) ret.CompletionStatus = "Paused";
			// if (ret.Progress >= 100) ret.CompletionStatus = "Done";

			return ret;
		}

		/*public static bool CheckPaused(Contract gg, Goal g)
		{
			if (g.Collected >= g.Total) return false; // If done, it cannot be paused

			if (g.Dependency != "")
			{
				if (g.Paused) return true; // Break chain if something other than the root is paused

				int index = gg.Goals.FindIndex(x => x.Uuid == g.Dependency);
				if (index < 0) return true; // Has invalid Dependency = Dont show

				Goal nextG = gg.Goals[index];
				return CheckPaused(gg, nextG);
			}

			return g.Paused;
		}*/

		/*public static (List<LineSeries>, List<TextAnnotation>) CalcGraphGoals(string sUuid)
		{
			List<LineSeries> lsRet = new();
			List<TextAnnotation> taRet = new();

			foreach (var contract in TrackingDataHelper.Data.Contracts)
			{
				foreach (var goal in contract.Goals)
				{
					LineSeries ls = new();
					TextAnnotation ta = new();
					byte alpha = 128;

					GoalEntryData ge = CalcUserGoal(contract.Uuid, goal);
					int totalCollected = CalcUtil.CalcTotalCollected(TrackingDataHelper.CurrentSeasonData.ActiveBpLevel, TrackingDataHelper.CurrentSeasonData.Cxp);
					int val = totalCollected - ge.Collected + ge.Total;

					if (val <= 0) continue;
					if (CheckPaused(contract, goal)) continue;

					ls.Points.Add(new DataPoint(0, val));
					ls.Points.Add(new DataPoint(TrackingDataHelper.GetDuration(sUuid), val));

					ta.Text = ge.Title;
					ta.TextPosition = new DataPoint(TrackingDataHelper.GetDuration(sUuid) / 2, val);
					ta.StrokeThickness = 0;

					if (totalCollected >= val) alpha = 13;

					LinearGradientBrush accent = (LinearGradientBrush)Application.Current.FindResource("Accent");

					if (ge.Color == "") ge.Color = accent.GradientStops[0].Color.ToString();
					ls.Color = OxyColor.FromAColor(alpha, OxyColor.Parse(ge.Color));
					ta.TextColor = OxyColor.FromAColor(alpha, OxyColor.Parse(ge.Color));

					lsRet.Add(ls);
					taRet.Add(ta);
				}
			}

			return (lsRet, taRet);
		}*/
	}

	public class GoalEntryData
	{
		public string Uuid { get; set; }
		public string GroupUuid { get; set; }
		//public string DepUuid { get; set; }
		public string Title { get; set; }
		public double Progress { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }
		//public string Color { get; set; }
		//public string CompletionStatus { get; set; }
		//public string ActivityStatus { get; set; }
		//public int StartXp { get; set; }
		//public int Active { get; set; }
		//public bool Paused { get; set; }

		public GoalEntryData(string uuid)
		{
			Uuid = uuid;
		}

		public GoalEntryData(string uuid, string groupUuid, string title, double progress, int collected, int remaining, int total)
		{
			Uuid = uuid;
			GroupUuid = groupUuid;
			//DepUuid = depUuid;
			Title = title;
			Progress = progress;
			Collected = collected;
			Remaining = remaining;
			Total = total;
			//Color = color;
			//CompletionStatus = completionStatus;
			//ActivityStatus = activityStatus;
			//Paused = paused;

			//StartXp = startXp;
			//Active = active;
		}
	}

	public class ContractData
	{
		public string Uuid { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public bool Paused { get; set; }
		public List<GoalEntryData> Goals { get; set; }

		public ContractData(string uuid, string name, string color, bool paused, List<GoalEntryData> goals)
		{
			Uuid = uuid;
			Name = name;
			Color = color;
			Paused = paused;
			Goals = goals;
		}
	}
}
