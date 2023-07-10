using System.Collections.Generic;

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

    public string Name => ""; // TODO: Fetch name from reward cosmetic
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
}