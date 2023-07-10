namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponAirBurstStats
{
    public int ShotgunPelletCount { get; set; }
    public float BurstDistance { get; set; }

    public WeaponAirBurstStats(int shotgunPelletCount, float burstDistance)
    {
        ShotgunPelletCount = shotgunPelletCount;
        BurstDistance = burstDistance;
    }
}