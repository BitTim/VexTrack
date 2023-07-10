namespace VexTrack.Core.Model.Game.Cosmetic.Weapon;

public class WeaponSkinChroma : Cosmetic 
{
    public string ParentUuid { get; set; }
    public string IconPath { get; set; }
    public string FullRenderPath { get; set; }
    public string SwatchPath { get; set; }

    public WeaponSkinChroma(string uuid, string name, string parentUuid, string iconPath, string fullRenderPath, string swatchPath) : base(uuid, name, "EquippableSkinChroma")
    {
        ParentUuid = parentUuid;
        IconPath = iconPath;
        FullRenderPath = fullRenderPath;
        SwatchPath = swatchPath;
    }
}