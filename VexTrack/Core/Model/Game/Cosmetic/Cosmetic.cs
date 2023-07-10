namespace VexTrack.Core.Model.Game.Cosmetic;

public class Cosmetic
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }

    public Cosmetic(string uuid, string name, string type)
    {
        Uuid = uuid;
        Name = name;
        Type = type;
    }
}