namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponAdsStats
{
    public float ZoomMultiplier { get; set; }
    public float FireRate { get; set; }
    public float RunSpeedModifier { get; set; }
    public int BurstCount { get; set; }
    public float FirstBulletAccuracy { get; set; }

    public WeaponAdsStats(float zoomMultiplier, float fireRate, float runSpeedModifier, int burstCount, float firstBulletAccuracy)
    {
        ZoomMultiplier = zoomMultiplier;
        FireRate = fireRate;
        RunSpeedModifier = runSpeedModifier;
        BurstCount = burstCount;
        FirstBulletAccuracy = firstBulletAccuracy;
    }
}