using System.Collections.ObjectModel;
using System.Linq;

namespace VexTrack.Core
{
	public static class HistoryDataCalc
	{
		public static string CalcHistoryResultFromScores(string scoreType, int score, int enemyScore, bool surrenderedWin, bool surrenderedLoss)
		{
			if (scoreType == "Score")
			{
				if (surrenderedWin) return "Surrendered Win";
				if (surrenderedLoss) return "Surrendered Loss";
			}

			if (scoreType == "Placement" || scoreType == "None") enemyScore = -1;
			if (scoreType == "None") score = -1;

			if (enemyScore == -1)
			{
				if (score == -1) return "";

				string scoreString = score.ToString();
				string suffix = "th";

				if (scoreString.Last() == '1') suffix = "st";
				if (scoreString.Last() == '2') suffix = "nd";
				if (scoreString.Last() == '3') suffix = "rd";

				if (scoreString.Length > 1 && scoreString[scoreString.Length - 2] == '1') suffix = "th";
				return scoreString + suffix;
			}

			if (score > enemyScore) return "Win";
			if (score < enemyScore) return "Loss";
			if (score == enemyScore) return "Draw";
			return "";
		}

		public static string CalcHistoryResultFromString(string description)
		{
			if (description == "" || description == null) return "";

			string[] splitDesc = description.Split(" ");
			string scoreStr = "";

			foreach (string token in splitDesc)
			{
				if (!token.Contains("-")) { continue; }
				scoreStr = token;

				if (scoreStr == "") return "";

				string[] scoreTokens = scoreStr.Split("-");
				if (scoreTokens.Length != 2) return "";
				if (scoreTokens[0] == "" || scoreTokens[1] == "") return "";

				int[] scoreComponents = { int.Parse(scoreTokens[0]), int.Parse(scoreTokens[1]) };

				if (scoreComponents[0] > scoreComponents[1]) return "Win";
				if (scoreComponents[0] < scoreComponents[1]) return "Loss";
				if (scoreComponents[0] == scoreComponents[1]) return "Draw";
			}

			return "";
		}

		public static (string, string, int, int) DescriptionToScores(string description)
		{
			bool isCustom = true;
			string gameMode = "Custom";
			foreach (string mode in Constants.Gamemodes)
			{
				if (description.Contains(mode))
				{
					isCustom = false;
					gameMode = mode;
					break;
				}
			}

			int score = -1;
			int enemyScore = -1;

			if (!isCustom)
			{
				string[] splitDesc = description.Split(" ");
				string scoreStr = "";

				foreach (string token in splitDesc)
				{
					if (!token.Contains("-")) { continue; }
					scoreStr = token;

					string[] scoreTokens = scoreStr.Split("-");
					if (scoreTokens.Length != 2) continue;
					if (scoreTokens[0] == "" || scoreTokens[1] == "") continue;

					(score, enemyScore) = (int.Parse(scoreTokens[0]), int.Parse(scoreTokens[1]));
					break;
				}
			}

			string desc = isCustom ? description : "";
			return (gameMode, desc, score, enemyScore);
		}
	}

	public class HistoryEntryData
	{
		public string SUUID { get; set; }
		public string HUUID { get; set; }
		public string GameMode { get; set; }
		public int Score { get; set; }
		public int EnemyScore { get; set; }
		public string Description { get; set; }
		public long Time { get; set; }
		public int Amount { get; set; }
		public string Map { get; set; }
		public bool SurrenderedWin { get; set; }
		public bool SurrenderedLoss { get; set; }
		public string Result { get; set; }
		public string Title { get; set; }

		public HistoryEntryData(string sUUID, string hUUID, string gameMode, long time, int amount, string map, string result, string description, int score, int enemyScore, bool surrenderedWin, bool surrenderedLoss)
		{
			SUUID = sUUID;
			HUUID = hUUID;
			GameMode = gameMode;
			Time = time;
			Amount = amount;
			Map = map;
			Result = result;
			Description = description;
			Score = score;
			EnemyScore = enemyScore;
			SurrenderedWin = surrenderedWin;
			SurrenderedLoss = surrenderedLoss;

			if (Constants.ScoreTypes[GameMode] == "None") Title = Description;
			else if (Constants.ScoreTypes[GameMode] == "Placement") Title = GameMode + " " + HistoryDataCalc.CalcHistoryResultFromScores("Placement", Score, EnemyScore, SurrenderedWin, SurrenderedLoss);
			else Title = GameMode + " " + Score.ToString() + "-" + EnemyScore.ToString();
		}
	}

	public class HistoryGroupData
	{
		public string SUUID { get; set; }
		public string GUUID { get; set; }
		public long Date { get; set; }
		public ObservableCollection<HistoryEntryData> Entries { get; set; }

		public HistoryGroupData(string sUUID, string gUUID, long date, ObservableCollection<HistoryEntryData> entries)
		{
			SUUID = sUUID;
			GUUID = gUUID;
			Date = date;
			Entries = entries;
		}
	}
}
