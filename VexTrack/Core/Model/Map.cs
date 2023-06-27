namespace VexTrack.Core.Model;

public class Map
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string ListViewImagePath { get; set; }
    public string SplashImagePath { get; set; }

    public Map(string uuid, string name, string listViewImagePath, string splashImagePath)
    {
        Uuid = uuid;
        Name = name;
        ListViewImagePath = listViewImagePath;
        SplashImagePath = splashImagePath;
    }
}