using System.Collections.Generic;
using VexTrack.Core.Model.WPF;

namespace VexTrack.Core.Model;

public class Agent
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public string PortraitPath { get; set; }
    public string KillFeedPortraitPath { get; set; }
    public string BackgroundPath { get; set; }
    public List<string> GradientColors { get; set; }
    public bool IsBaseContent { get; set; }
    public string RoleUuid { get; set; }
    public List<AgentAbility> Abilities { get; set; }

    public Agent(string uuid, string name, string description, string iconPath, string portraitPath, string killFeedPortraitPath, string backgroundPath, List<string> gradientColors, bool isBaseContent, string roleUuid, List<AgentAbility> abilities)
    {
        Uuid = uuid;
        Name = name;
        Description = description;
        IconPath = iconPath;
        PortraitPath = portraitPath;
        KillFeedPortraitPath = killFeedPortraitPath;
        BackgroundPath = backgroundPath;
        GradientColors = gradientColors;
        IsBaseContent = isBaseContent;
        RoleUuid = roleUuid;
        Abilities = abilities;
    }
}