using System.Collections.Generic;

namespace VexTrack.Core.Model.Game.Templates;

public class ContractTemplate
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public long StartTimestamp { get; set; }
    public long EndTimestamp { get; set; }
    public List<GoalTemplate> Goals { get; set; }

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