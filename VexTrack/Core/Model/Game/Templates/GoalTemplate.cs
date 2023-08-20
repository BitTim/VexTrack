using System.Collections.Generic;
using System.Linq;

namespace VexTrack.Core.Model.Game.Templates;

public class GoalTemplate
{
    public List<Reward> Rewards { get; set; }
    
    public int DoughCost { get; set; }
    public int XpTotal { get; set; }
    public int VpCost { get; set; }
    public bool CanBuyDough { get; set; }
    public bool CanBuyVp { get; set; }
    public bool IsEpilogue { get; set; }

    public string Name => GetName();
    public bool CanUseXp => !CanBuyDough;
    
    public GoalTemplate(List<Reward> rewards, bool canBuyDough, int doughCost, int xpTotal, bool canBuyVp, int vpCost, bool isEpilogue = false)
    {
        Rewards = rewards;
        
        CanBuyDough = canBuyDough;
        DoughCost = doughCost;
        XpTotal = xpTotal;
        CanBuyVp = canBuyVp;
        VpCost = vpCost;
        
        IsEpilogue = isEpilogue;
    }

    private string GetName()
    {
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