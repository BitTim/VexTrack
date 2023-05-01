using System.Collections.Generic;
using System.Linq;
using VexTrack.Core.Model;

namespace VexTrack.Core.Util;

public static class HistoryHelper
{
    public static List<HistoryGroup> GetFromSeason(string seasonUuid)
    {
        return Tracking.History.Where(hg => hg.SeasonUuid == seasonUuid).ToList();
    }

    public static int GetCountFromSeason(string seasonUuid)
    {
        return GetFromSeason(seasonUuid).Count;
    }
    
    public static HistoryEntry GetLastFromSeason(string seasonUuid)
    {
        return GetFromSeason(seasonUuid).Last().Entries.Last();
    }

    public static List<HistoryEntry> GetAllEntriesFromSeason(string seasonUuid)
    {
        return GetFromSeason(seasonUuid).SelectMany(hg => hg.Entries).ToList();
    }

    public static void SortHistory()
    {
        foreach (var hg in Tracking.History)
        {
            hg.Entries = hg.Entries.OrderByDescending(he => he.Time).ToList();
        }

        Tracking.History = Tracking.History.OrderByDescending(hg => hg.Date).ToList();
    }
}