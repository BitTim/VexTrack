using System;
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
    public int Progress => CalcUtil.CalcProgress(Total, Collected);
    public string NextUnlockName => GetNextUnlock()?.Name ?? "None";
    public double NextUnlockProgress => GetNextUnlock()?.Progress ?? 100;
    public int NextUnlockRemaining => GetNextUnlock()?.Remaining ?? 0;
    public int CompletionForecastDays => GetCompletionForecastDays();
    public long CompletionDateTimestamp => GetCompletionDateTimestamp();
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

    public int GetCompletionForecastDays()
    {
        if (GetRemaining() <= 0) return -1;

        var average = TrackingData.CurrentSeasonData.Average;
        if (average <= 0) return -2;
        
        return (int)MathF.Ceiling((float)GetRemaining() / average);
    }
    public long GetCompletionDateTimestamp()
    {
        var nDays = CompletionForecastDays;
        return nDays < 0 ? nDays : new DateTimeOffset(DateTime.Today.ToLocalTime().Date.AddDays(nDays)).ToUnixTimeSeconds();
    }

    public Goal GetNextUnlock()
    {
        return Goals.FirstOrDefault(goal => !goal.IsCompleted());
    }
}