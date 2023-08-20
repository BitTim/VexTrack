namespace VexTrack.Core.Model.Game.Cosmetic;

public class Buddy : Cosmetic 
{
    public string LevelUuid { get; set; }
    public string IconPath { get; set; }

    public Buddy(string uuid, string levelUuid, string name, string iconPath) : base(uuid, name, "EquippableCharmLevel")
    {
        LevelUuid = levelUuid;
        IconPath = iconPath;
    }
}