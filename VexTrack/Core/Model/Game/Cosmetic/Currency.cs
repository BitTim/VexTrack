namespace VexTrack.Core.Model.Game.Cosmetic;

public class Currency : Cosmetic 
{
    public string IconPath { get; set; }
    public string LargeIconPath { get; set; }

    public Currency(string uuid, string name, string iconPath, string largeIconPath) : base(uuid, name, "Currency")
    {
        IconPath = iconPath;
        LargeIconPath = largeIconPath;
    }
}