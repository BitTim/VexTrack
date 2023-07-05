namespace VexTrack.Core.Model;

public class AgentAbility
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Slot { get; set; }
    public string IconPath { get; set; }

    public AgentAbility(string name, string description, string slot, string iconPath)
    {
        Name = name;
        Description = description;
        Slot = slot;
        IconPath = iconPath;
    }
}