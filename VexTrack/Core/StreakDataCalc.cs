using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VexTrack.Core
{
	public static class StreakDataCalc
	{
		public static int CalcCurrentStreak(bool epilogue)
		{
			int streak = 0;
			List<string> targetStatus = new();
			targetStatus.Add(Constants.StreakStatusOrder.Keys.ElementAt(2));
			if(!epilogue) targetStatus.Add(Constants.StreakStatusOrder.Keys.ElementAt(1));

			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			foreach(StreakEntry streakEntry in TrackingDataHelper.Data.Streak)
			{
				if (streakEntry.Date == today.ToUnixTimeSeconds() && !targetStatus.Contains(streakEntry.Status)) continue;
				if (!targetStatus.Contains(streakEntry.Status)) break;
				streak++;
			}

			if (TrackingDataHelper.Data.Streak.FindIndex(x => x.Date == today.ToUnixTimeSeconds()) == -1) SetStreakEntry(today, Constants.StreakStatusOrder.Keys.ElementAt(0));
			return streak;
		}

		public static void SetStreakEntry(DateTimeOffset date, string status)
		{
			date = date.ToLocalTime().Date;

			int index = TrackingDataHelper.Data.Streak.FindIndex(x => x.Date == date.ToUnixTimeSeconds());
			if (index == -1)
			{
				TrackingDataHelper.Data.Streak.Add(new StreakEntry(Guid.NewGuid().ToString(), date.ToUnixTimeSeconds(), status));
				TrackingDataHelper.Data.Streak = TrackingDataHelper.Data.Streak.OrderByDescending(t => t.Date).ToList();
				TrackingDataHelper.SaveData();
				return;
			}

			//if (Constants.StreakStatusOrder[status] <= Constants.StreakStatusOrder[TrackingDataHelper.Data.Streak[index].Status]) return;
			TrackingDataHelper.Data.Streak[index].Status = status;
			TrackingDataHelper.SaveData();
		}

		public static Brush GetStreakColor(DateTimeOffset date, bool epilogue)
		{
			Dictionary<string,  Brush> brushes = new Dictionary<string, Brush>();
			brushes.Add(Constants.StreakStatusOrder.Keys.ElementAt(0), (Brush)Application.Current.FindResource("Shade"));
			brushes.Add(Constants.StreakStatusOrder.Keys.ElementAt(1), (Brush)Application.Current.FindResource("AccOrange"));
			brushes.Add(Constants.StreakStatusOrder.Keys.ElementAt(2), (Brush)Application.Current.FindResource("AccBlue"));

			date = date.ToLocalTime().Date;

			int index = TrackingDataHelper.Data.Streak.FindIndex(x => x.Date == date.ToUnixTimeSeconds());
			if (index == -1) return null;

			string status = TrackingDataHelper.Data.Streak[index].Status;
			if (Constants.StreakStatusOrder[status] > 1 && !epilogue) status = Constants.StreakStatusOrder.Keys.ElementAt(1);
			if (Constants.StreakStatusOrder[status] == 1 && epilogue) status = Constants.StreakStatusOrder.Keys.ElementAt(0);

			return brushes[status];
		}
	}
}
