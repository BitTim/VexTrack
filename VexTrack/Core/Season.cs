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
    public long EndDate { get; set; }
    public int ActiveBpLevel { get; set; }
    public int Cxp { get; set; }
    public List<HistoryEntry> History { get; set; }


    public int Total => CalcUtil.CalcMaxForSeason(true);
    public int Collected => CalcUtil.CalcTotalCollected(ActiveBpLevel, Cxp);
    public int Remaining => Total - Collected;
    public int Average => CalcUtil.CalcAverage(ActiveBpLevel, Cxp, TrackingData.GetDuration(Uuid), TrackingData.GetRemainingDays(Uuid));
    public double Progress => CalcUtil.CalcProgress(Total, Collected);

    public bool IsActive => DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds() < EndDate;
    public String Status => GetStatus();
    public int Duration => GetDuration();
    public int RemainingDays => GetRemainingDays();
    public SeriesCollection GraphSeriesCollection => GraphCalc.CalcGraphs(Total, History.First().Amount, Duration, BufferDays, RemainingDays, History);

    public int BufferDays => (int)Math.Ceiling(Duration * (SettingsHelper.Data.BufferPercentage / 100));
    public SeasonExtremes Extremes => GetExtremes();
    public List<Goal> Goals { get; }
    public ObservableCollection<Goal> ObservableGoals => new(Goals);
    
    public string NextUnlockName => GetNextUnlock()?.Name ?? "None";
    public double NextUnlockProgress => GetNextUnlock()?.Progress ?? 100;
    public int NextUnlockRemaining => GetNextUnlock()?.Remaining ?? 0;


    public Season(string uuid, string name, long endDate, int activeBpLevel, int cXp, List<HistoryEntry> history)
    {
        (Uuid, Name, EndDate, ActiveBpLevel, Cxp, History) = (uuid, name, endDate, activeBpLevel, cXp, history);
        Goals = GetGoals();
    }

    private string GetStatus()
    {
        if (Collected < CalcUtil.CalcMaxForSeason(false)) return IsActive ? "Warning" : "Failed";
        if (Collected < Total) return "Done";
        return Collected >= Total ? "DoneAll" : "";
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
        DateTimeOffset startDate = DateTimeOffset.FromUnixTimeSeconds(History.First().Time).ToLocalTime().Date;

        var duration = (endDate - startDate).Days;
        if ((endDate - startDate).Hours > 12) { duration += 1; }
        if (duration < 0) duration = 0;

        return duration;
    }
    
    private SeasonExtremes GetExtremes()
    {
        if (History.Count <= 0) return new SeasonExtremes(0, -1, 0, -1);
        
        var prevDate = DateTimeOffset.FromUnixTimeSeconds(History.First().Time).ToLocalTime().Date;
        
        var weakestAmount = History.First().Amount;
        DateTimeOffset weakestDate = prevDate;
        var strongestAmount = History.First().Amount;
        DateTimeOffset strongestDate = prevDate;

        var currAmount = 0;
        
        foreach (var he in History)
        {
            var currDate = DateTimeOffset.FromUnixTimeSeconds(he.Time).ToLocalTime().Date;
            if (currDate == prevDate)
            {
                currAmount += he.Amount;
                continue;
            }

            (strongestAmount, strongestDate, weakestAmount, weakestDate) = EvalExtremes(currAmount, strongestAmount, prevDate, strongestDate, weakestAmount, weakestDate);

            currAmount = he.Amount;
            if (!SettingsHelper.Data.IgnoreInactiveDays)
            {
                var gapSize = (currDate - prevDate).Days;
                for(var i = 1; i < gapSize; i++) (strongestAmount, strongestDate, weakestAmount, weakestDate) = EvalExtremes(0, strongestAmount, prevDate.AddDays(1), strongestDate, weakestAmount, weakestDate);
            }

            prevDate = currDate;
        }
        
        return new SeasonExtremes(strongestAmount, strongestDate.ToUnixTimeSeconds(), weakestAmount, weakestDate.ToUnixTimeSeconds());
    }
    
    private static (int strongestAmount, DateTimeOffset strongestDate, int weakestAmount, DateTimeOffset weakestDate) EvalExtremes(int currAmount, int strongestAmount, DateTime prevDate, DateTimeOffset strongestDate, int weakestAmount, DateTimeOffset weakestDate)
    {
        if (currAmount > strongestAmount)
        {
            strongestAmount = currAmount;
            strongestDate = prevDate;
        }

        if (currAmount >= weakestAmount) return (strongestAmount, strongestDate, weakestAmount, weakestDate);
        
        weakestAmount = currAmount;
        weakestDate = prevDate;

        return (strongestAmount, strongestDate, weakestAmount, weakestDate);
    }

    private List<Goal> GetGoals()
    {
        var goals = new List<Goal>();

        for (var i = ActiveBpLevel - 1; i < ActiveBpLevel + 2; i++)
        {
            if (i > Constants.BattlepassLevels + Constants.EpilogueLevels) break;
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
}

public struct SeasonExtremes
{
    public int StrongestDayAmount { get; }
    public long StrongestDayTimestamp { get; }
    public int WeakestDayAmount { get; }
    public long WeakestDayTimestamp { get; }

    public SeasonExtremes(int strongestDayAmount, long strongestDayTimestamp, int weakestDayAmount, long weakestDayTimestamp)
    {
        StrongestDayAmount = strongestDayAmount;
        StrongestDayTimestamp = strongestDayTimestamp;
        WeakestDayAmount = weakestDayAmount;
        WeakestDayTimestamp = weakestDayTimestamp;
    }
}