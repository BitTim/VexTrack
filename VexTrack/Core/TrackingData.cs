using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core
{
	public class HistoryEntry
	{
		public string Uuid { get; set; }
		public long Time { get; set; }
		public string GameMode { get; set; }
		public int Amount { get; set; }
		public string Map { get; set; }
		public string Description { get; set; }
		public int Score { get; set; }
		public int EnemyScore { get; set; }
		public bool SurrenderedWin { get; set; }
		public bool SurrenderedLoss { get; set; }

		public HistoryEntry(string uuid, long time, string gamemode, int amount, string map, string desc, int score, int enemyScore, bool surrenderedWin, bool surrenderedLoss)
		{
			Uuid = uuid;
			Time = time;
			GameMode = gamemode;
			Amount = amount;
			Map = map;
			Description = desc;
			Score = score;
			EnemyScore = enemyScore;
			SurrenderedWin = surrenderedWin;
			SurrenderedLoss = surrenderedLoss;
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

	public class Goal
	{
		public string Uuid { get; set; }
		public string Name { get; set; }
		public int Total { get; set; }
		public int Collected { get; set; }
		//public string Color { get; set; }
		//public string Dependency { get; set; }
		//public bool Paused { get; set; }

		public Goal(string uuid, string name, int total, int collected)
		{
			Uuid = uuid;
			Name = name;
			Total = total;
			Collected = collected;
			//Color = color;
			//Dependency = dependency;
			//Paused = paused;
		}
	}

	public class Contract
	{
		public string Uuid { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public bool Paused { get; set; }
		public List<Goal> Goals { get; set; }

		public Contract(string uuid, string name, string color, bool paused, List<Goal> goals)
		{
			Uuid = uuid;
			Name = name;
			Color = color;
			Paused = paused;
			Goals = goals;
		}
	}

	public class Season
	{
		public string Uuid { get; set; }
		public string Name { get; set; }
		public string EndDate { get; set; }
		public int ActiveBpLevel { get; set; }
		public int Cxp { get; set; }
		public List<HistoryEntry> History { get; set; }

		public Season(string uuid, string name, string endDate, int activeBpLevel, int cXp, List<HistoryEntry> history)
		{
			(Uuid, Name, EndDate, ActiveBpLevel, Cxp, History) = (uuid, name, endDate, activeBpLevel, cXp, history);
		}
	}

	public class TrackingData
	{
		public List<Contract> Contracts { get; set; }
		public List<StreakEntry> Streak { get; set; }
		public List<Season> Seasons { get; set; }

		public TrackingData(List<Contract> contracts, List<StreakEntry> streak, List<Season> seasons)
		{
			(Contracts, Streak, Seasons) = (contracts, streak, seasons);
		}
	}

	public static class TrackingDataHelper
	{
		public static TrackingData Data { get; set; }
		public static string CurrentSeasonUuid
		{
			get
			{
				if (Data != null) return Data.Seasons.Last().Uuid;
				else return null;
			}
		}
		public static Season CurrentSeasonData
		{
			get
			{
				if (Data != null) return Data.Seasons.Last();
				else return null;
			}
		}

		public static int GetRemainingDays(string sUuid = "", DateTimeOffset endDate = new(), bool overrideEndDate = false)
		{
			if (sUuid == "" && !overrideEndDate) return -1;

			if (!overrideEndDate) endDate = DateTimeOffset.Parse(Data.Seasons.Find(s => s.Uuid == sUuid).EndDate).ToLocalTime().Date;
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;

			var remainingDays = (endDate - today).Days;
			if ((endDate - today).Hours > 12) { remainingDays += 1; }
			if (remainingDays < 0) remainingDays = 0;

			return remainingDays;
		}

		public static int GetDuration(string sUuid)
		{
			DateTimeOffset endDate = DateTimeOffset.Parse(Data.Seasons.Find(s => s.Uuid == sUuid).EndDate).ToLocalTime().Date;
			DateTimeOffset startDate = DateTimeOffset.FromUnixTimeSeconds(Data.Seasons.Find(s => s.Uuid == sUuid).History.First().Time).ToLocalTime().Date;

			var duration = (endDate - startDate).Days;
			if ((endDate - startDate).Hours > 12) { duration += 1; }
			if (duration < 0) duration = 0;

			return duration;
		}
		
		

		// ================================
		//  Init and Convert
		// ================================
		
		public static void InitData()
		{
			List<Contract> goals = new();
			List<StreakEntry> streak = new();
			List<Season> seasons = new();

			Data = new TrackingData(goals, streak, seasons);
		}

		public static void ConvertFromLegacyData()
		{
			var rawJson = File.ReadAllText(Constants.LegacyDataPath);
			var jo = JObject.Parse(rawJson);

			List<Contract> contracts = new();
			foreach (JObject contract in jo["goals"])
			{
				var gName = (string)contract["name"];
				var collected = CalcUtil.CalcTotalCollected((int)jo["activeBPLevel"], (int)jo["cXP"]) - (int)contract["startXP"];
				var total = collected + (int)contract["remaining"];
				var color = (string)contract["color"];

				contracts.Add(new Contract(Guid.NewGuid().ToString(), gName, color, false, new List<Goal>()));
				contracts.Last().Goals.Add(new Goal(Guid.NewGuid().ToString(), gName, total, collected));
			}

			List<StreakEntry> streak = new();
			List<Season> seasons = new();

			var name = "First Season";
			var endDate = (string)jo["seasonEndDate"];
			var activeBpLevel = (int)jo["activeBPLevel"];
			var cXp = (int)jo["cXP"];

			List<HistoryEntry> history = new();
			foreach (JObject historyEntry in jo["history"])
			{
				var time = (long)historyEntry["time"];
				var description = (string)historyEntry["description"];
				var amount = (int)historyEntry["amount"];
				var map = (string)historyEntry["map"];

				if (string.IsNullOrEmpty(map)) map = Constants.Maps.Last();
				var (gameMode, desc, score, enemyScore) = HistoryDataCalc.DescriptionToScores(description);

				history.Add(new HistoryEntry(Guid.NewGuid().ToString(), time, gameMode, amount, map, desc, score, enemyScore, false, false));
			}

			seasons.Add(new Season(Guid.NewGuid().ToString(), name, endDate, activeBpLevel, cXp, history));
			Data = new TrackingData(contracts, streak, seasons);

			File.Move(Constants.LegacyDataPath, Constants.LegacyDataPath + ".bak");
			SaveData();

			var editableSeasonPopup = (EditableSeasonPopupViewModel)ViewModelManager.ViewModels["EditableSeasonPopup"];
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];

			editableSeasonPopup.CanCancel = false;
			editableSeasonPopup.SetParameters("Edit Season", true);
			editableSeasonPopup.SetData(SeasonDataCalc.CalcSeason(seasons.Last(), false));

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
						(gameMode, desc, score, enemyScore) = HistoryDataCalc.DescriptionToScores(description);
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
					history.Add(new HistoryEntry(hUuid, time, gameMode, amount, map, desc, score, enemyScore, surrenderedWin, surrenderedLoss));
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

					goalUuid ??= Guid.NewGuid().ToString();
					var goalObj = new Goal(goalUuid, goalName, total, collected);
					
					if(!convertToGrouped) goals.Add(goalObj);
					else contracts.Add(new Contract(Guid.NewGuid().ToString(), goalName,
							(string)goal["color"], (bool)goal["paused"], new List<Goal> { goalObj }));
				}

				if (convertToGrouped) return contracts;
				
				var uuid = (string)contract["uuid"];
				var name = (string)contract["name"];
				var color = (string)contract["goals"]?["color"];
				var paused = (bool)contract["goals"]?["paused"];
				contracts.Add(new Contract(uuid, name, color, paused, goals));
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
					seasons = LoadSeasonsV1(jo);
					streak = LoadStreakV1(jo);
					contracts = LoadContractsV2(jo);
					break;
			}

			if (seasons.Count == 0) CreateDataInitPopup();

			Data = new TrackingData(contracts, streak, seasons);
			Recalculate();
		}
		
		
		
		// ================================
		//  Saving
		// ================================

		public static void SaveData()
		{
			JObject jo = new();
			jo.Add("version", Constants.DataVersion);
			
			JArray goalGroups = new();
			foreach (var contract in Data.Contracts)
			{
				JObject contractObj = new()
				{
					{ "uuid", contract.Uuid },
					{ "name", contract.Name },
					{ "color", contract.Color },
					{ "paused", contract.Paused }
				};

				JArray goals = new();
				foreach (var goal in contract.Goals)
				{
					JObject goalObj = new()
					{
						{ "uuid", goal.Uuid },
						{ "name", goal.Name },
						{ "total", goal.Total },
						{ "collected", goal.Collected }
					};

					goals.Add(goalObj);
				}

				contractObj.Add("goals", goals);
				goalGroups.Add(contractObj);
			}
			jo.Add("goals", goalGroups);

			Data.Streak = Data.Streak.OrderByDescending(t => t.Date).ToList();
			JArray streak = new();
			foreach (var streakEntry in Data.Streak)
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
			foreach (var season in Data.Seasons)
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
				foreach (var historyEntry in season.History)
				{
					JObject historyEntryObj = new()
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
					};

					history.Add(historyEntryObj);
				}

				seasonObj.Add("history", history);
				seasons.Add(seasonObj);
			}

			jo.Add("seasons", seasons);

			if (!File.Exists(Constants.DataPath))
			{
				var sep = Constants.DataPath.LastIndexOf("/");

				Directory.CreateDirectory(Constants.DataPath.Substring(0, sep));
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
			List<string> completedGoals = new();
			foreach (var gg in Data.Contracts)
				foreach (var g in gg.Goals)
					if (g.Collected >= g.Total) completedGoals.Add(g.Uuid);

			//Recalculate total collected XP, collected XP in level and current level
			var prevTotalXp = CalcUtil.CalcTotalCollected(CurrentSeasonData.ActiveBpLevel, CurrentSeasonData.Cxp);

			var cxp = 0;
			var iter = 2;

			foreach (var he in TrackingDataHelper.CurrentSeasonData.History)
				cxp += he.Amount;

			while (cxp >= 0)
			{
				if (iter <= Constants.BattlepassLevels)
					cxp -= Constants.Level2Offset + (iter * Constants.XpPerLevel);
				else if (iter < Constants.BattlepassLevels + Constants.EpilogueLevels + 2)
					cxp -= Constants.XpPerEpilogueLevel;
				else
					break;
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

			for (var pass = 1; pass <= 2; pass++)
			{
				foreach (var contract in Data.Contracts)
				{
					foreach (var goal in contract.Goals)
					{
						var appliedXp = deltaXp;
						var hasDep = false;
						
						if (pass != 2) continue;

						// TODO: Remove Dep
						var dep = contract.Goals.Find(x => x.Uuid == goal.Dependency);
						if (dep != null)
						{
							hasDep = true;

							if (deltaXp > 0)
							{
								var depRemaining = dep.Total - (dep.Collected - deltaXp);
								if (depRemaining < 0) depRemaining = 0;

								appliedXp = deltaXp - depRemaining;
								if (appliedXp < 0) appliedXp = 0;
							}
							else
							{
								if (deltaXp * -1 >= goal.Collected) appliedXp = goal.Collected * -1;
							}
						}

						if (pass == 2 && !hasDep) continue;

						var newCollected = goal.Collected + appliedXp;
						if (newCollected < 0)
						{
							goal.Total += -newCollected;
							newCollected = 0;
						}

						goal.Collected = newCollected;

						if (goal.Collected >= goal.Total && !completedGoals.Contains(goal.Uuid)) completed.Add(goal.Name);
						if (goal.Collected < goal.Total && completedGoals.Contains(goal.Uuid)) lost.Add(goal.Name);
					}
				}
			}

			if (completed.Count != 0 || lost.Count != 0)
			{
				var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
				var paPopupVm = (ProgressActivityPopupViewModel)ViewModelManager.ViewModels["PAPopup"];

				paPopupVm.SetData(completed, lost);
				mainVm.QueuePopup(paPopupVm);
			}
		}

		public static void CallUpdate()
		{
			for (var i = 0; i < Data.Seasons.Count; i++)
			{
				var sortedHistory = Data.Seasons[i].History.OrderBy(h => h.Time).ToList();
				Data.Seasons[i].History = sortedHistory;
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



		public static HistoryEntry GetHistoryEntry(string sUuid, string hUuid)
		{
			return Data.Seasons.Find(s => s.Uuid == sUuid).History.Find(h => h.Uuid == hUuid);
		}

		public static HistoryEntry GetFirstHistoryEntry(string sUuid)
		{
			return Data.Seasons.Find(s => s.Uuid == sUuid).History.First();
		}

		public static HistoryEntry GetLastHistoryEntry(string sUuid)
		{
			return Data.Seasons.Find(s => s.Uuid == sUuid).History.Last();
		}

		public static Season GetSeason(string sUuid)
		{
			return Data.Seasons.Find(s => s.Uuid == sUuid);
		}


		public static void AddHistoryEntry(string sUuid, HistoryEntry data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.Uuid == sUuid)].History.Add(data);
			Recalculate();
			CallUpdate();
		}

		public static void RemoveHistoryEntry(string sUuid, string hUuid)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.Uuid == sUuid)]
				.History.RemoveAt(Data.Seasons[Data.Seasons.FindIndex(s => s.Uuid == sUuid)]
				.History.FindIndex(he => he.Uuid == hUuid));

			Recalculate();
			CallUpdate();
		}

		public static void EditHistoryEntry(string sUuid, string hUuid, HistoryEntry data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.Uuid == sUuid)]
				.History[Data.Seasons[Data.Seasons.FindIndex(s => s.Uuid == sUuid)]
				.History.FindIndex(he => he.Uuid == hUuid)] = data;

			var historyVm = (HistoryViewModel)ViewModelManager.ViewModels["History"];
			historyVm.EditEntry(new HistoryEntryData(sUuid, hUuid, data.GameMode, data.Time, data.Amount, data.Map, HistoryDataCalc.CalcHistoryResultFromScores(Constants.ScoreTypes[data.GameMode], data.Score, data.EnemyScore, data.SurrenderedWin, data.SurrenderedLoss), data.Description, data.Score, data.EnemyScore, data.SurrenderedWin, data.SurrenderedLoss));

			Recalculate();
			CallUpdate();
		}



		public static void AddGoal(string contractUuid, Goal data)
		{
			Data.Contracts[Data.Contracts.FindIndex(c => c.Uuid == contractUuid)].Goals.Add(data);
			CallUpdate();
		}

		public static void RemoveGoal(string contractUuid, string uuid)
		{
			Data.Contracts[Data.Contracts.FindIndex(c => c.Uuid == contractUuid)]
				.Goals.RemoveAt(Data.Contracts[Data.Contracts.FindIndex(c => c.Uuid == contractUuid)]
				.Goals.FindIndex(g => g.Uuid == uuid));
			CallUpdate();
		}

		public static void EditGoal(string contractUuid, string uuid, Goal data)
		{
			var index = Data.Contracts[Data.Contracts.FindIndex(c => c.Uuid == contractUuid)]
							.Goals.FindIndex(g => g.Uuid == uuid);
			if (index >= 0)
			{
				Data.Contracts[Data.Contracts.FindIndex(c => c.Uuid == contractUuid)]
					.Goals[index] = data;
				CallUpdate();
				return;
			}

			var prevGroupUuid = Data.Contracts.First(c => c.Goals.Any(g => g.Uuid == uuid)).Uuid;
			MoveGoal(prevGroupUuid, contractUuid, uuid, true);
		}

		// TODO: Evaluate if this is still needed
		public static void MoveGoal(string srcGroupUuid, string dstGroupUuid, string uuid, bool deleteGoalFromGroup = false)
		{
			var goal = Data.Contracts[Data.Contracts.FindIndex(gg => gg.Uuid == srcGroupUuid)]
							.Goals[Data.Contracts[Data.Contracts.FindIndex(gg => gg.Uuid == srcGroupUuid)]
							.Goals.FindIndex(g => g.Uuid == uuid)];

			AddGoal(dstGroupUuid, goal);
			if (deleteGoalFromGroup) RemoveGoal(srcGroupUuid, uuid);
		}


		public static void AddContract(Contract data)
		{
			Data.Contracts.Add(data);
			CallUpdate();
		}

		public static void RemoveContract(string uuid)
		{
			Data.Contracts.RemoveAt(Data.Contracts.FindIndex(gg => gg.Uuid == uuid));
			CallUpdate();
		}

		public static void EditContracts(string uuid, string name)
		{
			Data.Contracts[Data.Contracts.FindIndex(gg => gg.Uuid == uuid)].Name = name;
			CallUpdate();
		}



		public static void AddSeason(Season data)
		{
			Data.Seasons.Add(data);
			CallUpdate();
		}

		public static void RemoveSeason(string uuid)
		{
			Data.Seasons.RemoveAt(Data.Seasons.FindIndex(s => s.Uuid == uuid));
			if (Data.Seasons.Count == 0) CreateDataInitPopup();
			else if (GetActiveSeasons().Count == 0) CreateSeasonInitPopup();
			CallUpdate();
		}

		public static void EditSeason(string uuid, Season data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.Uuid == uuid)] = data;
			CallUpdate();
		}

		public static void EndSeason(string uuid)
		{
			// Set end date to today
			Data.Seasons[Data.Seasons.FindIndex(s => s.Uuid == uuid)].EndDate = DateTime.Today.ToLocalTime().Date.ToString("d");
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
			List<string> activeSeasons = new();
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;

			foreach (var s in Data.Seasons)
			{
				if (DateTimeOffset.Parse(s.EndDate).ToLocalTime().Date > today) activeSeasons.Add(s.Uuid);
			}

			return activeSeasons;
		}
	}
}
