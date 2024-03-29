﻿using System;
using System.Drawing;

namespace VexTrack.Core.Helper;

internal abstract class ColorHelper
{
	public struct Hsv
	{
		public float H;
		public readonly float S;
		public readonly float V;

		public Hsv(float hue, float saturation, float value) { (H, S, V) = (hue, saturation, value); }
	}

	public static Color ColorFromHsv(Hsv hsv)
	{
		var c = hsv.V * hsv.S;
		var x = c * (1 - MathF.Abs(hsv.H / 60 % 2 - 1));
		var m = hsv.V - c;

		float r = 0, g = 0, b = 0;

		while (hsv.H >= 360) hsv.H -= 360;

		if (hsv.H is >= 0 and < 60) (r, g, b) = (c, x, 0);
		if (hsv.H is >= 60 and < 120) (r, g, b) = (x, c, 0);
		if (hsv.H is >= 120 and < 180) (r, g, b) = (0, c, x);
		if (hsv.H is >= 180 and < 240) (r, g, b) = (0, x, c);
		if (hsv.H is >= 240 and < 300) (r, g, b) = (x, 0, c);
		if (hsv.H is >= 300 and < 360) (r, g, b) = (c, 0, x);

		return Color.FromArgb(255, (int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255));
	}

	public static float GetValue(Color c)
	{
		if (c.R == 0 && c is { G: 0, B: 0 }) return 0;

		var r = c.R / 255f;
		var g = c.G / 255f;
		var b = c.B / 255f;

		return MathF.Max(r, MathF.Max(g, b));
	}
}

public static class ColorConv
{
	public static System.Windows.Media.Color ToSwmColor(this Color color) => System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
	public static Color ToSdColor(this System.Windows.Media.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);
}