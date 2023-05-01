using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Util;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core.Model
{
	public abstract class Tracking
	{
		public static int Streak { get; set; }
		public static long LastStreakUpdateTimestamp { get; set; }
		public static List<Contract> Contracts { get; private set; }
		public static List<Season> Seasons { get; private set; }
		public static List<HistoryGroup> History { get; set; }

		protected Tracking(int streak, long lastStreakUpdateTimestamp,List<Contract> contracts, List<Season> seasons, List<HistoryGroup> history)
		{
			(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History) = (streak, lastStreakUpdateTimestamp, contracts, seasons, history);
		}

		public static Season CurrentSeasonData => Seasons?.Last();
		
		

		// ================================
		//  Init and Convert
		// ================================
		
		private static void SetData(int streak, long lastStreakUpdateTimestamp, List<Contract> contracts, List<Season> seasons, List<HistoryGroup> history)
		{
			(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History) = (streak, lastStreakUpdateTimestamp ,contracts, seasons, history);
		}

		private static void InitData()
		{
			var todayTimestamp = ((DateTimeOffset)DateTimeOffset.Now.Date.ToLocalTime()).ToUnixTimeSeconds();
			List<Contract> contracts = new();
			List<Season> seasons = new();
			List<HistoryGroup> history = new();

			SetData(0, todayTimestamp, contracts, seasons, history);
		}
		
		
		
		// ================================
		//  Loading
		// ================================
		
		// --------------------------------[ History ]--------------------------------
		
		private static List<HistoryGroup> LoadHistoryV2(JObject jo)
		{
			List<HistoryGroup> history = new();
			foreach (var jTokenGroup in jo["history"])
			{
				var historyGroup = (JObject)jTokenGroup;
				var sUuid = (string)historyGroup["sUuid"];
				var gUuid = (string)historyGroup["uuid"];
				var date = (long)historyGroup["date"];

				List<HistoryEntry> entries = new();
				foreach (var jTokenEntry in jTokenGroup["entries"])
				{
					var historyEntry = (JObject)jTokenEntry;
					var hUuid = (string)historyEntry["uuid"];
					var gamemode = (string)historyEntry["gameMode"];
					var time = (long)historyEntry["time"];
					var amount = (int)historyEntry["amount"];
					var map = (string)historyEntry["map"];
					var description = (string)historyEntry["description"];

					if (string.IsNullOrEmpty(map)) map = Constants.Maps.Last();

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

					hUuid ??= Guid.NewGuid().ToString();
					entries.Add(new HistoryEntry(gUuid, hUuid, time, gameMode, amount, map, desc, score, enemyScore,
						surrenderedWin, surrenderedLoss));
				}

				var sortedEntries = entries.OrderByDescending(he => he.Time).ToList();
				history.Add(new HistoryGroup(sUuid, gUuid, date, sortedEntries));
			}

			var sortedHistory = history.OrderByDescending(hg => hg.Date).ToList();
			return sortedHistory;
		}
		
		
		
		
		// --------------------------------[ Season ]--------------------------------
		
		private static (List<Season>, List<HistoryGroup>) LoadSeasonsAndHistoryV1(JObject jo)
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
					var map = (string)historyEntry["map"];
					var description = (string)historyEntry["description"];

					if (string.IsNullOrEmpty(map)) map = Constants.Maps.Last();

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
				seasons.Add(new Season(sUuid, name, DateTimeOffset.Parse(endDate).ToUnixTimeSeconds(), activeBpLevel, cXp));
			}
			
			foreach(var hg in history)
			{
				var sortedEntries = hg.Entries.OrderByDescending(he => he.Time).ToList();
				hg.Entries = sortedEntries;
			}
			
			var sortedHistory = history.OrderByDescending(hg => hg.Date).ToList();
			return (seasons, sortedHistory);
		}
		
		private static List<Season> LoadSeasonsV2(JObject jo, List<HistoryGroup> history)
		{
			List<Season> seasons = new();
			foreach (var jTokenSeason in jo["seasons"])
			{
				var season = (JObject)jTokenSeason;
				var sUuid = (string)season["uuid"];
				var name = (string)season["name"];
				var endDate = (long)season["endDate"];
				var activeBpLevel = (int)season["activeBPLevel"];
				var cXp = (int)season["cXP"];

				sUuid ??= Guid.NewGuid().ToString();
				seasons.Add(new Season(sUuid, name, endDate, activeBpLevel, cXp));
			}
			
			return seasons;
		}




		// --------------------------------[ Streak ]--------------------------------
		
		private static (int, long) LoadStreakV1(JObject jo)
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
			var todayTimestamp = ((DateTimeOffset)DateTimeOffset.Now.Date.ToLocalTime()).ToUnixTimeSeconds();
			if(streakEntries.First().Date == todayTimestamp) streakEntries.RemoveAt(0);

			return (streakEntries.TakeWhile(entry => entry.Status != "None").Count(), streakEntries.First().Date);
		}
		
		private static (int, long) LoadStreakV2(JObject jo)
		{
			var streak = (int?)jo["streak"] ?? 0;
			var lastStreakUpdateTimestamp = (long?)jo["lastStreakUpdate"] ?? 0;
			return (streak, lastStreakUpdateTimestamp);
		}

		
		
		// --------------------------------[ Contracts ]--------------------------------
		
		private static List<Contract> LoadContractsV1(JObject jo)
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

		private static List<Contract> LoadContractsV2(JObject jo)
		{
			List<Contract> contracts = new();
			foreach (var jTokenContract in jo["contracts"])
			{
				var contract = (JObject)jTokenContract;
				var source = contract["goals"];
				List<Goal> goals = new();
				
				if(source == null) continue;
				foreach (var jTokenGoal in source)
				{
					var goal = (JObject)jTokenGoal;
					var goalUuid = (string)goal["uuid"];
					var goalName = (string)goal["name"];

					var total = (int)goal["total"];
					var collected = (int)goal["collected"];
					if (collected > total) collected = total;
					
					goalUuid ??= Guid.NewGuid().ToString();
					goals.Add(new Goal(goalUuid, goalName, total, collected));
				}


				var uuid = (string)contract["uuid"];
				var name = (string)contract["name"];
				var color = (string)contract["color"];
				var paused = (bool)contract["paused"];
				contracts.Add(new Contract(uuid, name, color, paused, goals));
			}

			return contracts;
		}
		
		
		
		// --------------------------------[ General ]--------------------------------
		
		public static void LoadData()
		{
			if (!File.Exists(Constants.DataPath) || File.ReadAllText(Constants.DataPath) == "")
			{
				InitData();
				CreateDataInitPopup();
				return;
			}

			var rawJson = File.ReadAllText(Constants.DataPath);
			var jo = JObject.Parse(rawJson);

			var version = (string)jo["version"];
			if (string.IsNullOrEmpty(version)) version = "v1";

			var reSave = false;
			
			var streak = 0;
			var lastStreakUpdateTimestamp = (long)0;
			List<HistoryGroup> history = new();
			List<Season> seasons = new();
			List<Contract> contracts = new();

			switch (version)
			{
				case "v1":
					(streak, lastStreakUpdateTimestamp) = LoadStreakV1(jo);
					(seasons, history) = LoadSeasonsAndHistoryV1(jo);
					contracts = LoadContractsV1(jo);
					reSave = true;
					break;
				
				case "v2":
					(streak, lastStreakUpdateTimestamp) = LoadStreakV2(jo);
					history = LoadHistoryV2(jo);
					seasons = LoadSeasonsV2(jo, history);
					contracts = LoadContractsV2(jo);
					break;
			}

			if (seasons.Count == 0) CreateDataInitPopup();

			SetData(streak, lastStreakUpdateTimestamp, contracts, seasons, history);
			if(reSave) SaveData(); // Save in new format
			Recalculate();
		}
		
		
		
		// ================================
		//  Saving
		// ================================

		private static void SaveData()
		{
			JObject jo = new()
			{
				{ "version", Constants.DataVersion },
				{ "streak", Streak },
				{ "lastStreakUpdate", LastStreakUpdateTimestamp }
			};

			JArray contracts = new();
			foreach (var contract in Contracts)
			{
				JObject contractObj = new()
				{
					{ "uuid", contract.Uuid },
					{ "name", contract.Name },
					{ "color", contract.Color },
					{ "paused", contract.Paused }
				};

				JArray goals = new();
				foreach (var goalObj in contract.Goals.Select(goal => new JObject()
				         {
					         { "uuid", goal.Uuid },
					         { "name", goal.Name },
					         { "total", goal.Total },
					         { "collected", goal.Collected }
				         }))
				{
					goals.Add(goalObj);
				}

				contractObj.Add("goals", goals);
				contracts.Add(contractObj);
			}
			jo.Add("contracts", contracts);

			JArray seasons = new();
			foreach (var season in Seasons)
			{
				JObject seasonObj = new()
				{
					{ "uuid", season.Uuid },
					{ "name", season.Name },
					{ "endDate", season.EndDate },
					{ "activeBPLevel", season.ActiveBpLevel },
					{ "cXP", season.Cxp }
				};
				
				seasons.Add(seasonObj);
			}
			jo.Add("seasons", seasons);

			JArray history = new();
			foreach (var hg in History)
			{
				JObject hgObj = new()
				{
					{ "sUuid", hg.SeasonUuid },
					{ "uuid", hg.Uuid },
					{ "date", hg.Date }
				};

				JArray entries = new();
				foreach (var entryObj in hg.Entries.Select(he => new JObject()
				         {
					         { "uuid", he.Uuid },
			                 { "gameMode", he.GameMode },
			                 { "time", he.Time },
			                 { "amount", he.Amount },
			                 { "map", he.Map },
			                 { "description", he.Description },
			                 { "score", he.Score },
			                 { "enemyScore", he.EnemyScore },
			                 { "surrenderedWin", he.SurrenderedWin },
			                 { "surrenderedLoss", he.SurrenderedLoss }
				         }))
				{
					entries.Add(entryObj);
				}

				hgObj.Add("entries", entries);
				history.Add(hgObj);
			}
			jo.Add("history", history);

			if (!File.Exists(Constants.DataPath))
			{
				var sep = Constants.DataPath.LastIndexOf("/", StringComparison.Ordinal);

				Directory.CreateDirectory(Constants.DataPath[..sep]);
				File.CreateText(Constants.DataPath).Close();
			}

			File.WriteAllText(Constants.DataPath, jo.ToString());
		}
		
		

		// ================================
		//  Updating
		// ================================

		private static void Recalculate() // TODO: Restructure this to have Update() methods in Season and Contracts
		{
			List<string> completed = new();
			List<string> lost = new();

			//Get Completed goals
			var completedGoals = (from contract in Contracts from goal in contract.Goals where goal.Collected >= goal.Total select goal.Uuid).ToList();

			//Recalculate total collected XP, collected XP in level and current level
			var prevTotalXp = CalcHelper.CalcTotalCollected(CurrentSeasonData.ActiveBpLevel, CurrentSeasonData.Cxp);

			var iter = 2;
			var cxp = HistoryHelper.GetAllEntriesFromSeason(CurrentSeasonData.Uuid).Sum(he => he.Amount);

			while (cxp >= 0) // TODO: Find better way to figure out what battlepass level were gained / lost
			{
				if (iter <= Constants.BattlepassLevels) cxp -= Constants.Level2Offset + (iter * Constants.XpPerLevel);
				else if (iter < Constants.BattlepassLevels + Constants.EpilogueLevels + 2) cxp -= Constants.XpPerEpilogueLevel;
				else break;
				
				iter++;
			}
			iter--;

			for (var i = CurrentSeasonData.ActiveBpLevel - 1; i > iter - 1; i--)
				lost.Add("Battlepass Level " + i.ToString());

			for (var i = CurrentSeasonData.ActiveBpLevel; i < iter; i++)
				completed.Add("Battlepass Level " + i.ToString());

			CurrentSeasonData.ActiveBpLevel = iter;

			if (iter < Constants.BattlepassLevels)
				cxp += Constants.Level2Offset + (iter * Constants.XpPerLevel);
			else if (iter < Constants.BattlepassLevels + Constants.EpilogueLevels + 2)
				cxp += Constants.XpPerEpilogueLevel;

			CurrentSeasonData.Cxp = cxp;

			//Calculate difference in XP and apply to goals
			var currTotalXp = CalcHelper.CalcTotalCollected(CurrentSeasonData.ActiveBpLevel, CurrentSeasonData.Cxp);
			var deltaXp = currTotalXp - prevTotalXp;

			
			foreach (var contract in Contracts) // TODO: Move contents to update() function within contracts
			{
				if (contract.Paused) continue;
				var xpPool = Math.Abs(deltaXp);
				
				foreach (var goal in contract.Goals)
				{
					var goalLimit = deltaXp > 0 ? goal.Total - goal.Collected : goal.Collected;

					var appliedXp = xpPool > goalLimit ? goalLimit : xpPool;
					appliedXp *= Math.Sign(deltaXp);
					xpPool -= appliedXp;
						
					var newCollected = goal.Collected + appliedXp;
					if (newCollected < 0) newCollected = 0;

					goal.Collected = newCollected;

					if (goal.Collected >= goal.Total && !completedGoals.Contains(goal.Uuid)) completed.Add(goal.Name);
					if (goal.Collected < goal.Total && completedGoals.Contains(goal.Uuid)) lost.Add(goal.Name);
				}
			}

			if (completed.Count == 0 && lost.Count == 0) return;
			var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
			var paPopupVm = (ProgressActivityPopupViewModel)ViewModelManager.ViewModels[nameof(ProgressActivityPopupViewModel)];

			paPopupVm.SetData(completed, lost);
			mainVm.QueuePopup(paPopupVm);
		}

		private static void CallUpdate()
		{
			SaveData();
			var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
			mainVm.Update();
		}

		public static void ResetData()
		{
			InitData();
			SaveData();
			LoadData();
			CallUpdate();
		}

		
		
		
		public static void AddHistoryEntry(HistoryEntry data)
		{
			var seasonUuid = Seasons.Find(s => data.Time > s.StartDate && data.Time < s.EndDate)?.Uuid;

			if (string.IsNullOrEmpty(seasonUuid))
			{
				seasonUuid = CurrentSeasonData.Uuid;
				data.Time = DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds();
			}
			
			var date = TimeHelper.IsolateTimestampDate(data.Time);
			var groupUuid = History.Find(hg => hg.Date == date && hg.SeasonUuid == seasonUuid)?.Uuid;
					
			if (string.IsNullOrEmpty(groupUuid))
			{
				groupUuid = Guid.NewGuid().ToString();
				History.Add(new HistoryGroup(seasonUuid, groupUuid, date, new List<HistoryEntry>()));
			}

			data.GroupUuid = groupUuid;
			History.Find(hg => hg.Uuid == groupUuid).Entries.Add(data);

			HistoryHelper.SortHistory();
			
			Recalculate();
			CallUpdate();
		}

		public static void RemoveHistoryEntry(string groupUuid, string uuid)
		{
			var hgIdx = History.FindIndex(hg => hg.Uuid == groupUuid);
			var heIdx = History[hgIdx].Entries.FindIndex(he => he.Uuid == uuid);
			History[hgIdx].Entries.RemoveAt(heIdx);

			if(History[hgIdx].Entries.Count < 1) History.RemoveAt(hgIdx);
			
			Recalculate();
			CallUpdate();
		}

		public static void EditHistoryEntry(string groupUuid, string uuid, HistoryEntry data)
		{
			var hgIdx = History.FindIndex(hg => hg.Uuid == groupUuid);
			var heIdx = History[hgIdx].Entries.FindIndex(he => he.Uuid == uuid);
			History[hgIdx].Entries[heIdx] = data;
			
			HistoryHelper.SortHistory();
			
			Recalculate();
			CallUpdate();
		}

		
		
		public static void AddContract(Contract data)
		{
			Contracts.Add(data);
			CallUpdate();
		}

		// TODO: Update for Contracts
		public static void RemoveContract(string uuid)
		{
			Contracts.RemoveAt(Contracts.FindIndex(gg => gg.Uuid == uuid));
			CallUpdate();
		}

		// TODO: Update for Contracts
		public static void EditContract(string uuid, Contract data)
		{
			var index = Contracts.FindIndex(c => c.Uuid == uuid);
			if (index < 0) return;
			
			Contracts[index] = data;
			CallUpdate();
		}



		public static void AddSeason(Season data)
		{
			Seasons.Add(data);
			CallUpdate();
		}

		public static void EndSeason(string uuid)
		{
			// Set end date to today
			Seasons[Seasons.FindIndex(s => s.Uuid == uuid)].EndDate = ((DateTimeOffset)DateTime.Today.ToLocalTime()).ToUnixTimeSeconds();
			CallUpdate();
		}


		private static void CreateDataInitPopup()
		{
			var dataInitPopup = (DataInitPopupViewModel)ViewModelManager.ViewModels[nameof(DataInitPopupViewModel)];
			var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];

			dataInitPopup.InitData();
			dataInitPopup.CanCancel = false;
			mainVm.InterruptUpdate = true;

			mainVm.QueuePopup(dataInitPopup);
		}
	}
	
	
	
	public class LegacyStreakEntry
	{
		public long Date { get; set; }
		public string Status { get; set; }

		public LegacyStreakEntry(long date, string status)
		{
			Date = date;
			Status = status;
		}
	}
}
