using System;

namespace VexTrack.Core.Util;

public static class FormatHelper
{
    public static Func<double, string> FormatLargeNumber => value =>
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
    
    public static (double, string) FormatSize(double rawSize, bool isSpeed = false)
    {
        var size = rawSize;
        var unit = " B";

        if (isSpeed) unit = " B/s";

        var divisions = 0;
        while (size > 1)
        {
            size /= 1024;
            divisions++;

            if (divisions > 3) break;
        }

        if (divisions > 0)
        {
            size *= 1024;
            divisions--;
        }

        size = Math.Round(size, 2);

        if (divisions == 1) unit = unit.Insert(1, "K");
        if (divisions == 2) unit = unit.Insert(1, "M");
        if (divisions == 3) unit = unit.Insert(1, "G");

        return (size, unit);
    }
}