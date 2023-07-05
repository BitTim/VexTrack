namespace VexTrack.Core.Model.WPF;

public class AgentRole
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }

    public AgentRole(string uuid, string name, string description, string iconPath)
    {
        Uuid = uuid;
        Name = name;
        Description = description;
        IconPath = iconPath;
    }
}