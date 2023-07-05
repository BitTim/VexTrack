using System.Linq;
using VexTrack.Core.Model.Game;

namespace VexTrack.Core.Model;

public class HistoryEntry
{
	public string GroupUuid { get; set; }
    public string Uuid { get; set; }
    public long Time { get; set; }
    public GameMode GameMode { get; set; }
    public int Amount { get; set; }
    public Map Map { get; set; }
    public string Description { get; set; }
    public int Score { get; set; }
    public int EnemyScore { get; set; }
    public bool SurrenderedWin { get; set; }
    public bool SurrenderedLoss { get; set; }


    public string Title => GetTitle();
    public string Result => GetResult();
    

    public HistoryEntry(string groupUuid, string uuid, long time, GameMode gamemode, int amount, Map map, string desc, int score, int enemyScore, bool surrenderedWin, bool surrenderedLoss)
    {
	    GroupUuid = groupUuid;
        Uuid = uuid;
        Time = time;
        GameMode = gamemode;
        Amount = amount;
        Map = map;
        Description = desc;
        Score = score;
        EnemyScore = enemyScore;
        SurrenderedWin = surrenderedWin;
        SurrenderedLoss = surrenderedLoss;
    }



    public string GetTitle()
    {
	    return GameMode.ScoreType switch
	    {
		    "None" => Description,
		    "Placement" => GameMode.Name + " " + CalcHistoryResultFromScores("Placement", Score, EnemyScore, SurrenderedWin, SurrenderedLoss),
		    _ => GameMode.Name + " " + Score + "-" + EnemyScore
	    };
    }
    
    public string GetResult()
    {
	    return Score == -1 ? CalcHistoryResultFromString(Description) : CalcHistoryResultFromScores(GameMode.ScoreType, Score, EnemyScore, SurrenderedWin, SurrenderedLoss);
    }
    
    
	public static string CalcHistoryResultFromScores(string scoreType, int score, int enemyScore, bool surrenderedWin, bool surrenderedLoss)
	{
		switch (scoreType)
		{
			case "Score" when surrenderedWin:
				return "Surrendered Win";
			case "Score" when surrenderedLoss:
				return "Surrendered Loss";
			case "Placement":
			case "None":
				enemyScore = -1;
				break;
		}

		if (scoreType == "None") score = -1;

		if (enemyScore == -1)
		{
			if (score == -1) return "";

			var scoreString = score.ToString();

			var suffix = scoreString.Last() switch
			{
				'1' => "st",
				'2' => "nd",
				'3' => "rd",
				_ => "th"
			};

			if (scoreString.Length > 1 && scoreString[^2] == '1') suffix = "th";
			return scoreString + suffix;
		}

		if (score > enemyScore) return "Win";
		if (score < enemyScore) return "Loss";
		return score == enemyScore ? "Draw" : "";
	}

	private static string CalcHistoryResultFromString(string description)
	{
		if (string.IsNullOrEmpty(description)) return "";

		var splitDesc = description.Split(" ");

		foreach (var token in splitDesc)
		{
			if (!token.Contains("-")) { continue; }

			if (token == "") return "";

			var scoreTokens = token.Split("-");
			if (scoreTokens.Length != 2) return "";
			if (scoreTokens[0] == "" || scoreTokens[1] == "") return "";

			int[] scoreComponents = { int.Parse(scoreTokens[0]), int.Parse(scoreTokens[1]) };

			if (scoreComponents[0] > scoreComponents[1]) return "Win";
			if (scoreComponents[0] < scoreComponents[1]) return "Loss";
			if (scoreComponents[0] == scoreComponents[1]) return "Draw";
		}

		return "";
	}
	
	public static (GameMode, string, int, int) DescriptionToScores(string description)
	{
		var isCustom = true;
		var gameMode = ApiData.GameModes.Find(gm => gm.Name == "Custom");
		foreach (var mode in ApiData.GameModes.Where(gm => description.Contains(gm.Name)))
		{
			isCustom = false;
			gameMode = mode;
			break;
		}

		var score = -1;
		var enemyScore = -1;

		if (!isCustom)
		{
			var splitDesc = description.Split(" ");

			foreach (var token in splitDesc)
			{
				if (!token.Contains('-')) { continue; }

				var scoreTokens = token.Split("-");
				if (scoreTokens.Length != 2) continue;
				if (scoreTokens[0] == "" || scoreTokens[1] == "") continue;

				(score, enemyScore) = (int.Parse(scoreTokens[0]), int.Parse(scoreTokens[1]));
				break;
			}
		}

		var desc = isCustom ? description : "";
		return (gameMode, desc, score, enemyScore);
	}
}