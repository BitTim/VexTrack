namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponAdsStats
{
    public float ZoomMultiplier { get; set; }
    public float FireRate { get; set; }
    public float RunSpeedMultiplier { get; set; }
    public int BurstCount { get; set; }
    public float FirstBulletAccuracy { get; set; }

    public WeaponAdsStats(float zoomMultiplier, float fireRate, float runSpeedMultiplier, int burstCount, float firstBulletAccuracy)
    {
        ZoomMultiplier = zoomMultiplier;
        FireRate = fireRate;
        RunSpeedMultiplier = runSpeedMultiplier;
        BurstCount = burstCount;
        FirstBulletAccuracy = firstBulletAccuracy;
    }
}