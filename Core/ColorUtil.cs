using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	class ColorUtil
	{
        Color SetHue(Color oldColor)
        {
            var temp = new HSV();
            temp.h = oldColor.GetHue();
            temp.s = oldColor.GetSaturation();
            temp.v = getValue(oldColor);
            return ColorFromHSV(temp);
        }

        public struct HSV
        {
            public float h;
            public float s;
            public float v;

            public HSV(float hue, float saturation, float value) { (h, s, v) = (hue, saturation, value); }
        }

        static public Color ColorFromHSV(HSV hsv)
        {
            float c = hsv.v * hsv.s;
            float x = c * (1 - MathF.Abs((hsv.h / 60 % 2) - 1));
            float m = hsv.v - c;

            float r = 0, g = 0, b = 0;

            if (hsv.h >=   0 && hsv.h <  60) (r, g, b) = (c, x, 0);
            if (hsv.h >=  60 && hsv.h < 120) (r, g, b) = (x, c, 0);
            if (hsv.h >= 120 && hsv.h < 180) (r, g, b) = (0, c, x);
            if (hsv.h >= 180 && hsv.h < 240) (r, g, b) = (0, x, c);
            if (hsv.h >= 240 && hsv.h < 300) (r, g, b) = (x, 0, c);
            if (hsv.h >= 300 && hsv.h < 360) (r, g, b) = (c, 0, x);
            if (hsv.h == 360)                (r, g, b) = (c, x, 0);

            return Color.FromArgb(255, (int)((r + m) * 255),(int)((g + m) * 255), (int)((b + m) * 255));
        }

        public static float getValue(Color c)
        {
            float r, g, b;

            if (c.R == 0 && c.G == 0 && c.B == 0) return 0;

            r = c.R / 255f;
            g = c.G / 255f;
            b = c.B / 255f;

            return MathF.Max(r, MathF.Max(g, b));
        }
    }

    public static class ColorConv
	{
        public static System.Windows.Media.Color ToSWMColor(this Color color) => System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        public static Color ToSDColor(this System.Windows.Media.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}
