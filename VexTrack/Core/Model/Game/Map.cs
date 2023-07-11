namespace VexTrack.Core.Model.Game;

public class Map
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string ListViewImagePath { get; set; }
    public string SplashImagePath { get; set; }

    public Map(string uuid, string name, string type, string listViewImagePath, string splashImagePath)
    {
        Uuid = uuid;
        Name = name;
        Type = type;
        ListViewImagePath = listViewImagePath;
        SplashImagePath = splashImagePath;
    }
}