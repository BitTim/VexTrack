﻿using OxyPlot;
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
		public static DailyData CalcDailyData()
		{
			DailyData ret = new();

			GoalEntryData totalData = GoalDataCalc.CalcTotalGoal("", TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP);
			int bufferDays = Constants.BufferDays; //TODO: Move BufferDays to settings
			int idealRemainingDays = TrackingDataHelper.GetRemainingDays(TrackingDataHelper.CurrentSeasonUUID) - bufferDays;

			if (idealRemainingDays >= -bufferDays && idealRemainingDays <= 0) idealRemainingDays = 1;
			else if (idealRemainingDays < -bufferDays) _ = 0; //TODO: Insert trigger for creation of new season here

			int dailyColected = 0;
			foreach(HistoryEntry h in TrackingDataHelper.CurrentSeasonData.History)
			{
				if (DateTimeOffset.FromUnixTimeSeconds(h.Time).ToLocalTime().Date != DateTimeOffset.Now.ToLocalTime().Date) continue;
				dailyColected += h.Amount;
			}

			ret.Total = (int)MathF.Round(totalData.Remaining / idealRemainingDays);
			ret.Collected = dailyColected;
			ret.Remaining = ret.Total - ret.Collected;
			ret.Progress = CalcUtil.CalcProgress(ret.Total, ret.Collected);

			return ret;
		}

		public static LineSeries CalcDailyIdeal()
		{
			LineSeries ret = new();



			return ret;
		}

		/*
		yAxisDailyIdeal = []

		dateDelta = seasonEndDate - date.today()
		remainingDays = dateDelta.days
		dayDelta = duration - remainingDays

		if date.fromtimestamp(data["seasons"][seasonIndex.get()]["xpHistory"][len(data["seasons"][seasonIndex.get()]["xpHistory"]) - 1]["time"]) == date.today(): offset = 1
		else: offset = 0

		totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, epilogue, seasonIndex.get())

		if remainingDays >= 0 and remainingDays < duration:
			divisor = remainingDays - settings["bufferDays"] + 1
			if divisor <= 0: divisor = 1
    
			dailyTotal = round((totalXPTotal - yAxisYou[index - offset]) / divisor)
			yAxisDailyIdeal.append(yAxisYou[index - offset])

			for i in range(1, remainingDays + 2):
				yAxisDailyIdeal.append(yAxisDailyIdeal[i - 1] + dailyTotal)
				if yAxisDailyIdeal[i] > totalXP: yAxisDailyIdeal[i] = totalXP

			plot.plot(timeAxis[dayDelta - 1:], yAxisDailyIdeal, color='skyblue', label='Daily Ideal', alpha=1, linestyle="--")
    
		plot.plot(timeAxis[:len(yAxisYou)], yAxisYou, color='red', label='You', linewidth=3)
		 */
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
