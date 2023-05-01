using System.Collections.Generic;

namespace VexTrack.Core.Model;

public class HistoryGroup
{
    public string SeasonUuid { get; set; }
    public string Uuid { get; set; }
    public long Date { get; set; }
    public List<HistoryEntry> Entries { get; set; }

    public HistoryGroup(string sUuid, string uuid, long date, List<HistoryEntry> entries)
    {
        SeasonUuid = sUuid;
        Uuid = uuid;
        Date = date;
        Entries = entries;
    }
}