using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using VexTrack.Core.IO.LoaderParts;
using VexTrack.Core.Model;

namespace VexTrack.Core.IO;

public static class Loader
{
    internal static void LoadUserData()
    {
        if (!File.Exists(Constants.DataPath) || File.ReadAllText(Constants.DataPath) == "")
        {
            Tracking.InitData();
            Tracking.CreateDataInitPopup();
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

        if (seasons.Count == 0) Tracking.CreateDataInitPopup();

        Tracking.SetData(streak, lastStreakUpdateTimestamp, contracts, seasons, history);
        if(reSave) Saver.SaveUserData(streak, lastStreakUpdateTimestamp, contracts, seasons, history); // Save in new format
    }
}