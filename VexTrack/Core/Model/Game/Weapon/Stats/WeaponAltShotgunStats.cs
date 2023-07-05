namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponAltShotgunStats
{
    public int ShotgunPelletCount { get; set; }
    public double BurstRate { get; set; }

    public WeaponAltShotgunStats(int shotgunPelletCount, double burstRate)
    {
        ShotgunPelletCount = shotgunPelletCount;
        BurstRate = burstRate;
    }
}