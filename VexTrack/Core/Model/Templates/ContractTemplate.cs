using System.Collections.Generic;

namespace VexTrack.Core.Model.Templates;

public class ContractTemplate
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string RelationUuid { get; set; }
    public List<GoalTemplate> Goals { get; set; }

    public long StartTimestamp => 0; // TODO: Fetch from related season / event
    public long EndTimestamp => 0;
    
    public ContractTemplate(string uuid, string name, string type, string relationUuid, List<GoalTemplate> goals)
    {
        Uuid = uuid;
        Name = name;
        Type = type;
        RelationUuid = relationUuid;
        Goals = goals;
    }
}