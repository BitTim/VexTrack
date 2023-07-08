using System.Collections.Generic;

namespace VexTrack.Core.Model.Game.Cosmetic.Weapon;

public class WeaponSkin : Cosmetic
{
    public string ParentUuid { get; set; }
    public string IconPath { get; set; }
    public string WallpaperPath { get; set; }
    
    public List<string> ChromaUuids { get; set; }
    public List<string> LevelUuids { get; set; }

    public WeaponSkin(string uuid, string name, string parentUuid, string iconPath, string wallpaperPath, List<string> chromaUuids, List<string> levelUuids) : base(uuid, name, "WeaponSkin")
    {
        ParentUuid = parentUuid;
        IconPath = iconPath;
        WallpaperPath = wallpaperPath;
        ChromaUuids = chromaUuids;
        LevelUuids = levelUuids;
    }
}