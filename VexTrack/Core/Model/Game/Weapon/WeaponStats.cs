using System.Collections.Generic;
using VexTrack.Core.Model.Game.Weapon.Stats;

namespace VexTrack.Core.Model.Game.Weapon;

public class WeaponStats
{
    public double FireRate { get; set; }
    public int MagazineSize { get; set; }
    public double RunSpeedMultiplier { get; set; }
    public double EquipTimeSeconds { get; set; }
    public double ReloadTimeSeconds { get; set; }
    public double FirstBulletAccuracy { get; set; }
    public int ShotgunPelletCount { get; set; }
    
    public string WallPenetration { get; set; }
    public string AltFireMode { get; set; }

    public WeaponAdsStats AdsStats { get; set; }
    public WeaponAltShotgunStats AltShotgunStats { get; set; }
    public WeaponAirBurstStats AirBurstStats { get; set; }
    public List<WeaponDamageRange> DamageRanges { get; set; }

    public WeaponStats(double fireRate, int magazineSize, double runSpeedMultiplier, double equipTimeSeconds,
        double reloadTimeSeconds, double firstBulletAccuracy, int shotgunPelletCount, string wallPenetration,
        string altFireMode, WeaponAdsStats adsStats, WeaponAltShotgunStats altShotgunStats,
        WeaponAirBurstStats airBurstStats, List<WeaponDamageRange> damageRanges)
    {
        FireRate = fireRate;
        MagazineSize = magazineSize;
        RunSpeedMultiplier = runSpeedMultiplier;
        EquipTimeSeconds = equipTimeSeconds;
        ReloadTimeSeconds = reloadTimeSeconds;
        FirstBulletAccuracy = firstBulletAccuracy;
        ShotgunPelletCount = shotgunPelletCount;

        WallPenetration = wallPenetration;
        AltFireMode = altFireMode;

        AdsStats = adsStats;
        AltShotgunStats = altShotgunStats;
        AirBurstStats = airBurstStats;
        DamageRanges = damageRanges;
    }
}