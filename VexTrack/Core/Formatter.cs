using System;

namespace VexTrack.Core;

public static class Formatter
{
    public static Func<double, string> LargeNumberFormatter => value =>
    {
        if (value == 0) return "0";

        var mag = (int)(Math.Floor(Math.Log10(value)) / 3); // Truncates to 6, divides to 2
        var divisor = Math.Pow(10, mag * 3);

        var shortNumber = value / divisor;

        var suffix = mag switch
        {
            0 => string.Empty,
            1 => "k",
            2 => "M",
            3 => "B",
            _ => ""
        };

        return shortNumber.ToString("N1") + suffix;
    };
}