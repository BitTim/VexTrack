using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.IO.UserData.LoaderParts;
using VexTrack.Core.Model;

namespace VexTrack.Core.IO.UserData;

public static class UserDataLoader
{
    internal static void LoadUserData()
    {
        var version = "empty";
        JObject jo = null;
        
        if (File.Exists(Constants.DataPath) && !string.IsNullOrEmpty(File.ReadAllText(Constants.DataPath)))
        {
            var rawJson = File.ReadAllText(Constants.DataPath);
            jo = JObject.Parse(rawJson);
            
            version = jo.Value<string>("version");
            if (string.IsNullOrEmpty(version)) version = "v1";
        }

        if (jo == null) version = "empty";
        
        var reSave = false;
			
        var streak = 0;
        var lastStreakUpdateTimestamp = TimeHelper.TodayDate.AddDays(-1).ToUnixTimeSeconds();
        List<HistoryGroup> history = new();
        List<Season> seasons = new();
        List<Contract> contracts = new();

        switch (version)
        {
            case "empty":
                Model.UserData.CreateDataInitPopup();
                reSave = true;
                break;
            
            case "v1":
                (streak, lastStreakUpdateTimestamp) = UserDataV1.LoadStreak(jo);
                (seasons, history) = UserDataV1.LoadSeasonsAndHistory(jo);
                contracts = UserDataV1.LoadContracts(jo);
                reSave = true;
                break;
				
            case "v2":
                (streak, lastStreakUpdateTimestamp) = UserDataV2.LoadStreak(jo);
                history = UserDataV2.LoadHistory(jo);
                seasons = UserDataV2.LoadSeasons(jo);
                contracts = UserDataV2.LoadContracts(jo);
                break;
        }


        var currentSeason = seasons.FirstOrDefault();
        if (currentSeason == null || currentSeason.EndTimestamp <= TimeHelper.NowTimestamp)
        {
            foreach (var template in Model.ApiData.ContractTemplates.Where(ct => ct.Type == "Season" && ct.EndTimestamp > TimeHelper.NowTimestamp && ct.StartTimestamp <= TimeHelper.NowTimestamp))
            {
                List<Goal> goals = new();
                foreach (var goalTemplate in template.Goals)
                {
                    goals.Add(new Goal(goalTemplate, Guid.NewGuid().ToString(), 0));
                }
				
                seasons.Insert(0, new Season(template, 0, goals));
            }
        }
        
        Model.UserData.SetData(streak, lastStreakUpdateTimestamp, contracts, seasons, history);
        if(reSave) UserDataSaver.SaveUserData(streak, lastStreakUpdateTimestamp, contracts, seasons, history); // Save in new format
    }
}