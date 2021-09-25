using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VexTrack.Core
{
	public static class GoalDataCalc
	{
		public static GoalEntryData CalcTotalGoal(string uuid, int activeLevel, int cxp, bool epilogue)
		{
			GoalEntryData ret = new(uuid);

			ret.Title = "Total XP";

			ret.Total = CalcUtil.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XPPerLevel);
			if(epilogue) ret.Total += Constants.EpilogueLevels * Constants.XPPerEpilogueLevel;

			ret.Collected = CalcUtil.CalcTotalCollected(activeLevel, cxp);
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.StartXP = -1;
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
			ret.StartXP = -1;
			ret.Paused = false;
			if (ret.Progress >= 100) ret.CompletionStatus = "Done";

			return ret;
		}

		public static GoalEntryData CalcLevelGoal(string uuid, int activeLevel, int cxp)
		{
			GoalEntryData ret = new(uuid);

			int total;
			if (activeLevel - 1 < Constants.BattlepassLevels) total = Constants.Level2Offset + Constants.XPPerLevel * activeLevel;
			else if (activeLevel > Constants.BattlepassLevels + Constants.EpilogueLevels) total = 0;
			else total = Constants.XPPerEpilogueLevel;

			if (activeLevel > Constants.BattlepassLevels + Constants.EpilogueLevels) ret.Title = "Post Completion";
			else ret.Title = "Level " + activeLevel.ToString();

			ret.Total = total;
			ret.Collected = cxp;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.StartXP = -1;
			ret.Paused = false;
			if (ret.Progress >= 100) ret.CompletionStatus = "Done";

			return ret;
		}

		public static GoalEntryData CalcUserGoal(string groupUUID, Goal goalData)
		{
			GoalEntryData ret = new(goalData.UUID);

			ret.Title = goalData.Name;

			ret.GroupUUID = groupUUID;
			ret.DepUUID = goalData.Dependency;
			ret.Total = goalData.Total;
			ret.Collected = goalData.Collected;
			ret.Remaining = goalData.Total - goalData.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);
			ret.Active = -1;
			ret.Color = goalData.Color;
			ret.Paused = goalData.Paused;

			if (ret.DepUUID != null && ret.DepUUID != "") ret.ActivityStatus = "Linked";

			if (ret.Paused) ret.CompletionStatus = "Paused";
			if (ret.Progress >= 100) ret.CompletionStatus = "Done";

			return ret;
		}

		public static (List<LineSeries>, List<TextAnnotation>) CalcGraphGoals(string sUUID)
		{
			List<LineSeries> lsret = new();
			List<TextAnnotation> taret = new();

			foreach (GoalGroup gg in TrackingDataHelper.Data.Goals)
			{
				foreach (Goal g in gg.Goals)
				{
					LineSeries ls = new();
					TextAnnotation ta = new();
					byte alpha = 128;

					GoalEntryData ge = CalcUserGoal(gg.UUID, g);
					int totalCollected = CalcUtil.CalcTotalCollected(TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP);
					int val = totalCollected - ge.Collected + ge.Total;

					if (val <= 0) continue;

					ls.Points.Add(new DataPoint(0, val));
					ls.Points.Add(new DataPoint(TrackingDataHelper.GetDuration(sUUID), val));

					ta.Text = ge.Title;
					ta.TextPosition = new DataPoint(TrackingDataHelper.GetDuration(sUUID) / 2, val);
					ta.StrokeThickness = 0;

					if (totalCollected >= val) alpha = 13;

					LinearGradientBrush accent = (LinearGradientBrush)Application.Current.FindResource("Accent");

					if (ge.Color == "") ge.Color = accent.GradientStops[0].Color.ToString();
					ls.Color = OxyColor.FromAColor(alpha, OxyColor.Parse(ge.Color));
					ta.TextColor = OxyColor.FromAColor(alpha, OxyColor.Parse(ge.Color));

					lsret.Add(ls);
					taret.Add(ta);
				}
			}

			return (lsret, taret);
		}
	}

	public class GoalEntryData
	{
		public string UUID { get; set; }
		public string GroupUUID { get; set; }
		public string DepUUID { get; set; }
		public string Title { get; set; }
		public double Progress { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }
		public string Color { get; set; }
		public string CompletionStatus { get; set; }
		public string ActivityStatus { get; set; }
		public int StartXP { get; set; }
		public int Active { get; set; }
		public bool Paused { get; set; }

		public GoalEntryData(string uuid)
		{
			UUID = uuid;
		}

		public GoalEntryData(string uuid, string groupUUID, string depUUID, string title, double progress, int collected, int remaining, int total, string color, string completionStatus, string activityStatus, bool paused, int startXP = -1, int active = -1)
		{
			UUID = uuid;
			GroupUUID = groupUUID;
			DepUUID = depUUID;
			Title = title;
			Progress = progress;
			Collected = collected;
			Remaining = remaining;
			Total = total;
			Color = color;
			CompletionStatus = completionStatus;
			ActivityStatus = activityStatus;
			Paused = paused;

			StartXP = startXP;
			Active = active;
		}
	}

	public class GoalGroupData
	{
		public string UUID { get; set; }
		public string Name { get; set; }
		public List<GoalEntryData> Goals { get; set; }

		public GoalGroupData(string uuid, string name, List<GoalEntryData> goals)
		{
			UUID = uuid;
			Name = name;
			Goals = goals;
		}
	}
}
