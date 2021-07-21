using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	static class HistoryDataCalc
	{
		public static string CalcHistoryResult(string description)
		{
			string[] splitDesc = description.Split(" ");
			string scoreStr = "";

			foreach(string token in splitDesc)
			{
				if(!token.Contains("-")) { continue; }
				scoreStr = token;
			}

			if (scoreStr == "") return "";

			string[] scoreTokens = scoreStr.Split("-");
			if (scoreTokens.Length != 2) return "";

			int[] scoreComponents = { int.Parse(scoreTokens[0]), int.Parse(scoreTokens[1]) };

			if (scoreComponents[0] > scoreComponents[1]) return "Win";
			if (scoreComponents[0] < scoreComponents[1]) return "Loss";
			if (scoreComponents[0] == scoreComponents[1]) return "Draw";
			return "";
		}
	}
}
