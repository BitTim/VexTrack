using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;

namespace VexTrack.Core.IO.UserData.LoaderParts;

public static class UserDataV1
{
	// --------------------------------[ Season + History ]--------------------------------
	
	internal static (List<Season>, List<HistoryGroup>) LoadSeasonsAndHistory(JObject jo)
	{
		List<Season> seasons = new();
		List<HistoryGroup> history = new();
		
		foreach (var jTokenSeason in jo["seasons"])
		{
			var season = (JObject)jTokenSeason;
			var sUuid = (string)season["uuid"];
			var name = (string)season["name"];
			var endDate = (string)season["endDate"];
			var activeBpLevel = (int)season["activeBPLevel"];
			var cXp = (int)season["cXP"];

			var historyKey = "history";
			if (season["history"] == null)
			{
				historyKey = "xpHistory";
			}
			
			foreach (var jTokenEntry in season[historyKey])
			{
				var historyEntry = (JObject)jTokenEntry;
				var hUuid = (string)historyEntry["uuid"];
				var gamemode = (string)historyEntry["gameMode"];
				var time = (long)historyEntry["time"];
				var amount = (int)historyEntry["amount"];
				var mapUuid = (string)historyEntry["map"];
				var description = (string)historyEntry["description"];

				mapUuid = !string.IsNullOrEmpty(mapUuid) ? Model.ApiData.Maps.Find(m => m.Name == mapUuid).Uuid : // Convert Map Name to UUID
					Model.ApiData.Maps.Last().Uuid;
				var map = Model.ApiData.Maps.Find(m => m.Uuid == mapUuid);
				
				string gameMode, desc;
				int score, enemyScore;
				bool surrenderedWin, surrenderedLoss;

				if (gamemode == null)
				{
					(gameMode, desc, score, enemyScore) = HistoryEntry.DescriptionToScores(description);
					surrenderedWin = false;
					surrenderedLoss = false;
				}
				else
				{
					gameMode = gamemode;
					desc = description;
					score = (int)historyEntry["score"];
					enemyScore = (int)historyEntry["enemyScore"];
					surrenderedWin = (bool)historyEntry["surrenderedWin"];
					surrenderedLoss = (bool)historyEntry["surrenderedLoss"];
				}

				// This is because of a typo I did way back in v1.7
				if (gameMode == "Competetive") gameMode = "Competitive";
				
				hUuid ??= Guid.NewGuid().ToString();

				var date = TimeHelper.IsolateTimestampDate(time);
				var gUuid = history.Find(hg => hg.Date == date && hg.SeasonUuid == sUuid)?.Uuid;
				
				if (string.IsNullOrEmpty(gUuid))
				{
					gUuid = Guid.NewGuid().ToString();
					history.Add(new HistoryGroup(sUuid, gUuid, date, new List<HistoryEntry>()));
				}
				
				history.Find(hg => hg.Uuid == gUuid).Entries.Add(new HistoryEntry(gUuid, hUuid, time, gameMode, amount, map, desc, score, enemyScore, surrenderedWin, surrenderedLoss));
			}

			sUuid ??= Guid.NewGuid().ToString();
			seasons.Add(new Season(sUuid, name, TimeHelper.StringToTimestamp(endDate), activeBpLevel, cXp));
		}
		
		foreach(var hg in history)
		{
			var sortedEntries = hg.Entries.OrderByDescending(he => he.Time).ToList();
			hg.Entries = sortedEntries;
		}
		
		var sortedHistory = history.OrderByDescending(hg => hg.Date).ToList();
		return (seasons, sortedHistory);
	}




	// --------------------------------[ Streak ]--------------------------------
	
	internal static (int, long) LoadStreak(JObject jo)
	{
		List<LegacyStreakEntry> streakEntries = new();
		if (jo["streak"] != null)
		{
			streakEntries.AddRange(from JObject streakEntry in jo["streak"]
				let date = (long)streakEntry["date"]
				let status = (string)streakEntry["status"]
				select new LegacyStreakEntry(date, status));
		}
		streakEntries = streakEntries.OrderByDescending(t => t.Date).ToList();
		
		// Check if current day is already listed and remove it from the list if yes
		if(streakEntries.First().Date == TimeHelper.TodayTimestamp) streakEntries.RemoveAt(0);

		return (streakEntries.TakeWhile(entry => entry.Status != "None").Count(), streakEntries.First().Date);
	}

	
	
	// --------------------------------[ Contracts ]--------------------------------
	
	internal static List<Contract> LoadContracts(JObject jo)
	{
		List<Contract> contracts = new();
		foreach (var jTokenContract in jo["goals"])
		{
			var contract = (JObject)jTokenContract;
			var source = contract["goals"];
			var convertToGrouped = false;
			if (contract["goals"] == null)
			{
				source = jo["goals"];
				convertToGrouped = true;
			}

			List<Goal> goals = new();
			if (source == null) continue;
			foreach (var jTokenGoal in source)
			{
				var goal = (JObject)jTokenGoal;
				var goalUuid = (string)goal["uuid"];
				var goalName = (string)goal["name"];

				var total = (int)goal["total"];
				var collected = (int)goal["collected"];
				if (collected > total) collected = total;

				goalUuid ??= Guid.NewGuid().ToString();
				var goalObj = new Goal(goalUuid, goalName, total, collected);
				
				if(!convertToGrouped) goals.Add(goalObj);
				else contracts.Add(new Contract(Guid.NewGuid().ToString(), goalName,
						(string)goal["color"], (bool)goal["paused"], new List<Goal> { goalObj }));
			}

			if (convertToGrouped) return contracts;
			
			var uuid = (string)contract["uuid"];
			var name = (string)contract["name"];
			var loadedGoals = (JArray)contract["goals"];

			string color;
			bool paused;

			if (loadedGoals.Count < 1) (color, paused) = ("", false);
			else (color, paused) = ((string)loadedGoals.First()["color"], (bool)loadedGoals.First()["paused"]);
			
			if(goals.Count > 0) contracts.Add(new Contract(uuid, name, color, paused, goals));
		}

		return contracts;
	}
}