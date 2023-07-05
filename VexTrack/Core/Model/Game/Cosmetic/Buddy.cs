namespace VexTrack.Core.Model.Game.Cosmetic;

public class Buddy : Cosmetic 
{
    public string IconPath { get; set; }

    public Buddy(string uuid, string name, string iconPath) : base(uuid, name, "EquippableCharmLevel")
    {
        IconPath = iconPath;
    }
}