using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Model;
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
    
    
    
    
    // --------------------------------[ History ]--------------------------------
    		
    internal static List<HistoryGroup> LoadHistory(JObject jo)
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
    			var gamemodeUuid = (string)historyEntry["gameMode"];
    			var time = (long)historyEntry["time"];
    			var amount = (int)historyEntry["amount"];
    			var mapUuid = (string)historyEntry["map"];
    			var description = (string)historyEntry["description"];
                
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
			    var goalUuid = (string)goal["uuid"];
			    var goalName = (string)goal["name"];

			    var total = (int)goal["total"];
			    var collected = (int)goal["collected"];
			    if (collected > total) collected = total;
				
			    goalUuid ??= Guid.NewGuid().ToString();

			    var goalTemplate = new GoalTemplate( new List<Reward> {new("", "", 0, false)}, false, 0, total, false, 0);
			    goalTemplates.Add(goalTemplate);
			    goals.Add(new Goal(goalTemplate, goalUuid, collected));
		    }


		    var uuid = (string)contract["uuid"];
		    var name = (string)contract["name"];
		    var color = (string)contract["color"];
		    var paused = (bool)contract["paused"];
		    contracts.Add(new Contract(new ContractTemplate(uuid, name, "", "", goalTemplates), goals));
	    }

	    return contracts;
    }
}