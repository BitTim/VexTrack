namespace VexTrack.Core.Model.Game.Cosmetic;

public class PlayerCard : Cosmetic 
{
    public string IconPath { get; set; }
    public string SmallArtPath { get; set; }
    public string WideArtPath { get; set; }
    public string LargeArtPath { get; set; }

    public PlayerCard(string uuid, string name, string iconPath, string smallArtPath, string wideArtPath, string largeArtPath) : base(uuid, name, "PlayerCard")
    {
        IconPath = iconPath;
        SmallArtPath = smallArtPath;
        WideArtPath = wideArtPath;
        LargeArtPath = largeArtPath;
    }
}