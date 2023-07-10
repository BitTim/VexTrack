using System.Collections.Generic;

namespace VexTrack.Core.Model.Game.Weapon;

public class Weapon 
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string DefaultSkinUuid { get; set; }
    public string IconPath { get; set; }
    public string KillStreamIconPath { get; set; }
    public int ShopCost { get; set; }
    
    public WeaponStats Stats { get; set; }
    public List<string> SkinUuids { get; set; }

    public Weapon(string uuid, string name, string category, string defaultSkinUuid, string iconPath,
        string killStreamIconPath, int shopCost, WeaponStats stats, List<string> skinUuids)
    {
        Uuid = uuid;
        Name = name;
        Category = category;
        DefaultSkinUuid = defaultSkinUuid;
        IconPath = iconPath;
        KillStreamIconPath = killStreamIconPath;
        ShopCost = shopCost;
        
        Stats = stats;
        SkinUuids = skinUuids;
    }
}