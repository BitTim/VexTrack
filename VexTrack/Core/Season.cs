using System;
using System.Collections.Generic;
using System.Linq;

namespace VexTrack.Core;

public class Season
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public long EndDate { get; set; }
    public int ActiveBpLevel { get; set; }
    public int Cxp { get; set; }
    public List<HistoryEntry> History { get; set; }
    
    
    
    public double Progress => GetProgress();


    public Season(string uuid, string name, long endDate, int activeBpLevel, int cXp, List<HistoryEntry> history)
    {
        (Uuid, Name, EndDate, ActiveBpLevel, Cxp, History) = (uuid, name, endDate, activeBpLevel, cXp, history);
    }

    public int GetAverage()
    {
        var daysPassed = TrackingData.GetDuration(Uuid) - TrackingData.GetRemainingDays(Uuid);
        return (int)Math.Round((double)CalcUtil.CalcTotalCollected(ActiveBpLevel, Cxp) / (daysPassed + 1));
    }

    public double GetProgress()
    {
        return CalcUtil.CalcProgress(CalcUtil.CalcMaxForSeason(false), CalcUtil.CalcTotalCollected(ActiveBpLevel, Cxp));
    }

    public (int, long, int, long) GetExtremes()
    {
        if (History.Count <= 0) return (0, 0, 0, 0);
        
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
        
        return (weakestAmount, weakestDate.ToUnixTimeSeconds(), strongestAmount, strongestDate.ToUnixTimeSeconds());
    }

    
    
    private static (int strongestAmount, DateTimeOffset strongestDate, int weakestAmount, DateTimeOffset weakestDate)
        EvalExtremes(int currAmount, int strongestAmount, DateTime prevDate, DateTimeOffset strongestDate, int weakestAmount, DateTimeOffset weakestDate)
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
}