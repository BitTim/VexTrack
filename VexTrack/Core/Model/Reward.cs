namespace VexTrack.Core.Model;

public class Reward
{
    public string CosmeticUuid { get; set; }
    public string CosmeticType { get; set; }
    public int Amount { get; set; }
    public bool IsPremium { get; set; }

    public Reward(string cosmeticUuid, string cosmeticType, int amount, bool isPremium)
    {
        CosmeticUuid = cosmeticUuid;
        CosmeticType = cosmeticType;
        Amount = amount;
        IsPremium = isPremium;
    }
}