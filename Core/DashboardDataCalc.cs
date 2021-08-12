using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	public static class DashboardDataCalc
	{
		public static DailyData CalcDailyData(bool epilogue)
		{
			DailyData ret = new();

			GoalEntryData totalData = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue);
			int bufferDays = SettingsHelper.Data.BufferDays;
			int idealRemainingDays = TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID) - bufferDays;

			if (idealRemainingDays > -bufferDays && idealRemainingDays <= 0) idealRemainingDays = 1;
			else if (idealRemainingDays <= -bufferDays && idealRemainingDays <= 0) TrackingDataHelper.CreateSeasonInitPopup();

			int dailyColected = 0;
			foreach (HistoryEntry h in TrackingDataHelper.CurrentSeasonData.History.GetRange(1, TrackingDataHelper.CurrentSeasonData.History.Count - 1))
			{
				if (DateTimeOffset.FromUnixTimeSeconds(h.Time).ToLocalTime().Date != DateTimeOffset.Now.ToLocalTime().Date) continue;
				dailyColected += h.Amount;
			}

			int total = (int)MathF.Round(totalData.Remaining / idealRemainingDays);
			if(total <= 0) total = 0;

			ret.Total = total;
			ret.Collected = dailyColected;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);

			return ret;
		}

		public static LineSeries CalcDailyIdeal(LineSeries performance, bool epilogue)
		{
			LineSeries ret = new();

			List<int> amounts = new();
			int total = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue).Total;
			int bufferDays = SettingsHelper.Data.BufferDays;
			int effectiveRemaining = TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID) - bufferDays + 1;
			if (effectiveRemaining <= 0) effectiveRemaining = 1;

			int remainingDays = TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID);
			int duration = TrackingDataHelper.GetDuration(TrackingDataHelper.CurrentSeasonUUID);
			int graphOffset = duration - remainingDays - 1;

			int startOffset = 0;
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			DateTimeOffset lastEntryDate = DateTimeOffset.FromUnixTimeSeconds(TrackingDataHelper.GetLastHistoryEntry(TrackingDataHelper.CurrentSeasonUUID).Time).ToLocalTime().Date;
			if (lastEntryDate == today) startOffset = 1;

			int previousAmount = (int)performance.Points.First().Y;
			if (performance.Points.Count > 1) previousAmount = (int)performance.Points[performance.Points.Count - 1 - startOffset].Y;
			int dailyTotal = (int)MathF.Round((total - previousAmount) / effectiveRemaining);
			if (dailyTotal <= 0) return ret;

			amounts.Add(previousAmount);

			for(int i = 1; i < remainingDays + 2; i++)
			{
				int amount = amounts[i - 1] + dailyTotal;
				if (amount > total) amount = total;
				amounts.Add(amount);
			}

			int idx = 0;
			foreach (int amount in amounts) ret.Points.Add(new DataPoint(graphOffset + idx++, amount));

			ret.Color = OxyColors.SteelBlue;
			ret.StrokeThickness = 2;
			ret.LineStyle = LineStyle.Dash;
			ret.Title = "Daily Ideal";
			return ret;
		}

		public static LineSeries CalcAverageGraph(LineSeries performance, bool epilogue)
		{
			LineSeries ret = new();

			int total = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue).Total;
			int duration = TrackingDataHelper.GetDuration(TrackingDataHelper.CurrentSeasonUUID);
			int daysPassed = duration - TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID);
			int totalCollected = CalcUtil.CalcTotalCollected(TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP);
			int average = (int)MathF.Round(totalCollected / (daysPassed + 1));
			average = 30000;

			if (totalCollected >= total) return ret;

			ret.Points.Add(new DataPoint(daysPassed - 1, performance.Points[daysPassed - 1].Y));

			for (int i = daysPassed; i < duration + 1; i++)
			{
				int amount = (int)ret.Points.Last().Y + average;
				if (amount > total)
				{
					double x = ((double)total - (double)performance.Points[daysPassed - 1].Y) / (double)average + daysPassed - 1;
					ret.Points.Add(new DataPoint(x, total));
					break;
				}

				ret.Points.Add(new DataPoint(i, amount));
			}

			ret.Color = OxyColors.PaleVioletRed;
			ret.StrokeThickness = 2;
			ret.LineStyle = LineStyle.Dot;
			ret.Title = "Average";
			return ret;
		}

		public static LineSeries CalcGraphPoint(LineSeries series, OxyColor color)
		{
			LineSeries ret = new();

			int remainingDays = TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID);
			int duration = TrackingDataHelper.GetDuration(TrackingDataHelper.CurrentSeasonUUID);
			int xPos = duration - remainingDays;

			ret.Points.Add(new DataPoint(xPos, series.Points.Find(p => p.X == xPos).Y));

			ret.MarkerType = MarkerType.Circle;
			ret.MarkerFill = color;
			ret.MarkerSize = 4;

			return ret;
		}

		public static int CalcDaysFinished(bool epilogue)
		{
			int daysFinished = 0;

			int total = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue).Total;
			int duration = TrackingDataHelper.GetDuration(TrackingDataHelper.CurrentSeasonUUID);
			int remainingDays = TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID);
			int daysPassed = duration - remainingDays;
			int totalCollected = CalcUtil.CalcTotalCollected(TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP);
			int average = (int)MathF.Round(totalCollected / (daysPassed + 1));

			int val = totalCollected;
			for (int i = 0; i < remainingDays + 1; i++)
			{
				val += average;
				daysFinished = i;

				if (val >= total) break;
			}

			if (daysFinished > remainingDays) daysFinished = -1;
			return daysFinished;
		}
	}

	public class DailyData
	{
		public double Progress { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }

		public DailyData() { }
		public DailyData(double progress, int collected, int remaining, int total)
		{
			(Progress, Collected, Remaining, Total) = (progress, collected, remaining, total);
		}
	}
}
