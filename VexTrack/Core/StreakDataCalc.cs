using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VexTrack.Core
{
	public static class StreakDataCalc
	{
		public static int CalcCurrentStreak(bool epilogue)
		{
			var streak = 0;
			List<string> targetStatus = new();
			targetStatus.Add(Constants.StreakStatusOrder.Keys.ElementAt(2));
			if (!epilogue) targetStatus.Add(Constants.StreakStatusOrder.Keys.ElementAt(1));

			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;
			var prevDate = DateTimeOffset.FromUnixTimeSeconds(TrackingData.Streak[0].Date);

			foreach (var streakEntry in TrackingData.Streak)
			{
				if ((prevDate - DateTimeOffset.FromUnixTimeSeconds(streakEntry.Date)).Days > 1) break;
				prevDate = DateTimeOffset.FromUnixTimeSeconds(streakEntry.Date);

				if (streakEntry.Date == today.ToUnixTimeSeconds() && !targetStatus.Contains(streakEntry.Status)) continue;
				if (!targetStatus.Contains(streakEntry.Status)) break;
				streak++;
			}

			if (TrackingData.Streak.FindIndex(x => x.Date == today.ToUnixTimeSeconds()) == -1) SetStreakEntry(today, Constants.StreakStatusOrder.Keys.ElementAt(0));
			return streak;
		}

		public static void SetStreakEntry(DateTimeOffset date, string status)
		{
			date = date.ToLocalTime().Date;

			var index = TrackingData.Streak.FindIndex(x => x.Date == date.ToUnixTimeSeconds());
			if (index == -1)
			{
				TrackingData.Streak.Add(new StreakEntry(Guid.NewGuid().ToString(), date.ToUnixTimeSeconds(), status));
				TrackingData.Streak = TrackingData.Streak.OrderByDescending(t => t.Date).ToList();
				TrackingData.SaveData();
				return;
			}

			//if (Constants.StreakStatusOrder[status] <= Constants.StreakStatusOrder[TrackingData.Streak[index].Status]) return;
			TrackingData.Streak[index].Status = status;
			TrackingData.SaveData();
		}

		public static Brush GetStreakColor(DateTimeOffset date, bool epilogue)
		{
			var brushes = new Dictionary<string, Brush>();
			brushes.Add(Constants.StreakStatusOrder.Keys.ElementAt(0), (Brush)Application.Current.FindResource("Shade"));
			brushes.Add(Constants.StreakStatusOrder.Keys.ElementAt(1), (Brush)Application.Current.FindResource("AccOrange"));
			brushes.Add(Constants.StreakStatusOrder.Keys.ElementAt(2), (Brush)Application.Current.FindResource("AccBlue"));

			date = date.ToLocalTime().Date;

			var index = TrackingData.Streak.FindIndex(x => x.Date == date.ToUnixTimeSeconds());
			if (index == -1) return null;

			var status = TrackingData.Streak[index].Status;
			if (Constants.StreakStatusOrder[status] > 1 && !epilogue) status = Constants.StreakStatusOrder.Keys.ElementAt(1);
			if (Constants.StreakStatusOrder[status] == 1 && epilogue) status = Constants.StreakStatusOrder.Keys.ElementAt(0);

			return brushes[status];
		}
	}
	
	public class StreakEntry
	{
		public string Uuid { get; set; }
		public long Date { get; set; }
		public string Status { get; set; }

		public StreakEntry(string uuid, long date, string status)
		{
			Uuid = uuid;
			Date = date;
			Status = status;
		}
	}
}
