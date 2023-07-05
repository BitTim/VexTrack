namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponDamageRange
{
    public int RangeStartMeters { get; set; }
    public int RangeEndMeters { get; set; }
    public double HeadDamage { get; set; }
    public double BodyDamage { get; set; }
    public double LegDamage { get; set; }

    public WeaponDamageRange(int rangeStartMeters, int rangeEndMeters, double headDamage, double bodyDamage, double legDamage)
    {
        RangeStartMeters = rangeStartMeters;
        RangeEndMeters = rangeEndMeters;
        HeadDamage = headDamage;
        BodyDamage = bodyDamage;
        LegDamage = legDamage;
    }
}