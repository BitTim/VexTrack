using System.Collections.Generic;
using System.Linq;

namespace VexTrack.Core.Model;

public class HistoryGroup
{
    public string SeasonUuid { get; set; }
    public string Uuid { get; set; }
    public long Date { get; set; }
    public List<HistoryEntry> Entries { get; set; }

    public int SumCollected => Entries.Select(e => e.Amount).Sum();
    
    public HistoryGroup(string sUuid, string uuid, long date, List<HistoryEntry> entries)
    {
        SeasonUuid = sUuid;
        Uuid = uuid;
        Date = date;
        Entries = entries;
    }
}