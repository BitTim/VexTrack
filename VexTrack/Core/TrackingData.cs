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
		public string UUID { get; set; }
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
			UUID = uuid;
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
		public string UUID { get; set; }
		public long Date { get; set; }
		public string Status { get; set; }

		public StreakEntry(string uuid, long date, string status)
		{
			UUID = uuid;
			Date = date;
			Status = status;
		}
	}

	public class Goal
	{
		public string UUID { get; set; }
		public string Name { get; set; }
		public int Total { get; set; }
		public int Collected { get; set; }
		public string Color { get; set; }
		public string Dependency { get; set; }
		public bool Paused { get; set; }

		public Goal(string uuid, string name, int total, int collected, string color, string dependency, bool paused)
		{
			UUID = uuid;
			Name = name;
			Total = total;
			Collected = collected;
			Color = color;
			Dependency = dependency;
			Paused = paused;
		}
	}

	public class GoalGroup
	{
		public string UUID { get; set; }
		public string Name { get; set; }
		public List<Goal> Goals { get; set; }

		public GoalGroup(string uuid, string name, List<Goal> goals)
		{
			UUID = uuid;
			Name = name;
			Goals = goals;
		}
	}

	public class Season
	{
		public string UUID { get; set; }
		public string Name { get; set; }
		public string EndDate { get; set; }
		public int ActiveBPLevel { get; set; }
		public int CXP { get; set; }
		public List<HistoryEntry> History { get; set; }

		public Season(string uuid, string name, string endDate, int activeBPLevel, int cXP, List<HistoryEntry> history)
		{
			(UUID, Name, EndDate, ActiveBPLevel, CXP, History) = (uuid, name, endDate, activeBPLevel, cXP, history);
		}
	}

	public class TrackingData
	{
		public List<GoalGroup> Goals { get; set; }
		public List<StreakEntry> Streak { get; set; }
		public List<Season> Seasons { get; set; }

		public TrackingData(List<GoalGroup> goals, List<StreakEntry> streak, List<Season> seasons)
		{
			(Goals, Streak, Seasons) = (goals, streak, seasons);
		}
	}

	public static class TrackingDataHelper
	{
		public static TrackingData Data { get; set; }
		public static string CurrentSeasonUUID
		{
			get
			{
				if (Data != null) return Data.Seasons.Last().UUID;
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

		public static int GetRemainingDays(string sUUID = "", DateTimeOffset endDate = new(), bool overrideEndDate = false)
		{
			if (sUUID == "" && !overrideEndDate) return -1;

			if (!overrideEndDate) endDate = DateTimeOffset.Parse(Data.Seasons.Find(s => s.UUID == sUUID).EndDate).ToLocalTime().Date;
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;

			int remainingDays = (endDate - today).Days;
			if ((endDate - today).Hours > 12) { remainingDays += 1; }
			if (remainingDays < 0) remainingDays = 0;

			return remainingDays;
		}

		public static int GetDuration(string sUUID)
		{
			DateTimeOffset endDate = DateTimeOffset.Parse(Data.Seasons.Find(s => s.UUID == sUUID).EndDate).ToLocalTime().Date;
			DateTimeOffset startDate = DateTimeOffset.FromUnixTimeSeconds(Data.Seasons.Find(s => s.UUID == sUUID).History.First().Time).ToLocalTime().Date;

			int duration = (endDate - startDate).Days;
			if ((endDate - startDate).Hours > 12) { duration += 1; }
			if (duration < 0) duration = 0;

			return duration;
		}

		public static void InitData()
		{
			List<GoalGroup> goals = new();
			List<StreakEntry> streak = new();
			List<Season> seasons = new();

			Data = new TrackingData(goals, streak, seasons);
		}

		public static void ConvertData()
		{
			string rawJSON = File.ReadAllText(Constants.LegacyDataPath);
			JObject jo = JObject.Parse(rawJSON);

			List<GoalGroup> goals = new();
			goals.Add(new GoalGroup(Guid.NewGuid().ToString(), "No Group", new List<Goal>()));
			foreach (JObject goal in jo["goals"])
			{
				string gName = (string)goal["name"];
				int collected = CalcUtil.CalcTotalCollected((int)jo["activeBPLevel"], (int)jo["cXP"]) - (int)goal["startXP"];
				int total = collected + (int)goal["remaining"];
				string color = (string)goal["color"];

				goals[0].Goals.Add(new Goal(Guid.NewGuid().ToString(), gName, total, collected, color, "", false));
			}

			List<StreakEntry> streak = new();
			List<Season> seasons = new();

			string name = "First Season";
			string endDate = (string)jo["seasonEndDate"];
			int activeBPLevel = (int)jo["activeBPLevel"];
			int cXP = (int)jo["cXP"];

			List<HistoryEntry> history = new();
			foreach (JObject historyEntry in jo["history"])
			{
				long time = (long)historyEntry["time"];
				string description = (string)historyEntry["description"];
				int amount = (int)historyEntry["amount"];
				string map = (string)historyEntry["map"];

				if (map == null || map == "") map = Constants.Maps.Last();

				string gameMode, desc;
				int score, enemyScore;
				(gameMode, desc, score, enemyScore) = HistoryDataCalc.DescriptionToScores(description);

				history.Add(new HistoryEntry(Guid.NewGuid().ToString(), time, gameMode, amount, map, desc, score, enemyScore, false, false));
			}

			seasons.Add(new Season(Guid.NewGuid().ToString(), name, endDate, activeBPLevel, cXP, history));
			Data = new TrackingData(goals, streak, seasons);

			File.Move(Constants.LegacyDataPath, Constants.LegacyDataPath + ".bak");
			SaveData();



			EditableSeasonPopupViewModel EditableSeasonPopup = (EditableSeasonPopupViewModel)ViewModelManager.ViewModels["EditableSeasonPopup"];
			MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];

			EditableSeasonPopup.CanCancel = false;
			EditableSeasonPopup.SetParameters("Edit Season", true);
			EditableSeasonPopup.SetData(SeasonDataCalc.CalcSeason(seasons.Last(), false));

			MainVM.QueuePopup(EditableSeasonPopup);
		}

		public static void LoadData()
		{
			bool reSave = false;

			if (!File.Exists(Constants.DataPath) || File.ReadAllText(Constants.DataPath) == "")
			{
				if (File.Exists(Constants.LegacyDataPath))
				{
					ConvertData();
					return;
				}

				InitData();
				CreateDataInitPopup();
				return;
			}

			string rawJSON = File.ReadAllText(Constants.DataPath);
			JObject jo = JObject.Parse(rawJSON);

			List<Season> seasons = new();
			foreach (JObject season in jo["seasons"])
			{
				string sUUID = (string)season["uuid"];
				string name = (string)season["name"];
				string endDate = (string)season["endDate"];
				int activeBPLevel = (int)season["activeBPLevel"];
				int cXP = (int)season["cXP"];

				string historyKey = "history";
				if (season["history"] == null)
				{
					historyKey = "xpHistory";
				}

				List<HistoryEntry> history = new();
				foreach (JObject historyEntry in season[historyKey])
				{
					string hUUID = (string)historyEntry["uuid"];
					string gamemode = (string)historyEntry["gameMode"];
					long time = (long)historyEntry["time"];
					int amount = (int)historyEntry["amount"];
					string map = (string)historyEntry["map"];
					string description = (string)historyEntry["description"];

					// Fix previous typos
					if (gamemode == "Competetive")
					{
						gamemode = "Competitive";
						reSave = true;
					}

					if (gamemode == "Snowballfight")
					{
						gamemode = "Snowball Fight";
						reSave = true;
					}

					string gameMode, desc;
					int score, enemyScore;
					bool surrenderedWin, surrenderedLoss;

					if (gamemode is null or "Custom")
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

					if (hUUID == null) hUUID = Guid.NewGuid().ToString();
					history.Add(new HistoryEntry(hUUID, time, gameMode, amount, map, desc, score, enemyScore, surrenderedWin, surrenderedLoss));
				}

				if (sUUID == null) sUUID = Guid.NewGuid().ToString();
				seasons.Add(new Season(sUUID, name, endDate, activeBPLevel, cXP, history));
			}

			List<StreakEntry> streak = new();
			if (jo["streak"] != null)
			{
				foreach (JObject streakEntry in jo["streak"])
				{
					string uuid = (string)streakEntry["uuid"];
					long date = (long)streakEntry["date"];
					string status = (string)streakEntry["status"];

					if (uuid == null) uuid = Guid.NewGuid().ToString();
					streak.Add(new StreakEntry(uuid, date, status));
				}
			}
			streak = streak.OrderByDescending(t => t.Date).ToList();

			List<GoalGroup> goalGroups = new();
			foreach (JObject goalGroup in jo["goals"])
			{
				JToken source = goalGroup["goals"];
				bool convertToGrouped = false;
				if (goalGroup["goals"] == null)
				{
					source = jo["goals"];
					goalGroups.Add(new GoalGroup(Constants.DefaultGroupUUID, "No Group", new List<Goal>()));
					convertToGrouped = true;
				}

				List<Goal> goals = new();
				foreach (JObject goal in source)
				{
					string uuid = (string)goal["uuid"];
					string name = (string)goal["name"];
					string color = (string)goal["color"];

					int collected;
					int total;
					bool paused;
					string dependency;

					if (goal["total"] == null || goal["collected"] == null)
					{
						collected = CalcUtil.CalcTotalCollected(seasons.Last().ActiveBPLevel, seasons.Last().CXP) - (int)goal["startXP"];
						total = collected + (int)goal["remaining"];
						reSave = true;
					}
					else
					{
						total = (int)goal["total"];
						collected = (int)goal["collected"];
					}

					if (goal["paused"] == null)
					{
						paused = false;
						reSave = true;
					}
					else paused = (bool)goal["paused"];

					if (goal["dependency"] == null)
					{
						dependency = "";
						reSave = true;
					}
					else dependency = (string)goal["dependency"];

					if (uuid == null) uuid = Guid.NewGuid().ToString();
					goals.Add(new Goal(uuid, name, total, collected, color, dependency, paused));
				}

				if (!convertToGrouped)
				{
					string uuid = (string)goalGroup["uuid"];
					string name = (string)goalGroup["name"];
					goalGroups.Add(new GoalGroup(uuid, name, goals));
				}
				else
				{
					goalGroups[0].Goals = goals;
					break;
				}
			}

			if (seasons.Count == 0) CreateDataInitPopup();

			Data = new TrackingData(goalGroups, streak, seasons);
			if (reSave) SaveData();

			Recalculate();
		}

		public static void SaveData()
		{
			JObject jo = new();

			JArray goalGroups = new();
			foreach (GoalGroup goalGroup in Data.Goals)
			{
				JObject goalGroubObj = new();
				goalGroubObj.Add("uuid", goalGroup.UUID);
				goalGroubObj.Add("name", goalGroup.Name);

				JArray goals = new();
				foreach (Goal goal in goalGroup.Goals)
				{
					JObject goalObj = new();
					goalObj.Add("uuid", goal.UUID);
					goalObj.Add("name", goal.Name);
					goalObj.Add("total", goal.Total);
					goalObj.Add("collected", goal.Collected);
					goalObj.Add("color", goal.Color);
					goalObj.Add("dependency", goal.Dependency);
					goalObj.Add("paused", goal.Paused);

					goals.Add(goalObj);
				}

				goalGroubObj.Add("goals", goals);
				goalGroups.Add(goalGroubObj);
			}
			jo.Add("goals", goalGroups);

			Data.Streak = Data.Streak.OrderByDescending(t => t.Date).ToList();
			JArray streak = new();
			foreach (StreakEntry streakEntry in Data.Streak)
			{
				JObject streakEntryObj = new();
				streakEntryObj.Add("uuid", streakEntry.UUID);
				streakEntryObj.Add("date", streakEntry.Date);
				streakEntryObj.Add("status", streakEntry.Status);

				streak.Add(streakEntryObj);
			}
			jo.Add("streak", streak);

			JArray seasons = new();
			foreach (Season season in Data.Seasons)
			{
				JObject seasonObj = new();
				seasonObj.Add("uuid", season.UUID);
				seasonObj.Add("name", season.Name);
				seasonObj.Add("endDate", season.EndDate);
				seasonObj.Add("activeBPLevel", season.ActiveBPLevel);
				seasonObj.Add("cXP", season.CXP);

				JArray history = new();
				foreach (HistoryEntry historyEntry in season.History)
				{
					JObject historyEntryObj = new();
					historyEntryObj.Add("uuid", historyEntry.UUID);
					historyEntryObj.Add("gameMode", historyEntry.GameMode);
					historyEntryObj.Add("time", historyEntry.Time);
					historyEntryObj.Add("amount", historyEntry.Amount);
					historyEntryObj.Add("map", historyEntry.Map);
					historyEntryObj.Add("description", historyEntry.Description);
					historyEntryObj.Add("score", historyEntry.Score);
					historyEntryObj.Add("enemyScore", historyEntry.EnemyScore);
					historyEntryObj.Add("surrenderedWin", historyEntry.SurrenderedWin);
					historyEntryObj.Add("surrenderedLoss", historyEntry.SurrenderedLoss);

					history.Add(historyEntryObj);
				}

				seasonObj.Add("history", history);
				seasons.Add(seasonObj);
			}

			jo.Add("seasons", seasons);

			if (!File.Exists(Constants.DataPath))
			{
				int sep = Constants.DataPath.LastIndexOf("/");

				Directory.CreateDirectory(Constants.DataPath.Substring(0, sep));
				File.CreateText(Constants.DataPath).Close();
			}

			File.WriteAllText(Constants.DataPath, jo.ToString());
		}

		public static void Recalculate()
		{
			List<string> completed = new();
			List<string> lost = new();

			//Get Completed goals
			List<string> completedGoals = new();
			foreach (GoalGroup gg in Data.Goals)
				foreach (Goal g in gg.Goals)
					if (g.Collected >= g.Total) completedGoals.Add(g.UUID);

			//Recalculate total collected XP, collected XP in level and current level
			int prevTotalXP = CalcUtil.CalcTotalCollected(CurrentSeasonData.ActiveBPLevel, CurrentSeasonData.CXP);

			int cxp = 0;
			int iter = 2;

			foreach (HistoryEntry he in TrackingDataHelper.CurrentSeasonData.History)
				cxp += he.Amount;

			while (cxp >= 0)
			{
				if (iter <= Constants.BattlepassLevels)
					cxp -= Constants.Level2Offset + (iter * Constants.XPPerLevel);
				else if (iter < Constants.BattlepassLevels + Constants.EpilogueLevels + 2)
					cxp -= Constants.XPPerEpilogueLevel;
				else
					break;
				iter++;
			}
			iter--;

			for (int i = CurrentSeasonData.ActiveBPLevel - 1; i > iter - 1; i--)
				lost.Add("Battlepass Level " + i.ToString());

			for (int i = CurrentSeasonData.ActiveBPLevel; i < iter; i++)
				completed.Add("Battlepass Level " + i.ToString());

			CurrentSeasonData.ActiveBPLevel = iter;

			if (iter < Constants.BattlepassLevels)
				cxp += Constants.Level2Offset + (iter * Constants.XPPerLevel);
			else if (iter < Constants.BattlepassLevels + Constants.EpilogueLevels + 2)
				cxp += Constants.XPPerEpilogueLevel;

			CurrentSeasonData.CXP = cxp;

			//Calculate difference in XP and apply to goals
			int currTotalXP = CalcUtil.CalcTotalCollected(CurrentSeasonData.ActiveBPLevel, CurrentSeasonData.CXP);
			int deltaXP = currTotalXP - prevTotalXP;

			for (int pass = 1; pass <= 2; pass++)
			{
				foreach (GoalGroup gg in Data.Goals)
				{
					foreach (Goal g in gg.Goals)
					{
						int appliedXP = deltaXP;
						bool hasDep = false;

						if (g.Paused) continue;
						if (g.Dependency != "")
						{
							if (pass != 2) continue;

							Goal dep = gg.Goals.Find(x => x.UUID == g.Dependency);
							if (dep != null)
							{
								hasDep = true;

								if (deltaXP > 0)
								{
									int depRemaining = dep.Total - (dep.Collected - deltaXP);
									if (depRemaining < 0) depRemaining = 0;

									appliedXP = deltaXP - depRemaining;
									if (appliedXP < 0) appliedXP = 0;
								}
								else
								{
									if (deltaXP * -1 >= g.Collected) appliedXP = g.Collected * -1;
								}
							}
						}

						if (pass == 2 && !hasDep) continue;

						int newCollected = g.Collected + appliedXP;
						if (newCollected < 0)
						{
							g.Total += -newCollected;
							newCollected = 0;
						}

						g.Collected = newCollected;

						if (g.Collected >= g.Total && !completedGoals.Contains(g.UUID)) completed.Add(g.Name);
						if (g.Collected < g.Total && completedGoals.Contains(g.UUID)) lost.Add(g.Name);
					}
				}
			}

			if (completed.Count != 0 || lost.Count != 0)
			{
				MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
				ProgressActivityPopupViewModel PAPopupVM = (ProgressActivityPopupViewModel)ViewModelManager.ViewModels["PAPopup"];

				PAPopupVM.SetData(completed, lost);
				MainVM.QueuePopup(PAPopupVM);
			}
		}

		public static void CallUpdate()
		{
			for (int i = 0; i < Data.Seasons.Count; i++)
			{
				List<HistoryEntry> SortedHistory = Data.Seasons[i].History.OrderBy(h => h.Time).ToList();
				Data.Seasons[i].History = SortedHistory;
			}

			SaveData();
			MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			MainVM.Update();
		}

		public static void ResetData()
		{
			InitData();
			SaveData();
			LoadData();
			CallUpdate();
		}



		public static HistoryEntry GetHistoryEntry(string sUUID, string hUUID)
		{
			return Data.Seasons.Find(s => s.UUID == sUUID).History.Find(h => h.UUID == hUUID);
		}

		public static HistoryEntry GetFirstHistoryEntry(string sUUID)
		{
			return Data.Seasons.Find(s => s.UUID == sUUID).History.First();
		}

		public static HistoryEntry GetLastHistoryEntry(string sUUID)
		{
			return Data.Seasons.Find(s => s.UUID == sUUID).History.Last();
		}

		public static Season GetSeason(string sUUID)
		{
			return Data.Seasons.Find(s => s.UUID == sUUID);
		}


		public static void AddHistoryEntry(string sUUID, HistoryEntry data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)].History.Add(data);
			Recalculate();
			CallUpdate();
		}

		public static void RemoveHistoryEntry(string sUUID, string hUUID)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History.RemoveAt(Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History.FindIndex(he => he.UUID == hUUID));

			Recalculate();
			CallUpdate();
		}

		public static void EditHistoryEntry(string sUUID, string hUUID, HistoryEntry data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History[Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History.FindIndex(he => he.UUID == hUUID)] = data;

			HistoryViewModel HistoryVM = (HistoryViewModel)ViewModelManager.ViewModels["History"];
			HistoryVM.EditEntry(new HistoryEntryData(sUUID, hUUID, data.GameMode, data.Time, data.Amount, data.Map, HistoryDataCalc.CalcHistoryResultFromScores(Constants.ScoreTypes[data.GameMode], data.Description, data.Score, data.EnemyScore, data.SurrenderedWin, data.SurrenderedLoss), data.Description, data.Score, data.EnemyScore, data.SurrenderedWin, data.SurrenderedLoss));

			Recalculate();
			CallUpdate();
		}



		public static void AddGoal(string groupUUID, Goal data)
		{
			Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == groupUUID)].Goals.Add(data);
			CallUpdate();
		}

		public static void RemoveGoal(string groupUUID, string uuid)
		{
			Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == groupUUID)]
				.Goals.RemoveAt(Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == groupUUID)]
				.Goals.FindIndex(g => g.UUID == uuid));
			CallUpdate();
		}

		public static void EditGoal(string groupUUID, string uuid, Goal data)
		{
			int index = Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == groupUUID)]
							.Goals.FindIndex(g => g.UUID == uuid);
			if (index >= 0)
			{
				Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == groupUUID)]
					.Goals[index] = data;
				CallUpdate();
				return;
			}

			string prevGroupUUID = Data.Goals.Where(gg => gg.Goals.Any(g => g.UUID == uuid)).First().UUID;
			MoveGoal(prevGroupUUID, groupUUID, uuid, true);
		}

		public static void MoveGoal(string srcGroupUUID, string dstGroupUUID, string uuid, bool deleteGoalFromGroup = false)
		{
			Goal goal = Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == srcGroupUUID)]
							.Goals[Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == srcGroupUUID)]
							.Goals.FindIndex(g => g.UUID == uuid)];

			AddGoal(dstGroupUUID, goal);
			if (deleteGoalFromGroup) RemoveGoal(srcGroupUUID, uuid);
		}


		public static void AddGoalGroup(GoalGroup data)
		{
			Data.Goals.Add(data);
			CallUpdate();
		}

		public static void RemoveGoalGroup(string uuid)
		{
			Data.Goals.RemoveAt(Data.Goals.FindIndex(gg => gg.UUID == uuid));
			CallUpdate();
		}

		public static void EditGoalGroup(string uuid, string name)
		{
			Data.Goals[Data.Goals.FindIndex(gg => gg.UUID == uuid)].Name = name;
			CallUpdate();
		}



		public static void AddSeason(Season data)
		{
			Data.Seasons.Add(data);
			CallUpdate();
		}

		public static void RemoveSeason(string uuid)
		{
			Data.Seasons.RemoveAt(Data.Seasons.FindIndex(s => s.UUID == uuid));
			if (Data.Seasons.Count == 0) CreateDataInitPopup();
			else if (GetActiveSeasons().Count == 0) CreateSeasonInitPopup();
			CallUpdate();
		}

		public static void EditSeason(string uuid, Season data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == uuid)] = data;
			CallUpdate();
		}

		public static void EndSeason(string uuid)
		{
			// Set end date to today
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == uuid)].EndDate = DateTime.Today.ToLocalTime().Date.ToString("d");
			CallUpdate();
		}


		public static void CreateSeasonInitPopup()
		{
			EditableSeasonPopupViewModel EditableSeasonPopup = (EditableSeasonPopupViewModel)ViewModelManager.ViewModels["EditableSeasonPopup"];
			MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];

			EditableSeasonPopup.CanCancel = false;
			EditableSeasonPopup.SetParameters("Create Season", false);

			MainVM.InterruptUpdate = true;
			MainVM.QueuePopup(EditableSeasonPopup);
		}

		public static void CreateDataInitPopup()
		{
			DataInitPopupViewModel DataInitPopup = (DataInitPopupViewModel)ViewModelManager.ViewModels["DataInitPopup"];
			MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];

			DataInitPopup.InitData();
			DataInitPopup.CanCancel = false;
			MainVM.InterruptUpdate = true;

			MainVM.QueuePopup(DataInitPopup);
		}

		public static List<string> GetActiveSeasons()
		{
			List<string> activeSeasons = new();
			DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;

			foreach (Season s in Data.Seasons)
			{
				if (DateTimeOffset.Parse(s.EndDate).ToLocalTime().Date > today) activeSeasons.Add(s.UUID);
			}

			return activeSeasons;
		}
	}
}
