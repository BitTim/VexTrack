using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	public static class GraphCalc
	{
		public static LineSeries CalcIdealGraph(string sUUID)
		{
			LineSeries ret = new();

			int total = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.GetSeason(sUUID).ActiveBPLevel, TrackingDataHelper.GetSeason(sUUID).CXP).Total;
			int bufferDays = Constants.BufferDays; //TODO: Move BufferDays to settings
			int duration = TrackingDataHelper.GetDuration(sUUID);

			int initCollected = TrackingDataHelper.GetFirstHistoryEntry(sUUID).Amount;
			int initRemaining = total - initCollected;
			int totalDaily = (int)MathF.Round(initRemaining / (duration - bufferDays));

			ret.Points.Add(new DataPoint(0, initCollected));
			for (int i = 1; i < duration + 1; i++)
			{
				int value = (int)ret.Points.Last().Y + totalDaily;
				if (value > total) value = total;

				ret.Points.Add(new DataPoint(i, value));
			}

			ret.Color = OxyColors.Gray;
			ret.StrokeThickness = 2;
			return ret;
		}

		public static LineSeries CalcPerformanceGraph(string sUUID)
		{
			LineSeries ret = new();

			DateTimeOffset prevDate = DateTimeOffset.FromUnixTimeSeconds(TrackingDataHelper.GetFirstHistoryEntry(sUUID).Time).ToLocalTime().Date;
			List<int> dailyAmounts = new();

			// Construct list of daily amounts

			foreach (HistoryEntry h in TrackingDataHelper.GetSeason(sUUID).History)
			{
				DateTimeOffset currDate = DateTimeOffset.FromUnixTimeSeconds(h.Time).ToLocalTime().Date;
				if (currDate == prevDate && dailyAmounts.Count != 0)
				{
					dailyAmounts[dailyAmounts.Count - 1] += h.Amount;
					continue;
				}

				int gapSize = (currDate - prevDate).Days;
				prevDate = currDate;

				int prevValue = 0;
				if (dailyAmounts.Count != 0) prevValue = dailyAmounts.Last();

				for (int i = 1; i < gapSize; i++) dailyAmounts.Add(prevValue);
				dailyAmounts.Add(h.Amount + prevValue);
			}

			int inactiveDays = 0;
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			DateTimeOffset seasonEndDate = DateTimeOffset.Parse(TrackingDataHelper.GetSeason(sUUID).EndDate).ToLocalTime().Date;

			if (today < seasonEndDate) inactiveDays = (today - prevDate).Days;
			else inactiveDays = (seasonEndDate - prevDate).Days;
			for (int i = 0; i < inactiveDays; i++) dailyAmounts.Add(dailyAmounts.Last());

			// Translate list to datapoints

			int idx = 0;
			foreach (int amount in dailyAmounts) ret.Points.Add(new DataPoint(idx++, amount));

			ret.Color = OxyColors.Red;
			ret.StrokeThickness = 4;
			return ret;
		}

		public static List<LineSeries> CalcBattlepassLevels(string sUUID)
		{
			List<LineSeries> ret = new();

			for (int i = 0; i < Constants.BattlepassLevels + 1; i++)
			{
				LineSeries ls = new();
				byte alpha = 128;
				int val = CalcUtil.CumulativeSum(i, Constants.Level2Offset, Constants.XPPerLevel);

				ls.Points.Add(new DataPoint(0, val));
				ls.Points.Add(new DataPoint(TrackingDataHelper.GetDuration(sUUID), val));

				if (CalcUtil.CalcTotalCollected(TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP) >= val) alpha = 13;

				if (i % 5 == 0) ls.Color = OxyColor.FromAColor(alpha, OxyColors.LimeGreen);
				else ls.Color = OxyColor.FromAColor(alpha, OxyColors.LightGray);

				ret.Add(ls);
			}

			return ret;
		}
	}
}
