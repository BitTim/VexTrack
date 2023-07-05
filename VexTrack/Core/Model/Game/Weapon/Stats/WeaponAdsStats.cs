namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponAdsStats
{
    public double ZoomMultiplier { get; set; }
    public double FireRate { get; set; }
    public double RunSpeedModifier { get; set; }
    public int BurstCount { get; set; }
    public double FirstBulletAccuracy { get; set; }

    public WeaponAdsStats(double zoomMultiplier, double fireRate, double runSpeedModifier, int burstCount, double firstBulletAccuracy)
    {
        ZoomMultiplier = zoomMultiplier;
        FireRate = fireRate;
        RunSpeedModifier = runSpeedModifier;
        BurstCount = burstCount;
        FirstBulletAccuracy = firstBulletAccuracy;
    }
}