using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveCharts;

namespace VexTrack.Core;

public class Season
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public long StartDate => ((DateTimeOffset)DateTimeOffset.FromUnixTimeSeconds(History.First().Time).Date.ToLocalTime()).ToUnixTimeSeconds(); // TODO: Change to API data with #69
    public long EndDate { get; set; }
    public int Collected => CalcUtil.CalcTotalCollected(ActiveBpLevel, Cxp); // TODO: Replace ActiveBpLevel and Cxp with #69
    public int ActiveBpLevel { get; set; }
    public int Cxp { get; set; }
    public int Duration => GetDuration();
    public int RemainingDays => GetRemainingDays();
    public List<HistoryEntry> History { get; set; }


    public int Total => CalcUtil.CalcMaxForSeason(true);
    private static int TotalMin => CalcUtil.CalcMaxForSeason(false);
    public int Remaining => Total - Collected;
    public double RemainingMin => TotalMin - Collected;
    public int Average => CalcUtil.CalcAverage(Collected, Duration, RemainingDays);
    public double Progress => CalcUtil.CalcProgress(Total, Collected);
    public int BufferDays => (int)Math.Ceiling(Duration * (SettingsHelper.Data.BufferPercentage / 100));

    private bool IsActive => DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds() < EndDate;
    public string Status => GetStatus();
    
    private List<Goal> Goals { get; }
    public ObservableCollection<Goal> ObservableGoals => new(Goals);
    public SeasonExtremes Extremes => GetExtremes();
    public SeriesCollection GraphSeriesCollection => GraphCalc.CalcGraphs(Total, History.First().Amount, StartDate, Duration, BufferDays, RemainingDays, History);

    
    public string NextUnlockName => GetNextUnlock()?.Name ?? "None";
    public double NextUnlockProgress => GetNextUnlock()?.Progress ?? 100;
    public int NextUnlockRemaining => GetNextUnlock()?.Remaining ?? 0;


    
    public Season(string uuid, string name, long endDate, int activeBpLevel, int cXp, List<HistoryEntry> history)
    {
        (Uuid, Name, EndDate, ActiveBpLevel, Cxp, History) = (uuid, name, endDate, activeBpLevel, cXp, history);
        Goals = GetGoals();
    }

    public SeriesCollection GetDailyGraphSeriesCollection(int dayIndex, int dailyIdeal)
    {
        return GraphCalc.CalcDailyGraphs(Total, dayIndex, StartDate, Duration, dailyIdeal, Average, History);
    }

    
    
    
    private int GetRemainingDays()
    {
        var endDate = DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date;
        DateTimeOffset today = DateTimeOffset.Now.ToLocalTime().Date;

        var remainingDays = (endDate - today).Days;
        if ((endDate - today).Hours > 12) { remainingDays += 1; }
        if (remainingDays < 0) remainingDays = 0;

        return remainingDays;
    }

    private int GetDuration()
    {
        DateTimeOffset endDate = DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date;
        var startDate = DateTimeOffset.FromUnixTimeSeconds(StartDate);

        var duration = (endDate - startDate).Days;
        if ((endDate - startDate).Hours > 12) { duration += 1; }
        if (duration < 0) duration = 0;

        return duration;
    }
    
    private string GetStatus()
    {
        if (Collected < TotalMin) return IsActive ? "Warning" : "Failed";
        if (Collected < Total) return "Done";
        return Collected >= Total ? "DoneAll" : "";
    }
    
    private List<Goal> GetGoals()
    {
        var goals = new List<Goal>();
        var maxLevel = Constants.BattlepassLevels + Constants.EpilogueLevels;
        var startLevel = ActiveBpLevel - 1;
        var endLevel = ActiveBpLevel + 2;

        if (ActiveBpLevel > maxLevel - 3)
        {
            startLevel = maxLevel - 2;
            endLevel = maxLevel + 1;
        }

        for (var i = startLevel; i < endLevel; i++)
        {
            if (i > maxLevel) break;
            var levelTotal = CalcUtil.CalcMaxForLevel(i);
            var goal = new Goal(Guid.NewGuid().ToString(), "Level " + i, levelTotal, ActiveBpLevel <= i ? ActiveBpLevel == i ? Cxp : 0 : levelTotal);
            goals.Add(goal);
        }

        return goals;
    }

    private Goal GetNextUnlock()
    {
        return Goals.FirstOrDefault(goal => !goal.IsCompleted());
    }
    
    private SeasonExtremes GetExtremes()
    {
        var extremes = new SeasonExtremes(0, 0);
        if (History.Count <= 0) return extremes;
        
        var prevDate = (DateTimeOffset) DateTimeOffset.FromUnixTimeSeconds(History.First().Time).ToLocalTime().Date;
        
        extremes.StrongestDayAmount = History.First().Amount;
        extremes.WeakestDayAmount = History.First().Amount;
        extremes.StrongestDayTimestamp = prevDate.ToUnixTimeSeconds();
        extremes.WeakestDayTimestamp = prevDate.ToUnixTimeSeconds();

        var currAmount = 0;
        
        foreach (var he in History)
        {
            var currDate = DateTimeOffset.FromUnixTimeSeconds(he.Time).ToLocalTime().Date;
            if (currDate == prevDate)
            {
                currAmount += he.Amount;
                continue;
            }

            extremes.EvalExtremes(currAmount, prevDate);

            currAmount = he.Amount;
            if (!SettingsHelper.Data.IgnoreInactiveDays)
            {
                var gapSize = (currDate - prevDate).Days;
                for(var i = 1; i < gapSize; i++) extremes.EvalExtremes(0, prevDate.AddDays(1));
            }

            prevDate = currDate;
        }
        
        return extremes;
    }
}



public struct SeasonExtremes
{
    public int StrongestDayAmount { get; set; }
    public int WeakestDayAmount { get; set; }
    public long StrongestDayTimestamp { get; set; }
    public long WeakestDayTimestamp { get; set; }

    public SeasonExtremes(int strongestDayAmount, int weakestDayAmount, long strongestDayTimestamp = -1, long weakestDayTimestamp = -1)
    {
        StrongestDayAmount = strongestDayAmount;
        WeakestDayAmount = weakestDayAmount;
        StrongestDayTimestamp = strongestDayTimestamp;
        WeakestDayTimestamp = weakestDayTimestamp;
    }
    
    public void EvalExtremes(int currAmount, DateTimeOffset prevDate)
    {
        if (currAmount > StrongestDayAmount)
        {
            StrongestDayAmount = currAmount;
            StrongestDayTimestamp = prevDate.ToUnixTimeSeconds();
        }

        if (currAmount >= WeakestDayAmount && !(WeakestDayAmount == 0 && SettingsHelper.Data.IgnoreInactiveDays)) return;
        
        WeakestDayAmount = currAmount;
        WeakestDayTimestamp = prevDate.ToUnixTimeSeconds();
    }
}