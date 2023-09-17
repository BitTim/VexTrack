using System.Collections.Generic;
using System.Linq;
using VexTrack.Core.Model;

namespace VexTrack.Core.Helper;

public static class HistoryHelper
{
    public static List<HistoryGroup> GetFromSeason(string seasonUuid, List<HistoryGroup> historyOverride = null)
    {
        var history = historyOverride ?? UserData.History;
        return history.Where(hg => hg.SeasonUuid == seasonUuid).ToList();
    }

    public static int GetCountFromSeason(string seasonUuid, List<HistoryGroup> historyOverride = null)
    {
        return GetFromSeason(seasonUuid, historyOverride).Count;
    }
    
    public static HistoryEntry GetFirstFromSeason(string seasonUuid, List<HistoryGroup> historyOverride = null)
    {
        return GetFromSeason(seasonUuid, historyOverride).LastOrDefault()?.Entries.Last();
    }

    public static List<HistoryEntry> GetAllEntriesFromSeason(string seasonUuid, List<HistoryGroup> historyOverride = null)
    {
        return GetFromSeason(seasonUuid, historyOverride).SelectMany(hg => hg.Entries).ToList();
    }

    public static int CalcCollectedFromSeason(string seasonUuid, List<HistoryGroup> historyOverride = null)
    {
        var startXp = UserData.Seasons.FirstOrDefault(s => s.Uuid == seasonUuid)?.StartXp ?? 0;
        return GetAllEntriesFromSeason(seasonUuid, historyOverride).Sum(he => he.Amount) + startXp;
    }
    
    public static List<int> CalcDailyCollectedFromSeason(string seasonUuid, List<HistoryGroup> historyOverride = null)
    {
        var dailyAmounts = new List<int>();
        var historyGroups = GetFromSeason(seasonUuid, historyOverride);

        var seasonData = UserData.Seasons.FirstOrDefault(s => s.Uuid == seasonUuid);
        if (seasonData == null) return dailyAmounts;

        var startDate = TimeHelper.TimestampToDate(seasonData.StartTimestamp);
        
        for (var i = 0; i < seasonData.Duration + 1; i++)
        {
            var amount = 0;

            var currDate = startDate.AddDays(i).ToLocalTime().ToUnixTimeSeconds();
            var group = historyGroups.FirstOrDefault(hg => hg.Date == currDate);

            if (group != null)
            {
                amount = group.Entries.Select(e => e.Amount).Sum();
            }
            
            dailyAmounts.Add(amount);
        }

        return dailyAmounts;
    }

    public static void SortHistory()
    {
        foreach (var hg in UserData.History)
        {
            hg.Entries = hg.Entries.OrderByDescending(he => he.Time).ToList();
        }

        UserData.History = UserData.History.OrderByDescending(hg => hg.Date).ToList();
    }
}