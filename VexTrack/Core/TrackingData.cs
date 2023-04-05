using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core
{
	public abstract class TrackingData
	{
		public static int Streak { get; set; }
		public static long LastStreakUpdateTimestamp { get; set; }
		public static List<Contract> Contracts { get; private set; }
		public static List<Season> Seasons { get; private set; }

		protected TrackingData(int streak, long lastStreakUpdateTimestamp,List<Contract> contracts, List<Season> seasons)
		{
			(Streak, LastStreakUpdateTimestamp, Contracts, Seasons) = (streak, lastStreakUpdateTimestamp, contracts, seasons);
		}

		public static Season CurrentSeasonData => Seasons?.Last();
		
		

		// ================================
		//  Init and Convert
		// ================================
		
		private static void SetData(int streak, long lastStreakUpdateTimestamp, List<Contract> contracts, List<Season> seasons)
		{
			(Streak, LastStreakUpdateTimestamp, Contracts, Seasons) = (streak, lastStreakUpdateTimestamp ,contracts, seasons);
		}

		private static void InitData()
		{
			var todayTimestamp = ((DateTimeOffset)DateTimeOffset.Now.Date.ToLocalTime()).ToUnixTimeSeconds();
			List<Contract> contracts = new();
			List<Season> seasons = new();

			SetData(0, todayTimestamp, contracts, seasons);
		}
		
		
		
		// ================================
		//  Loading
		// ================================

		// --------------------------------[ Season ]--------------------------------
		
		private static List<Season> LoadSeasonsV1(JObject jo)
		{
			List<Season> seasons = new();
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

				List<HistoryEntry> history = new();
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
					history.Add(new HistoryEntry(sUuid, hUuid, time, gameMode, amount, map, desc, score, enemyScore, surrenderedWin, surrenderedLoss));
				}

				sUuid ??= Guid.NewGuid().ToString();
				seasons.Add(new Season(sUuid, name, DateTimeOffset.Parse(endDate).ToUnixTimeSeconds(), activeBpLevel, cXp, history));
			}
			
			return seasons;
		}
		
		private static List<Season> LoadSeasonsV2(JObject jo)
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

				var historyKey = "history";
				if (season["history"] == null)
				{
					historyKey = "xpHistory";
				}

				List<HistoryEntry> history = new();
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

					hUuid ??= Guid.NewGuid().ToString();
					history.Add(new HistoryEntry(sUuid, hUuid, time, gameMode, amount, map, desc, score, enemyScore, surrenderedWin, surrenderedLoss));
				}

				sUuid ??= Guid.NewGuid().ToString();
				seasons.Add(new Season(sUuid, name, endDate, activeBpLevel, cXp, history));
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
			List<Season> seasons = new();
			List<Contract> contracts = new();

			switch (version)
			{
				case "v1":
					(streak, lastStreakUpdateTimestamp) = LoadStreakV1(jo);
					seasons = LoadSeasonsV1(jo);
					contracts = LoadContractsV1(jo);
					reSave = true;
					break;
				
				case "v2":
					(streak, lastStreakUpdateTimestamp) = LoadStreakV2(jo);
					seasons = LoadSeasonsV2(jo);
					contracts = LoadContractsV2(jo);
					break;
			}

			if (seasons.Count == 0) CreateDataInitPopup();

			SetData(streak, lastStreakUpdateTimestamp, contracts, seasons);
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

				JArray history = new();
				foreach (var historyEntryObj in season.History.Select(historyEntry => new JObject()
				         {
					         { "uuid", historyEntry.Uuid },
					         { "gameMode", historyEntry.GameMode },
					         { "time", historyEntry.Time },
					         { "amount", historyEntry.Amount },
					         { "map", historyEntry.Map },
					         { "description", historyEntry.Description },
					         { "score", historyEntry.Score },
					         { "enemyScore", historyEntry.EnemyScore },
					         { "surrenderedWin", historyEntry.SurrenderedWin },
					         { "surrenderedLoss", historyEntry.SurrenderedLoss }
				         }))
				{
					history.Add(historyEntryObj);
				}

				seasonObj.Add("history", history);
				seasons.Add(seasonObj);
			}

			jo.Add("seasons", seasons);

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
			var prevTotalXp = CalcUtil.CalcTotalCollected(CurrentSeasonData.ActiveBpLevel, CurrentSeasonData.Cxp);

			var iter = 2;
			var cxp = CurrentSeasonData.History.Sum(he => he.Amount);

			while (cxp >= 0)
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
			var currTotalXp = CalcUtil.CalcTotalCollected(CurrentSeasonData.ActiveBpLevel, CurrentSeasonData.Cxp);
			var deltaXp = currTotalXp - prevTotalXp;

			
			foreach (var contract in Contracts)
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
			foreach (var t in Seasons)
			{
				var sortedHistory = t.History.OrderBy(h => h.Time).ToList();
				t.History = sortedHistory;
			}

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

		

		public static HistoryEntry GetLastHistoryEntry(string uuid)
		{
			return Seasons.Find(s => s.Uuid == uuid).History.Last();
		}


		public static void AddHistoryEntry(string seasonUuid, HistoryEntry data)
		{
			Seasons[Seasons.FindIndex(s => s.Uuid == seasonUuid)].History.Add(data);
			Recalculate();
			CallUpdate();
		}

		public static void RemoveHistoryEntry(string seasonUuid, string uuid)
		{
			Seasons[Seasons.FindIndex(s => s.Uuid == seasonUuid)]
				.History.RemoveAt(Seasons[Seasons.FindIndex(s => s.Uuid == seasonUuid)]
				.History.FindIndex(he => he.Uuid == uuid));

			Recalculate();
			CallUpdate();
		}

		public static void EditHistoryEntry(string seasonUuid, string uuid, HistoryEntry data)
		{
			Seasons[Seasons.FindIndex(s => s.Uuid == seasonUuid)]
				.History[Seasons[Seasons.FindIndex(s => s.Uuid == seasonUuid)]
				.History.FindIndex(he => he.Uuid == uuid)] = data;

			var historyVm = (HistoryViewModel)ViewModelManager.ViewModels[nameof(HistoryViewModel)];
			historyVm.EditEntry(new HistoryEntry(seasonUuid, uuid, data.Time, data.GameMode, data.Amount, data.Map, data.Description, data.Score, data.EnemyScore, data.SurrenderedWin, data.SurrenderedLoss));

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
