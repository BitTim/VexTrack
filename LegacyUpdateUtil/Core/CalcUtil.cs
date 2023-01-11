using System;

namespace LegacyUpdateUtil.Core
{
	public static class CalcUtil
	{
		public static double CalcProgress(double total, double collected)
		{
			if (total <= 0) return 100;

			var ret = Math.Floor(collected / total * 100);
			return ret;
		}
	}
}
