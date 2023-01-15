using System.Collections.Generic;

namespace VexTrack.Core;

public class Contract
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public bool Paused { get; set; }
    public List<Goal> Goals { get; set; }

    public Contract(string uuid, string name, string color, bool paused, List<Goal> goals)
    {
        Uuid = uuid;
        Name = name;
        Color = color;
        Paused = paused;
        Goals = goals;
    }
}