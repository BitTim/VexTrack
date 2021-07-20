using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace VexTrack.Core
{
	class HistoryEntry
	{
		public double time { get; set; }
		public string description { get; set; }
		public int amount { get; set; }

		public HistoryEntry(double time, string desc, int amount)
		{
			(this.time, this.description, this.amount) = (time, desc, amount);
		}
	}

	class Goal
	{
		public string name { get; set; }
		public int remaining { get; set; }
		public int startXP { get; set; }
		public string color { get; set; }

		public Goal(string name, int remaining, int startXP, string color)
		{
			(this.name, this.remaining, this.startXP, this.color) = (name, remaining, startXP, color);
		}
	}

	class Season
	{
		public string name { get; set; }
		public string endDate { get; set; }
		public int activeBPLevel { get; set; }
		public int cXP { get; set; }
		public List<HistoryEntry> xpHistory { get; set; }

		public Season(string name, string endDate, int activeBPLevel, int cXP, List<HistoryEntry> xpHistory)
		{
			(this.name, this.endDate, this.activeBPLevel, this.cXP, this.xpHistory) = (name, endDate, activeBPLevel, cXP, xpHistory);
		}
	}

	class TrackingData
	{
		public List<Goal> goals { get; set; }
		public List<Season> seasons { get; set; }

		public TrackingData(List<Goal> goals, List<Season> seasons)
		{
			(this.goals, this.seasons) = (goals, seasons);
		}
	}

	class TrackingDataHelper
	{
		public TrackingData Tracking { get; set; }

		public void loadData(string path)
		{
			string rawJSON = File.ReadAllText(path);
			JObject o = JObject.Parse(rawJSON);

			var test = from p in o["seasons"] select (string)p["name"];
		}

		public void saveData(string path)
		{

		}
	}
}
