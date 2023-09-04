using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveCharts;
using VexTrack.Core.Helper;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.Game.Templates;
using VexTrack.MVVM.Model;

namespace VexTrack.Core.Model;

public class Season
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public long ActualStartTimestamp { get; set; }
    public long StartTimestamp => ActualStartTimestamp == -1 ? TimeHelper.IsolateTimestampDate(HistoryHelper.GetFirstFromSeason(Uuid).Time) : ActualStartTimestamp;
    public long EndTimestamp { get; set; }
    public int Duration => GetDuration();
    public int RemainingDays => GetRemainingDays();


    public int Total => Goals.Sum(g => g.Total);
    private int TotalMin => Goals.Where(g => !g.IsEpilogue).Sum(g => g.Total);
    public int Collected => HistoryHelper.CalcCollectedFromSeason(Uuid);
    public int Remaining => Total - Collected;
    public double RemainingMin => TotalMin - Collected;
    public int Average => CalcHelper.CalcAverage(Collected, Duration, RemainingDays);
    public double Progress => CalcHelper.CalcProgress(Total, Collected);
    public int BufferDays => (int)Math.Ceiling(Duration * (SettingsHelper.Data.BufferPercentage / 100));

    private bool IsActive => TimeHelper.NowTimestamp < EndTimestamp;
    public string Status => GetStatus();
    
    public List<Goal> Goals { get; set; }
    public ObservableCollection<Goal> ObservableGoals => new(Goals);
    public SeasonExtremes Extremes => GetExtremes();
    public SeriesCollection GraphSeriesCollection => GraphCalcHelper.CalcGraphs(Total, HistoryHelper.GetFirstFromSeason(Uuid)?.Amount ?? 0, StartTimestamp, Duration, BufferDays, RemainingDays, Uuid);

    
    public string NextUnlockName => GetNextUnlock()?.Name ?? "None";
    public double NextUnlockProgress => GetNextUnlock()?.Progress ?? 100;
    public int NextUnlockRemaining => GetNextUnlock()?.Remaining ?? 0;


    
    public Season(string uuid, string name, long startTimestamp, long endTimestamp, List<Goal> goals)
    {
        (Uuid, Name, ActualStartTimestamp, EndTimestamp, Goals) = (uuid, name, startTimestamp, endTimestamp, goals);
    }

    public SeriesCollection GetDailyGraphSeriesCollection(int dayIndex, int dailyIdeal)
    {
        return GraphCalcHelper.CalcDailyGraphs(Total, dayIndex, StartTimestamp, Duration, dailyIdeal, Average, Uuid);
    }

    
    
    
    private int GetRemainingDays()
    {
        var endDate = TimeHelper.TimestampToDate(EndTimestamp);
        var today = TimeHelper.TodayDate;

        var remainingDays = (endDate - today).Days;
        if ((endDate - today).Hours > 12) { remainingDays += 1; }
        if (remainingDays < 0) remainingDays = 0;

        return remainingDays;
    }

    private int GetDuration()
    {
        var endDate = TimeHelper.TimestampToDate(EndTimestamp);
        var startDate = TimeHelper.TimestampToDate(StartTimestamp);

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

    private Goal GetNextUnlock()
    {
        return Goals.FirstOrDefault(goal => !goal.IsCompleted());
    }
    
    private SeasonExtremes GetExtremes()
    {
        var extremes = new SeasonExtremes(0, 0);
        if (HistoryHelper.GetCountFromSeason(Uuid) <= 0) return extremes;

        var prevDate = TimeHelper.TimestampToDate(StartTimestamp);
        
        extremes.StrongestDayAmount = HistoryHelper.GetFirstFromSeason(Uuid).Amount;
        extremes.WeakestDayAmount = HistoryHelper.GetFirstFromSeason(Uuid).Amount;
        extremes.StrongestDayTimestamp = prevDate.ToUnixTimeSeconds();
        extremes.WeakestDayTimestamp = prevDate.ToUnixTimeSeconds();

        var currAmount = 0;

        foreach (var hg in HistoryHelper.GetFromSeason(Uuid))
        {
            foreach (var he in hg.Entries)
            {
                var currDate = TimeHelper.TimestampToDate(he.Time);
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