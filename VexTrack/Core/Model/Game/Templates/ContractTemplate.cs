using System.Collections.Generic;
using System.Linq;

namespace VexTrack.Core.Model.Game.Templates;

public class ContractTemplate
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public long StartTimestamp { get; set; }
    public long EndTimestamp { get; set; }
    public List<GoalTemplate> Goals { get; set; }

    public int XpTotal => Goals.Sum(g => g.XpTotal);
    
    public ContractTemplate(string uuid, string name, string type, long startTimestamp, long endTimestamp, List<GoalTemplate> goals)
    {
        Uuid = uuid;
        Name = name;
        Type = type;
        StartTimestamp = startTimestamp;
        EndTimestamp = endTimestamp;
        Goals = goals;
    }
}