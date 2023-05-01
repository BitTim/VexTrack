using System;

namespace VexTrack.Core.Helper;

public static class TimeHelper
{
    public static long NowTimestamp => DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds();
    public static DateTimeOffset NowTime => DateTimeOffset.Now.ToLocalTime();
    public static long TodayTimestamp => IsolateTimestampDate(NowTimestamp);
    public static DateTimeOffset TodayDate => DateTime.Today.ToLocalTime();
    
    

    public static DateTimeOffset TimestampToTime(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime();
    }
    
    public static DateTimeOffset TimestampToDate(long timestamp)
    {
        return TimestampToTime(timestamp).Date;
    }
    
    public static long IsolateTimestampDate(long timestamp)
    {
        return TimestampToDate(timestamp).ToUnixTimeSeconds();
    }

    public static long StringToTimestamp(string str)
    {
        return DateTimeOffset.Parse(str).ToLocalTime().ToUnixTimeSeconds();
    }
}