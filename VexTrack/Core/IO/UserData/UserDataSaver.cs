﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Model;

namespace VexTrack.Core.IO.UserData;

public static class UserDataSaver
{
    internal static void SaveUserData(int streak, long lastStreakUpdateTimestamp, List<Contract> contracts, List<Season> seasons, List<HistoryGroup> history)
	{
		JObject jo = new()
		{
			{ "version", Constants.DataVersion },
			{ "streak", streak },
			{ "lastStreakUpdate", lastStreakUpdateTimestamp }
		};

		JArray contractsArr = new();
		foreach (var contract in contracts)
		{
			JObject contractObj = new()
			{
				{ "uuid", contract.Uuid },
				{ "name", contract.Name },
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
			contractsArr.Add(contractObj);
		}
		jo.Add("contracts", contractsArr);

		JArray seasonsArr = new();
		foreach (var season in seasons)
		{
			JObject seasonObj = new()
			{
				{ "uuid", season.Uuid },
				{ "name", season.Name },
				{ "startTimestamp", season.StartTimestamp },
				{ "endTimestamp", season.EndTimestamp },
			};
			
			JArray goals = new();
			foreach (var goal in season.Goals)
			{
				JObject goalObj = new()
				{
					{ "uuid", goal.Uuid },
					{ "collected", goal.Collected}
				};

				JObject template = null;
				
				if (!goal.Template.IsFromApi)
				{
					template = new JObject()
					{
						{ "canBuyDough", goal.Template.CanBuyDough },
						{ "doughCost", goal.Template.DoughCost },
						{ "xpTotal", goal.Template.XpTotal },
						{ "canBuyVp", goal.Template.CanBuyVp },
						{ "vpCost", goal.Template.VpCost },
						{ "isEpilogue", goal.Template.IsEpilogue },
						{ "nameOverride", goal.Template.NameOverride }
					};
				}
				
				goalObj.Add("template", template);
				goals.Add(goalObj);
			}
			
			seasonObj.Add("goals", goals);
			seasonsArr.Add(seasonObj);
		}
		jo.Add("seasons", seasonsArr);

		JArray historyArr = new();
		foreach (var hg in history)
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
		                 { "gameMode", he.GameMode.Uuid },
		                 { "time", he.Time },
		                 { "amount", he.Amount },
		                 { "map", he.Map.Uuid },
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
			historyArr.Add(hgObj);
		}
		jo.Add("history", historyArr);

		if (!File.Exists(Constants.DataPath))
		{
			var sep = Constants.DataPath.LastIndexOf("/", StringComparison.Ordinal);

			Directory.CreateDirectory(Constants.DataPath[..sep]);
			File.CreateText(Constants.DataPath).Close();
		}

		File.WriteAllText(Constants.DataPath, jo.ToString());
	}
}