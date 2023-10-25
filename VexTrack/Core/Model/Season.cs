using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveCharts;
using VexTrack.Core.Helper;
using VexTrack.Core.Model.Game.Templates;

namespace VexTrack.Core.Model;

public class Season : Contract
{
    
    public new long StartTimestamp => ActualStartTimestamp == -1 ? TimeHelper.IsolateTimestampDate(HistoryHelper.GetFirstFromSeason(Uuid).Time) : ActualStartTimestamp;
    public new int Collected => HistoryHelper.CalcCollectedFromSeason(Uuid);
    public new string Status => GetStatus();
    
    private int TotalMin => Goals.Where(g => !g.IsEpilogue).Sum(g => g.Total);
    public int RemainingMin => TotalMin - Collected;
    private long ActualStartTimestamp { get; set; }
    public int StartXp { get; set; }
    public int Average => CalcHelper.CalcAverage(Collected, Duration, RemainingDays);
    
    public SeasonExtremes Extremes => GetExtremes();
    public SeriesCollection GraphSeriesCollection => GraphCalcHelper.CalcGraphs(Uuid);



    public Season(ContractTemplate template, int startXp, List<Goal> goals) : base(template, goals)
    {
        StartXp = startXp;
        ActualStartTimestamp = template.StartTimestamp;
    }

    public SeriesCollection GetDailyGraphSeriesCollection(int dayIndex, int dailyIdeal)
    {
        return GraphCalcHelper.CalcDailyGraphs(dayIndex, dailyIdeal, Uuid);
    }
    
    private string GetStatus()
    {
        if (Collected < TotalMin)
        {
            if (!IsActive) return "Failed";
            if (RemainingDays < BufferDays) return "Warning";
            return "Active";
        }

        if (Collected >= TotalMin && Collected < Total) return "Done";
        if (Collected >= Total) return "DoneAll";
        return "";
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