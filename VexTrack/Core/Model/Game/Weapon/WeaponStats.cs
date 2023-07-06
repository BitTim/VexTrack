using System.Collections.Generic;
using VexTrack.Core.Model.Game.Weapon.Stats;

namespace VexTrack.Core.Model.Game.Weapon;

public class WeaponStats
{
    public float FireRate { get; set; }
    public int MagazineSize { get; set; }
    public float RunSpeedMultiplier { get; set; }
    public float EquipTimeSeconds { get; set; }
    public float ReloadTimeSeconds { get; set; }
    public float FirstBulletAccuracy { get; set; }
    public int ShotgunPelletCount { get; set; }
    
    public string WallPenetration { get; set; }
    public string Feature { get; set; }
    public string FireMode { get; set; }
    public string AltFireMode { get; set; }

    public WeaponAdsStats AdsStats { get; set; }
    public WeaponAltShotgunStats AltShotgunStats { get; set; }
    public WeaponAirBurstStats AirBurstStats { get; set; }
    public List<WeaponDamageRange> DamageRanges { get; set; }

    public WeaponStats(float fireRate, int magazineSize, float runSpeedMultiplier, float equipTimeSeconds,
        float reloadTimeSeconds, float firstBulletAccuracy, int shotgunPelletCount, string wallPenetration,
        string feature, string fireMode, string altFireMode, WeaponAdsStats adsStats,
        WeaponAltShotgunStats altShotgunStats, WeaponAirBurstStats airBurstStats, List<WeaponDamageRange> damageRanges)
    {
        FireRate = fireRate;
        MagazineSize = magazineSize;
        RunSpeedMultiplier = runSpeedMultiplier;
        EquipTimeSeconds = equipTimeSeconds;
        ReloadTimeSeconds = reloadTimeSeconds;
        FirstBulletAccuracy = firstBulletAccuracy;
        ShotgunPelletCount = shotgunPelletCount;

        WallPenetration = wallPenetration;
        Feature = feature;
        FireMode = fireMode;
        AltFireMode = altFireMode;

        AdsStats = adsStats;
        AltShotgunStats = altShotgunStats;
        AirBurstStats = airBurstStats;
        DamageRanges = damageRanges;
    }
}