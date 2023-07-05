namespace VexTrack.Core.Model.Game.Cosmetic.Weapon;

public class WeaponSkinLevel : Cosmetic 
{
    public string LevelItem { get; set; }
    public string IconPath { get; set; }

    public WeaponSkinLevel(string uuid, string name, string levelItem, string iconPath) : base(uuid, name, "EquippableSkinLevel")
    {
        LevelItem = levelItem;
        IconPath = iconPath;
    }
}