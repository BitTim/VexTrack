using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VexTrack.Core;

public class Contract
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public bool Paused { get; set; }
    public List<Goal> Goals { get; set; }
    
    
    
    public int Total => GetTotal();
    public int Collected => GetCollected();
    public int Remaining => GetRemaining();
    public double Progress => CalcUtil.CalcProgress(Total, Collected);
    public string NextUnlockName => GetNextUnlock()?.Name ?? "None";
    public double NextUnlockProgress => GetNextUnlock()?.Progress ?? 1.0;
    public int NextUnlockRemaining => GetNextUnlock()?.Remaining ?? 0;
    public ObservableCollection<Goal> ObservableGoals => new ObservableCollection<Goal>(Goals);
    


    public Contract(string uuid, string name, string color, bool paused, List<Goal> goals)
    {
        Uuid = uuid;
        Name = name;
        Color = color;
        Paused = paused;
        Goals = goals;
    }

    public int GetTotal() { return Goals.Sum(goal => goal.Total); }
    public int GetCollected() { return Goals.Sum(goal => goal.Collected); }
    public int GetRemaining() { return GetTotal() - GetCollected(); }

    public Goal GetNextUnlock()
    {
        return Goals.FirstOrDefault(goal => !goal.IsCompleted());
    }
}