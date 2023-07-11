namespace VexTrack.Core.Model.Game;

public class GameMode
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string MapType { get; set; }
    public string ScoreType { get; set; }
    public string IconPath { get; set; }

    public GameMode(string uuid, string name, string mapType, string scoreType, string iconPath)
    {
        Uuid = uuid;
        Name = name;
        MapType = mapType;
        ScoreType = scoreType;
        IconPath = iconPath;
    }
}