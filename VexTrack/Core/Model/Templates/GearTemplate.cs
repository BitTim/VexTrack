using System.Collections.Generic;

namespace VexTrack.Core.Model.Templates;

public class GearTemplate
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string AgentUuid { get; set; }
    public List<GoalTemplate> Goals { get; set; }

    public GearTemplate(string uuid, string name, string agentUuid, List<GoalTemplate> goals)
    {
        Uuid = uuid;
        Name = name;
        AgentUuid = agentUuid;
        Goals = goals;
    }
}