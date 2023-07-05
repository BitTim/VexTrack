namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponAirBurstStats
{
    public int ShotgunPelletCount { get; set; }
    public double BurstDistance { get; set; }

    public WeaponAirBurstStats(int shotgunPelletCount, double burstDistance)
    {
        ShotgunPelletCount = shotgunPelletCount;
        BurstDistance = burstDistance;
    }
}