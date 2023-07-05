using System.Collections.Generic;
using VexTrack.Core.Model.Game.Cosmetic.Weapon;

namespace VexTrack.Core.Model.Game.Weapon;

public class Weapon 
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string DefaultSkinUuid { get; set; }
    public string IconPath { get; set; }
    public string KillStreamIconPath { get; set; }
    
    public WeaponStats Stats { get; set; }
    public int ShopCost { get; set; }
    public List<WeaponSkin> Skins { get; set; }

    public Weapon(string uuid, string name, string category, string defaultSkinUuid, string iconPath,
        string killStreamIconPath, WeaponStats stats, int shopCost, List<WeaponSkin> skins)
    {
        Uuid = uuid;
        Name = name;
        Category = category;
        DefaultSkinUuid = defaultSkinUuid;
        IconPath = iconPath;
        KillStreamIconPath = killStreamIconPath;
        
        Stats = stats;
        ShopCost = shopCost;
        Skins = skins;
    }
}