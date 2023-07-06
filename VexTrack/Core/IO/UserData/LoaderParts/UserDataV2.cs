using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Model;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.Templates;

namespace VexTrack.Core.IO.UserData.LoaderParts;

public static class UserDataV2
{
    // --------------------------------[ Season ]--------------------------------
    
    internal static List<Season> LoadSeasons(JObject jo)
    {
	    List<Season> seasons = new();
	    foreach (var jTokenSeason in jo["seasons"])
	    {
		    var season = (JObject)jTokenSeason;
		    var sUuid = season.Value<string>("uuid");
		    var name = season.Value<string>("name");
		    var endDate = season.Value<long>("endDate");
		    var activeBpLevel = season.Value<int>("activeBPLevel");
		    var cXp = season.Value<int>("cXP");

		    sUuid ??= Guid.NewGuid().ToString();
		    seasons.Add(new Season(sUuid, name, endDate, activeBpLevel, cXp));
	    }
		
	    return seasons;
    }
    
    
    
    
    // --------------------------------[ History ]--------------------------------
    		
    internal static List<HistoryGroup> LoadHistory(JObject jo)
    {
    	List<HistoryGroup> history = new();
    	foreach (var jTokenGroup in jo["history"])
    	{
    		var historyGroup = (JObject)jTokenGroup;
    		var sUuid = historyGroup.Value<string>("sUuid");
    		var gUuid = historyGroup.Value<string>("uuid");
    		var date = historyGroup.Value<long>("date");

    		List<HistoryEntry> entries = new();
    		foreach (var jTokenEntry in jTokenGroup["entries"])
    		{
    			var historyEntry = (JObject)jTokenEntry;
    			var hUuid = historyEntry.Value<string>("uuid");
    			var gamemodeUuid = historyEntry.Value<string>("gameMode");
    			var time = historyEntry.Value<long>("time");
    			var amount = historyEntry.Value<int>("amount");
    			var mapUuid = historyEntry.Value<string>("map");
    			var description = historyEntry.Value<string>("description");
                
                var map = Model.ApiData.Maps.Find(m => m.Uuid == mapUuid);

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
    				gameMode = Model.ApiData.GameModes.Find(gm => gm.Uuid == gamemodeUuid);
    				desc = description;
    				score = historyEntry.Value<int>("score");
    				enemyScore = historyEntry.Value<int>("enemyScore");
    				surrenderedWin = historyEntry.Value<bool>("surrenderedWin");
    				surrenderedLoss = historyEntry.Value<bool>("surrenderedLoss");
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
    
    
    
    
    // --------------------------------[ Streak ]--------------------------------
    
    internal static (int, long) LoadStreak(JObject jo)
    {
	    var streak = (int?)jo["streak"] ?? 0;
	    var lastStreakUpdateTimestamp = (long?)jo["lastStreakUpdate"] ?? 0;
	    return (streak, lastStreakUpdateTimestamp);
    }
    
    
    
    
    // --------------------------------[ Contracts ]--------------------------------

    internal static List<Contract> LoadContracts(JObject jo)
    {
	    List<Contract> contracts = new();
	    foreach (var jTokenContract in jo["contracts"])
	    {
		    var contract = (JObject)jTokenContract;
		    var source = contract["goals"];
		    List<GoalTemplate> goalTemplates = new();
		    List<Goal> goals = new();
			
		    if(source == null) continue;
		    foreach (var jTokenGoal in source)
		    {
			    var goal = (JObject)jTokenGoal;
			    var goalUuid = goal.Value<string>("uuid");
			    var goalName = goal.Value<string>("name");

			    var total = goal.Value<int>("total");
			    var collected = goal.Value<int>("collected");
			    if (collected > total) collected = total;
				
			    goalUuid ??= Guid.NewGuid().ToString();

			    var goalTemplate = new GoalTemplate( new List<Reward> {new("", "", 0, false)}, false, 0, total, false, 0);
			    goalTemplates.Add(goalTemplate);
			    goals.Add(new Goal(goalTemplate, goalUuid, collected));
		    }


		    var uuid = contract.Value<string>("uuid");
		    var name = contract.Value<string>("name");
		    var color = contract.Value<string>("color");
		    var paused = contract.Value<bool>("paused");
		    contracts.Add(new Contract(new ContractTemplate(uuid, name, "", -1, -1, goalTemplates), goals));
	    }

	    return contracts;
    }
}