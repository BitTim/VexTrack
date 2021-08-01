using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	public static class SeasonDataCalc
	{
		public static SeasonEntryData CalcSeason(Season seasonData, bool epilogue)
		{
			SeasonEntryData ret = new(seasonData.UUID);
			GoalEntryData totalData = GoalDataCalc.CalcTotalGoal(ret.UUID, seasonData.ActiveBPLevel, seasonData.CXP, epilogue);

			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			DateTimeOffset seasonEndDate = DateTimeOffset.Parse(seasonData.EndDate).ToLocalTime().Date;
			bool ended = false;
			if (today >= seasonEndDate) ended = true;

			int remainingDays = TrackingDataHelper.GetRemainingDays(ret.UUID);
			int daysPassed = TrackingDataHelper.GetDuration(ret.UUID) - remainingDays;
			int average = (int)MathF.Round(totalData.Collected / daysPassed);

			int currentDayAmount = 0;
			int strongestAmount = 0;
			int weakestAmount = -1;
			DateTimeOffset strongestDate = new();
			DateTimeOffset weakestDate = new();
			DateTimeOffset prevDate = DateTimeOffset.FromUnixTimeSeconds(seasonData.History.First().Time).ToLocalTime().Date;

			bool ignoreInactiveDays = Constants.IgnoreInactiveDays; //TODO: Move to settings

			foreach(HistoryEntry he in seasonData.History)
			{
				DateTimeOffset currDate = DateTimeOffset.FromUnixTimeSeconds(he.Time).ToLocalTime().Date;
				if (currDate == prevDate)
				{
					currentDayAmount += he.Amount;
					continue;
				}

				(strongestAmount, weakestAmount, strongestDate, weakestDate) = EvaluateAmounts(currentDayAmount, strongestAmount, weakestAmount, prevDate, strongestDate, weakestDate);

				prevDate = currDate;
				currentDayAmount = 0;
				currentDayAmount += he.Amount;

				if (!ignoreInactiveDays)
				{
					int gapSize = (currDate - prevDate).Days;
					for (int i = 1; i < gapSize; i++) (strongestAmount, weakestAmount, strongestDate, weakestDate) = EvaluateAmounts(0, strongestAmount, weakestAmount, prevDate.AddDays(1), strongestDate, weakestDate);
				}
			}

			ret.Title = seasonData.Name;

			ret.Progress = totalData.Progress;
			ret.Ended = ended;
			ret.Average = average;
			ret.RemainingDays = remainingDays;
			ret.StrongestAmount = strongestAmount;
			ret.WeakestAmount = weakestAmount;
			ret.StrongestDate = strongestDate;
			ret.WeakestDate = weakestDate;
			ret.History = seasonData.History;

			return ret;
		}

		private static (int, int, DateTimeOffset, DateTimeOffset) EvaluateAmounts(int currentDayAmount, int strongestAmount, int weakestAmount, DateTimeOffset prevDate, DateTimeOffset strongestDate, DateTimeOffset weakestDate)
		{
			if (currentDayAmount > strongestAmount)
			{
				strongestAmount = currentDayAmount;
				strongestDate = prevDate;
			}

			if (currentDayAmount < weakestAmount && weakestAmount != -1)
			{
				weakestAmount = currentDayAmount;
				weakestDate = prevDate;
			}
			if (weakestAmount == -1) weakestAmount = currentDayAmount;

			return (strongestAmount, weakestAmount, strongestDate, weakestDate);
		}
	}

	public class SeasonEntryData
	{
		public string UUID { get; set; }
		public string Title { get; set; }
		public double Progress { get; set; }
		public bool Ended { get; set; }
		public int Average { get; set; }
		public int RemainingDays { get; set; }
		public DateTimeOffset StrongestDate { get; set; }
		public DateTimeOffset WeakestDate { get; set; }
		public int StrongestAmount { get; set; }
		public int WeakestAmount { get; set; }
		public List<HistoryEntry> History { get; set; }

		public SeasonEntryData(string uuid)
		{
			UUID = uuid;
		}

		public SeasonEntryData(string uuid, string title, double progress, bool ended, int average, int remainingDays, DateTimeOffset strongestDate, DateTimeOffset weakestDate, int strongestAmount, int weakestAmount, List<HistoryEntry> History)
		{
			(UUID, Title, Progress, Ended, Average, RemainingDays, StrongestDate, WeakestDate, StrongestAmount, WeakestAmount, History) = (uuid, title, progress, ended, average, remainingDays, strongestDate, weakestDate, strongestAmount, weakestAmount, history);
		}
	}
}
