using System;

namespace VexTrack.Core.Util;

public static class TimeHelper
{
    public static long IsolateTimestampDate(long timestamp)
    {
        var dt = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().Date;
        return ((DateTimeOffset)dt).ToUnixTimeSeconds();
    }

    public static DateTimeOffset TimestampToDto(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime();
    }
}