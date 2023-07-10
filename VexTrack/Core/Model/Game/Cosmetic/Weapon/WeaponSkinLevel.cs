namespace VexTrack.Core.Model.Game.Cosmetic.Weapon;

public class WeaponSkinLevel : Cosmetic 
{
    public string ParentUuid { get; set; }
    public string LevelItem { get; set; }
    public string IconPath { get; set; }

    public WeaponSkinLevel(string uuid, string name, string parentUuid, string levelItem, string iconPath) : base(uuid, name, "EquippableSkinLevel")
    {
        ParentUuid = parentUuid;
        LevelItem = levelItem;
        IconPath = iconPath;
    }
}