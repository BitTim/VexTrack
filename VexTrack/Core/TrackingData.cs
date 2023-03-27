using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core
{
	public class TrackingData
	{
		public static List<Contract> Contracts { get; set; }
		public static List<StreakEntry> Streak { get; set; }
		public static List<Season> Seasons { get; set; }

		public TrackingData(List<Contract> contracts, List<StreakEntry> streak, List<Season> seasons)
		{
			(Contracts, Streak, Seasons) = (contracts, streak, seasons);
		}

		public static Season CurrentSeasonData => Seasons?.Last();

		public static int GetRemainingDays(string sUuid = "", DateTimeOffset endDate = new(), bool overrideEndDate = false)
		{
			if (sUuid == "") sUuid = CurrentSeasonData.Uuid;

			if (!overrideEndDate) endDate = DateTimeOffset.FromUnixTimeSeconds(Seasons.Find(s => s.Uuid == sUuid).EndDate).ToLocalTime().Date;
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;

			var remainingDays = (endDate - today).Days;
			if ((endDate - today).Hours > 12) { remainingDays += 1; }
			if (remainingDays < 0) remainingDays = 0;

			return remainingDays;
		}

		public static int GetDuration(string sUuid = "")
		{
			if (sUuid == "") sUuid = CurrentSeasonData.Uuid;
			
			DateTimeOffset endDate = DateTimeOffset.FromUnixTimeSeconds(Seasons.Find(s => s.Uuid == sUuid).EndDate).ToLocalTime().Date;
			DateTimeOffset startDate = DateTimeOffset.FromUnixTimeSeconds(Seasons.Find(s => s.Uuid == sUuid).History.First().Time).ToLocalTime().Date;

			var duration = (endDate - startDate).Days;
			if ((endDate - startDate).Hours > 12) { duration += 1; }
			if (duration < 0) duration = 0;

			return duration;
		}
		
		

		// ================================
		//  Init and Convert
		// ================================
		
		private static void SetData(List<Contract> contracts, List<StreakEntry> streak, List<Season> seasons)
		{
			(Contracts, Streak, Seasons) = (contracts, streak, seasons);
		}
		
		public static void InitData()
		{
			List<Contract> contracts = new();
			List<StreakEntry> streak = new();
			List<Season> seasons = new();

			SetData(contracts, streak, seasons);
		}

		public static void ConvertFromLegacyData()
		{
			var rawJson = File.ReadAllText(Constants.LegacyDataPath);
			var jo = JObject.Parse(rawJson);

			List<Contract> contracts = new();
			foreach (var jTokenGoals in jo["goals"]!)
			{
				var contract = (JObject)jTokenGoals;
				var gName = (string)contract["name"];
				var collected = CalcUtil.CalcTotalCollected((int)jo["activeBPLevel"], (int)jo["cXP"]) - (int)contract["startXP"];
				var total = collected + (int)contract["remaining"];
				var color = (string)contract["color"];

				contracts.Add(new Contract(Guid.NewGuid().ToString(), gName, color, false, new List<Goal>()));
				contracts.Last().Goals.Add(new Goal(Guid.NewGuid().ToString(), gName, total, collected));
			}

			List<StreakEntry> streak = new();
			List<Season> seasons = new();

			const string name = "Legacy Season";
			var endDate = (string)jo["seasonEndDate"];
			var activeBpLevel = (int)jo["activeBPLevel"];
			var cXp = (int)jo["cXP"];
			var seasonUuid = Guid.NewGuid().ToString();

			List<HistoryEntry> history = new();
			foreach (var jTokenHistory in jo["history"]!)
			{
				var historyEntry = (JObject)jTokenHistory;
				var time = (long)historyEntry["time"];
				var description = (string)historyEntry["description"];
				var amount = (int)historyEntry["amount"];
				var map = (string)historyEntry["map"];

				if (string.IsNullOrEmpty(map)) map = Constants.Maps.Last();
				var (gameMode, desc, score, enemyScore) = HistoryEntry.DescriptionToScores(description);

				history.Add(new HistoryEntry(seasonUuid, Guid.NewGuid().ToString(), time, gameMode, amount, map, desc, score, enemyScore, false, false));
			}

			seasons.Add(new Season(seasonUuid, name, DateTimeOffset.Parse(endDate!).ToUnixTimeSeconds(), activeBpLevel, cXp, history));
			SetData(contracts, streak, seasons);

			File.Move(Constants.LegacyDataPath, Constants.LegacyDataPath + ".bak");
			SaveData();

			var editableSeasonPopup = (EditableSeasonPopupViewModel)ViewModelManager.ViewModels["EditableSeasonPopup"];
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];

			editableSeasonPopup.CanCancel = false;
			editableSeasonPopup.SetParameters("Edit Season", true);
			editableSeasonPopup.SetData(seasons.Last());

			mainVm.QueuePopup(editableSeasonPopup);
		}

		
		
		// ================================
		//  Loading
		// ================================

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

		private static List<StreakEntry> LoadStreakV1(JObject jo)
		{
			List<StreakEntry> streak = new();
			if (jo["streak"] != null)
			{
				foreach (var jTokenStreak in jo["streak"])
				{
					var streakEntry = (JObject)jTokenStreak;
					var uuid = (string)streakEntry["uuid"];
					var date = (long)streakEntry["date"];
					var status = (string)streakEntry["status"];

					uuid ??= Guid.NewGuid().ToString();
					streak.Add(new StreakEntry(uuid, date, status));
				}
			}
			streak = streak.OrderByDescending(t => t.Date).ToList();
			return streak;
		}

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
		
		public static void LoadData()
		{
			if (!File.Exists(Constants.DataPath) || File.ReadAllText(Constants.DataPath) == "")
			{
				if (File.Exists(Constants.LegacyDataPath))
				{
					ConvertFromLegacyData();
					return;
				}

				InitData();
				CreateDataInitPopup();
				return;
			}

			var rawJson = File.ReadAllText(Constants.DataPath);
			var jo = JObject.Parse(rawJson);

			var version = (string)jo["version"];
			if (string.IsNullOrEmpty(version)) version = "v1";

			List<Season> seasons = new();
			List<StreakEntry> streak = new();
			List<Contract> contracts = new();

			switch (version)
			{
				case "v1":
					seasons = LoadSeasonsV1(jo);
					streak = LoadStreakV1(jo);
					contracts = LoadContractsV1(jo);
					break;
				
				case "v2":
					seasons = LoadSeasonsV2(jo);
					streak = LoadStreakV1(jo);
					contracts = LoadContractsV2(jo);
					break;
			}

			if (seasons.Count == 0) CreateDataInitPopup();

			SetData(contracts, streak, seasons);
			Recalculate();
		}
		
		
		
		// ================================
		//  Saving
		// ================================

		public static void SaveData()
		{
			JObject jo = new() { { "version", Constants.DataVersion } };

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

			Streak = Streak.OrderByDescending(t => t.Date).ToList();
			JArray streak = new();
			foreach (var streakEntry in Streak)
			{
				JObject streakEntryObj = new()
				{
					{ "uuid", streakEntry.Uuid },
					{ "date", streakEntry.Date },
					{ "status", streakEntry.Status }
				};

				streak.Add(streakEntryObj);
			}
			jo.Add("streak", streak);

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
		
		public static void Recalculate()
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
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			var paPopupVm = (ProgressActivityPopupViewModel)ViewModelManager.ViewModels["PAPopup"];

			paPopupVm.SetData(completed, lost);
			mainVm.QueuePopup(paPopupVm);
		}

		public static void CallUpdate()
		{
			for (var i = 0; i < Seasons.Count; i++)
			{
				var sortedHistory = Seasons[i].History.OrderBy(h => h.Time).ToList();
				Seasons[i].History = sortedHistory;
			}

			SaveData();
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			mainVm.Update();
		}

		public static void ResetData()
		{
			InitData();
			SaveData();
			LoadData();
			CallUpdate();
		}



		public static HistoryEntry GetHistoryEntry(string seasonUuid, string uuid)
		{
			return Seasons.Find(s => s.Uuid == seasonUuid).History.Find(h => h.Uuid == uuid);
		}

		public static HistoryEntry GetFirstHistoryEntry(string uuid)
		{
			return Seasons.Find(s => s.Uuid == uuid).History.First();
		}

		public static HistoryEntry GetLastHistoryEntry(string uuid)
		{
			return Seasons.Find(s => s.Uuid == uuid).History.Last();
		}

		public static Season GetSeason(string uuid)
		{
			return Seasons.Find(s => s.Uuid == uuid);
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

			var historyVm = (HistoryViewModel)ViewModelManager.ViewModels["History"];
			historyVm.EditEntry(new HistoryEntry(seasonUuid, uuid, data.Time, data.GameMode, data.Amount, data.Map, data.Description, data.Score, data.EnemyScore, data.SurrenderedWin, data.SurrenderedLoss));

			Recalculate();
			CallUpdate();
		}



		public static void AddGoal(string contractUuid, Goal data)
		{
			Contracts[Contracts.FindIndex(c => c.Uuid == contractUuid)].Goals.Add(data);
			CallUpdate();
		}

		public static void RemoveGoal(string contractUuid, string uuid)
		{
			Contracts[Contracts.FindIndex(c => c.Uuid == contractUuid)]
				.Goals.RemoveAt(Contracts[Contracts.FindIndex(c => c.Uuid == contractUuid)]
				.Goals.FindIndex(g => g.Uuid == uuid));
			CallUpdate();
		}

		public static void EditGoal(string contractUuid, string uuid, Goal data)
		{
			var index = Contracts[Contracts.FindIndex(c => c.Uuid == contractUuid)]
							.Goals.FindIndex(g => g.Uuid == uuid);
			if (index >= 0)
			{
				Contracts[Contracts.FindIndex(c => c.Uuid == contractUuid)]
					.Goals[index] = data;
				CallUpdate();
				return;
			}

			var prevGroupUuid = Contracts.First(c => c.Goals.Any(g => g.Uuid == uuid)).Uuid;
			MoveGoal(prevGroupUuid, contractUuid, uuid, true);
		}

		// TODO: Evaluate if this is still needed
		public static void MoveGoal(string srcGroupUuid, string dstGroupUuid, string uuid, bool deleteGoalFromGroup = false)
		{
			var goal = Contracts[Contracts.FindIndex(gg => gg.Uuid == srcGroupUuid)]
							.Goals[Contracts[Contracts.FindIndex(gg => gg.Uuid == srcGroupUuid)]
							.Goals.FindIndex(g => g.Uuid == uuid)];

			AddGoal(dstGroupUuid, goal);
			if (deleteGoalFromGroup) RemoveGoal(srcGroupUuid, uuid);
		}


		public static void AddContract(Contract data)
		{
			Contracts.Add(data);
			CallUpdate();
		}

		public static void RemoveContract(string uuid)
		{
			Contracts.RemoveAt(Contracts.FindIndex(gg => gg.Uuid == uuid));
			CallUpdate();
		}

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

		public static void RemoveSeason(string uuid)
		{
			Seasons.RemoveAt(Seasons.FindIndex(s => s.Uuid == uuid));
			if (Seasons.Count == 0) CreateDataInitPopup();
			else if (GetActiveSeasons().Count == 0) CreateSeasonInitPopup();
			CallUpdate();
		}

		public static void EditSeason(string uuid, Season data)
		{
			Seasons[Seasons.FindIndex(s => s.Uuid == uuid)] = data;
			CallUpdate();
		}

		public static void EndSeason(string uuid)
		{
			// Set end date to today
			Seasons[Seasons.FindIndex(s => s.Uuid == uuid)].EndDate = ((DateTimeOffset)DateTime.Today.ToLocalTime()).ToUnixTimeSeconds();
			CallUpdate();
		}


		public static void CreateSeasonInitPopup()
		{
			var editableSeasonPopup = (EditableSeasonPopupViewModel)ViewModelManager.ViewModels["EditableSeasonPopup"];
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];

			editableSeasonPopup.CanCancel = false;
			editableSeasonPopup.SetParameters("Create Season", false);

			mainVm.InterruptUpdate = true;
			mainVm.QueuePopup(editableSeasonPopup);
		}

		public static void CreateDataInitPopup()
		{
			var dataInitPopup = (DataInitPopupViewModel)ViewModelManager.ViewModels["DataInitPopup"];
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];

			dataInitPopup.InitData();
			dataInitPopup.CanCancel = false;
			mainVm.InterruptUpdate = true;

			mainVm.QueuePopup(dataInitPopup);
		}

		public static List<string> GetActiveSeasons()
		{
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;

			return (from s in Seasons where DateTimeOffset.FromUnixTimeSeconds(s.EndDate).ToLocalTime().Date > today select s.Uuid).ToList();
		}
	}
}
