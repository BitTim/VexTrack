using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	static class Constants
	{
		public static readonly string DataPath = @"dat/data.json";
		public static readonly string LegacyDataPath = @"dat/config.json";

		public static readonly int BattlepassLevels = 50;
		public static readonly int EpilogueLevels = 5;

		public static readonly int XPPerLevel = 750;
		public static readonly int XPPerEpilogueLevel = 36500;
		public static readonly int Level2Offset = 500;

		//TODO: Move this to Settings
		public static readonly int BufferDays = 8;
	}
}
