using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.Game.Templates;

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
				var gamemodeUuid = (string)historyEntry["gameMode"];
				var time = (long)historyEntry["time"];
				var amount = (int)historyEntry["amount"];
				var mapUuid = (string)historyEntry["map"];
				var description = (string)historyEntry["description"];

				mapUuid = !string.IsNullOrEmpty(mapUuid) ? Model.ApiData.Maps.Find(m => m.Name == mapUuid).Uuid : // Convert Map Name to UUID
					Model.ApiData.Maps.Last().Uuid;
				var map = Model.ApiData.Maps.Find(m => m.Uuid == mapUuid);

				// These are here because of typos I did way back in v1.7
				if (gamemodeUuid == "Competetive") gamemodeUuid = "Competitive";
				if (gamemodeUuid == "Snowballfight") gamemodeUuid = "Snowball Fight";

				GameMode gameMode;
				string desc;
				int score, enemyScore;
				bool surrenderedWin, surrenderedLoss;

				if (gamemodeUuid == null)
				{
					(gameMode, desc, score, enemyScore) = HistoryEntry.DescriptionToScores(description);
					surrenderedWin = false;
					surrenderedLoss = false;
				}
				else
				{
					gameMode = Model.ApiData.GameModes.Find(gm => gm.Name == gamemodeUuid);
					desc = description;
					score = (int)historyEntry["score"];
					enemyScore = (int)historyEntry["enemyScore"];
					surrenderedWin = (bool)historyEntry["surrenderedWin"];
					surrenderedLoss = (bool)historyEntry["surrenderedLoss"];
				}
				
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

			var startTimestamp = TimeHelper.IsolateTimestampDate(HistoryHelper.GetFirstFromSeason(sUuid, history).Time);
			var endTimestamp = TimeHelper.StringToTimestamp(endDate);

			// If a Season with a matching name is found in ApiData, use the Uuid from the ApiData
			foreach (var template in Model.ApiData.ContractTemplates.Where(ct => ct.Type == "Season"))
			{
				var tokens = name.Split(' ');
				var romanRegex = new Regex(@"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$");

				for (var i = 0; i < tokens.Length; i++)
				{
					if (romanRegex.IsMatch(tokens[i])) tokens[i] = FormatHelper.RomanToArabicNumbers(tokens[i]);
				}

				var modifiedName = string.Join(' ', tokens);
                modifiedName = Regex.Replace(modifiedName, "[^a-zA-Z0-9]", string.Empty);
				modifiedName = modifiedName.ToLower();
				
				var modifiedApiName = Regex.Replace(template.Name, "[^a-zA-Z0-9]", string.Empty);
				modifiedApiName = modifiedApiName.ToLower();

				if (modifiedName.Equals(modifiedApiName))
				{
					var prevSUuid = sUuid;
					
					sUuid = template.Uuid;
					startTimestamp = template.StartTimestamp;
					endTimestamp = template.EndTimestamp;

					history.Where(hg => hg.SeasonUuid == prevSUuid).ToList().ForEach(hg => hg.SeasonUuid = sUuid);
					break;
				}
			}
            
			// Convert old xp tracking for seasons to newer one similar to contracts
			var goals = new List<Goal>();
			var templates = Model.ApiData.ContractTemplates.Find(ct => ct.Type == "Season" && ct.Uuid == sUuid)?.Goals;
			var goalIdx = 0;

			if (templates != null)
			{
				foreach (var template in templates)
				{
					var collected = 0;

					if (goalIdx == activeBpLevel - 1) collected = cXp;
					if (goalIdx < activeBpLevel - 1) collected = template.XpTotal;
					goalIdx++;
				
					var goal = new Goal(template, Guid.NewGuid().ToString(), collected);
					goals.Add(goal);
				}
			}
			else
			{
				var minTotal = CalcHelper.CalcMaxForSeason(false);
				var maxTotal = CalcHelper.CalcMaxForSeason(true);
				var epilogueTotal = maxTotal - minTotal;

				var collectedPool = HistoryHelper.CalcCollectedFromSeason(sUuid, history);
				var normalCollected = collectedPool > minTotal ? minTotal : collectedPool;
				var epilogueCollected = collectedPool - normalCollected;
				
                goals.Add(new Goal(new GoalTemplate(new List<Reward>(), false, false, 0, minTotal, false, 0, false, name + " Battlepass"), Guid.NewGuid().ToString(), normalCollected));
                goals.Add(new Goal(new GoalTemplate(new List<Reward>(), false, false, 0, epilogueTotal, false, 0, false, name + " Epilogue"), Guid.NewGuid().ToString(), epilogueCollected));
			}
			
			sUuid ??= Guid.NewGuid().ToString();
			seasons.Add(new Season(sUuid, name, startTimestamp, endTimestamp, goals));
		}
		
		foreach(var hg in history)
		{
			var sortedEntries = hg.Entries.OrderByDescending(he => he.Time).ToList();
			hg.Entries = sortedEntries;
		}
		
		var sortedSeasons = seasons.OrderByDescending(s => s.StartTimestamp).ToList();
		var sortedHistory = history.OrderByDescending(hg => hg.Date).ToList();
		return (sortedSeasons, sortedHistory);
	}




	// --------------------------------[ Streak ]--------------------------------
	
	internal static (int, long) LoadStreak(JObject jo)
	{
		List<LegacyStreakEntry> streakEntries = new();
		if (jo["streak"] != null)
		{
			streakEntries.AddRange(from JObject streakEntry in jo["streak"]
				let date = streakEntry.Value<long>("date")
				let status = streakEntry.Value<string>("status")
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

			List<GoalTemplate> goalTemplates = new();
			List<Goal> goals = new();
			if (source == null) continue;
			foreach (var jTokenGoal in source)
			{
				var goal = (JObject)jTokenGoal;
				var goalUuid = goal.Value<string>("uuid");
				var goalName = goal.Value<string>("name");

				var total = goal.Value<int>("total");
				var collected = goal.Value<int>("collected");
				if (collected > total) collected = total;

				goalUuid ??= Guid.NewGuid().ToString();
				var goalTemplate = new GoalTemplate(new List<Reward>{new("", "", 0, false)}, false, false, 0, total, false, 0);
				var goalObj = new Goal(goalTemplate, goalUuid, collected);

				if (!convertToGrouped)
				{
					goalTemplates.Add(goalTemplate);
					goals.Add(goalObj);
				}
				else contracts.Add(new Contract(new ContractTemplate(Guid.NewGuid().ToString(), goalName,
						"", -1, -1, new List<GoalTemplate> { goalTemplate }), new List<Goal> { goalObj }));
			}

			if (convertToGrouped) return contracts;
			
			var uuid = contract.Value<string>("uuid");
			var name = contract.Value<string>("name"); 

			if(goals.Count > 0) contracts.Add(new Contract(new ContractTemplate(uuid, name, "", -1, -1, goalTemplates), goals));
		}

		return contracts;
	}
}