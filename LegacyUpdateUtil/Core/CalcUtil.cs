using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyUpdateUtil.Core
{
	public static class CalcUtil
	{
		public static double CalcProgress(double total, double collected)
		{
			if (total <= 0) return 100;

			double ret = Math.Floor(collected / total * 100);
			return ret;
		}
	}
}
