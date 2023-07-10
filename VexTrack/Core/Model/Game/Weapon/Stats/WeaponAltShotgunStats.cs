namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponAltShotgunStats
{
    public int ShotgunPelletCount { get; set; }
    public float BurstRate { get; set; }

    public WeaponAltShotgunStats(int shotgunPelletCount, float burstRate)
    {
        ShotgunPelletCount = shotgunPelletCount;
        BurstRate = burstRate;
    }
}