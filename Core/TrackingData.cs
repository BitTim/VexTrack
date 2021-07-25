﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.Core
{
	public class HistoryEntry
	{
		public string UUID { get; set; }
		public long Time { get; set; }
		public string Description { get; set; }
		public int Amount { get; set; }

		public string Map { get; set; }

		public HistoryEntry(string uuid, long time, string desc, int amount, string map)
		{
			(UUID, Time, Description, Amount, Map) = (uuid, time, desc, amount, map);
		}
	}

	public class Goal
	{
		public string UUID { get; set; }
		public string Name { get; set; }
		public int Remaining { get; set; }
		public int StartXP { get; set; }
		public string Color { get; set; }

		public Goal(string uuid, string name, int remaining, int startXP, string color)
		{
			(UUID, Name, Remaining, StartXP, Color) = (uuid, name, remaining, startXP, color);
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
		public List<Goal> Goals { get; set; }
		public List<Season> Seasons { get; set; }

		public TrackingData(List<Goal> goals, List<Season> seasons)
		{
			(Goals, Seasons) = (goals, seasons);
		}
	}

	public static class TrackingDataHelper
	{
		public static TrackingData Data { get; set; }
		public static string CurrentSeasonUUID { get => Data.Seasons.Last().UUID; }
		public static Season CurrentSeasonData { get => Data.Seasons.Last(); }

		public static void InitData(string seasonName, string seasonEndDate, int activeBPLevel, int cXP)
		{
			List<Goal> goals = new();
			List<Season> seasons = new();

			List<HistoryEntry> history = new();
			history.Add(new HistoryEntry(Guid.NewGuid().ToString(), DateTimeOffset.Now.ToUnixTimeSeconds(), "Initialization", cXP, null));
			seasons.Add(new Season(Guid.NewGuid().ToString(), seasonName, seasonEndDate, activeBPLevel, cXP, history));

			Data = new TrackingData(goals, seasons);
			SaveData();
		}

		public static void ConvertData()
		{
			string rawJSON = File.ReadAllText(Constants.LegacyDataPath);
			JObject jo = JObject.Parse(rawJSON);

			List<Goal> goals = new();
			foreach (JObject goal in jo["goals"])
			{
				string gName = (string)goal["name"];
				int remaining = (int)goal["remaining"];
				int startXP = (int)goal["startXP"];
				string color = (string)goal["color"];

				goals.Add(new Goal(Guid.NewGuid().ToString(), gName, remaining, startXP, color));
			}

			List<Season> seasons = new();

			//TODO: Display dialog before setting Season Name
			string name = "First Season";
			string endDate = (string)jo["endDate"];
			int activeBPLevel = (int)jo["activeBPLevel"];
			int cXP = (int)jo["cXP"];

			List<HistoryEntry> history = new();
			foreach (JObject historyEntry in jo["history"])
			{
				long time = (long)historyEntry["time"];
				string description = (string)historyEntry["description"];
				int amount = (int)historyEntry["amount"];
				string map = (string)historyEntry["map"];

				history.Add(new HistoryEntry(Guid.NewGuid().ToString(), time, description, amount, map));
			}

			seasons.Add(new Season(Guid.NewGuid().ToString(), name, endDate, activeBPLevel, cXP, history));
			Data = new TrackingData(goals, seasons);

			File.Move(Constants.LegacyDataPath, Constants.LegacyDataPath + ".bak");
			SaveData();
		}

		public static void LoadData()
		{
			if (!File.Exists(Constants.DataPath) || File.ReadAllText(Constants.DataPath) == "")
			{
				if (File.Exists(Constants.LegacyDataPath))
				{
					ConvertData();
					return;
				}

				InitData("First Season", "01.01.1970", 2, 0);
				return;
			}

			string rawJSON = File.ReadAllText(Constants.DataPath);
			JObject jo = JObject.Parse(rawJSON);

			List<Goal> goals = new();
			foreach(JObject goal in jo["goals"]) {
				string uuid = (string)goal["uuid"];
				string name = (string)goal["name"];
				int remaining = (int)goal["remaining"];
				int startXP = (int)goal["startXP"];
				string color = (string)goal["color"];

				if (uuid == null) uuid = Guid.NewGuid().ToString();
				goals.Add(new Goal(uuid, name, remaining, startXP, color));
			}

			List<Season> seasons = new();
			foreach (JObject season in jo["seasons"]) {
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
				foreach(JObject historyEntry in season[historyKey])
				{
					string hUUID = (string)historyEntry["uuid"];
					string description = (string)historyEntry["description"];
					long time = (long)historyEntry["time"];
					int amount = (int)historyEntry["amount"];
					string map = (string)historyEntry["map"];

					if (hUUID == null) hUUID = Guid.NewGuid().ToString();
					history.Add(new HistoryEntry(hUUID, time, description, amount, map));
				}

				if (sUUID == null) sUUID = Guid.NewGuid().ToString();
				seasons.Add(new Season(sUUID, name, endDate, activeBPLevel, cXP, history));
			}

			Data = new TrackingData(goals, seasons);
		}

		public static void SaveData()
		{
			JObject jo = new();

			JArray goals = new();
			foreach(Goal goal in Data.Goals)
			{
				JObject goalObj = new();
				goalObj.Add("uuid", goal.UUID);
				goalObj.Add("name", goal.Name);
				goalObj.Add("remaining", goal.Remaining);
				goalObj.Add("startXP", goal.StartXP);
				goalObj.Add("color", goal.Color);

				goals.Add(goalObj);
			}

			jo.Add("goals", goals);

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
					historyEntryObj.Add("description", historyEntry.Description);
					historyEntryObj.Add("time", historyEntry.Time);
					historyEntryObj.Add("amount", historyEntry.Amount);
					historyEntryObj.Add("map", historyEntry.Map);

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




		public static HistoryEntry GetHistoryEntry(int season, int index)
		{
			return Data.Seasons[season].History[index];
		}

		public static void AddHistoryEntry(string sUUID, HistoryEntry data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)].History.Add(data);
			CallUpdate();
		}

		public static void RemoveHistoryEntry(string sUUID, string hUUID)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History.RemoveAt(Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History.FindIndex(he => he.UUID == hUUID));
			CallUpdate();
		}

		public static void EditHistoryEntry(string sUUID, string hUUID, HistoryEntry data)
		{
			Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History[Data.Seasons[Data.Seasons.FindIndex(s => s.UUID == sUUID)]
				.History.FindIndex(he => he.UUID == hUUID)] = data;
			CallUpdate();
		}



		public static void AddFoal(Goal data)
		{
			Data.Goals.Add(data);
			CallUpdate();
		}

		public static void RemoveGoal(string uuid)
		{
			Data.Goals.RemoveAt(Data.Goals.FindIndex(g => g.UUID == uuid));
			CallUpdate();
		}

		public static void EditHistoryEntry(string uuid, Goal data)
		{
			Data.Goals[Data.Goals.FindIndex(g => g.UUID == uuid)] = data;
			CallUpdate();
		}
	}
}
