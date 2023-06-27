namespace VexTrack.Core.Model;

public class GameMode
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string ScoreType { get; set; }
    public string IconPath { get; set; }

    public GameMode(string uuid, string name, string scoreType, string iconPath)
    {
        Uuid = uuid;
        Name = name;
        ScoreType = scoreType;
        IconPath = iconPath;
    }
}