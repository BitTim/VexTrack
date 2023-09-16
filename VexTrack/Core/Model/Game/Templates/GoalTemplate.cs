using System.Collections.Generic;

namespace VexTrack.Core.Model.Game.Templates;

public class GoalTemplate
{
    public List<Reward> Rewards { get; set; }
    
    public bool IsFromApi { get; set; }
    public int DoughCost { get; set; }
    public int XpTotal { get; set; }
    public int VpCost { get; set; }
    public bool CanBuyDough { get; set; }
    public bool CanBuyVp { get; set; }
    public bool IsEpilogue { get; set; }
    public string NameOverride { get; set; }

    public string Name => GetName();
    public bool CanUseXp => !CanBuyDough;
    
    public GoalTemplate(List<Reward> rewards, bool isFromApi, bool canBuyDough, int doughCost, int xpTotal, bool canBuyVp, int vpCost, bool isEpilogue = false, string nameOverride = "")
    {
        Rewards = rewards;

        IsFromApi = isFromApi;
        CanBuyDough = canBuyDough;
        DoughCost = doughCost;
        XpTotal = xpTotal;
        CanBuyVp = canBuyVp;
        VpCost = vpCost;
        
        IsEpilogue = isEpilogue;
        NameOverride = nameOverride;
    }

    private string GetName()
    {
        if (!string.IsNullOrEmpty(NameOverride)) return NameOverride;
        
        var rewardNames = new List<string>();
        foreach (var reward in Rewards)
        {
            if (reward == null) continue;
            rewardNames.Add(ApiData.GetCosmetic(reward.CosmeticType, reward.CosmeticUuid).Name);
        }

        if (rewardNames.Count < 0) return "No Rewards";
        return string.Join(" | ", rewardNames);
    }
}