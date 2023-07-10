namespace VexTrack.Core.Model.Game.Weapon.Stats;

public class WeaponDamageRange
{
    public int RangeStartMeters { get; set; }
    public int RangeEndMeters { get; set; }
    public float HeadDamage { get; set; }
    public float BodyDamage { get; set; }
    public float LegDamage { get; set; }

    public WeaponDamageRange(int rangeStartMeters, int rangeEndMeters, float headDamage, float bodyDamage, float legDamage)
    {
        RangeStartMeters = rangeStartMeters;
        RangeEndMeters = rangeEndMeters;
        HeadDamage = headDamage;
        BodyDamage = bodyDamage;
        LegDamage = legDamage;
    }
}