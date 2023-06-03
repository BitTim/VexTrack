using System;
using System.Collections.Generic;
using System.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.IO.UserData;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core.Model;

public abstract class UserData
{
	public static int Streak { get; set; }
	public static long LastStreakUpdateTimestamp { get; set; }
	public static List<Contract> Contracts { get; private set; }
	public static List<Season> Seasons { get; private set; }
	public static List<HistoryGroup> History { get; set; }

	protected UserData(int streak, long lastStreakUpdateTimestamp,List<Contract> contracts, List<Season> seasons, List<HistoryGroup> history)
	{
		(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History) = (streak, lastStreakUpdateTimestamp, contracts, seasons, history);
	}

	public static Season CurrentSeasonData => Seasons?.Last();
		
		

	// ================================
	//  Init and Convert
	// ================================

	internal static void SetData(int streak, long lastStreakUpdateTimestamp, List<Contract> contracts, List<Season> seasons, List<HistoryGroup> history)
	{
		(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History) = (streak, lastStreakUpdateTimestamp ,contracts, seasons, history);
		Recalculate();
	}

	internal static void InitData()
	{
		List<Contract> contracts = new();
		List<Season> seasons = new();
		List<HistoryGroup> history = new();

		SetData(0, TimeHelper.TodayTimestamp, contracts, seasons, history);
	}
		
		
		
	// ================================
	//  Updating
	// ================================

	private static void Recalculate() // TODO: Restructure this to have Update() methods in Season and Contracts
	{
		List<string> completed = new();
		List<string> lost = new();

		//Get Completed goals
		var completedGoals = (from contract in Contracts from goal in contract.Goals where goal.Collected >= goal.Total select goal.Uuid).ToList();

		//Recalculate total collected XP, collected XP in level and current level
		var prevTotalXp = CalcHelper.CalcTotalCollected(CurrentSeasonData.ActiveBpLevel, CurrentSeasonData.Cxp);

		var iter = 2;
		var cxp = HistoryHelper.GetAllEntriesFromSeason(CurrentSeasonData.Uuid).Sum(he => he.Amount);

		while (cxp >= 0) // TODO: Find better way to figure out what battlepass level were gained / lost
		{
			if (iter <= Constants.BattlepassLevels) cxp -= Constants.Level2Offset + (iter * Constants.XpPerLevel);
			else if (iter < Constants.BattlepassLevels + Constants.EpilogueLevels + 2) cxp -= Constants.XpPerEpilogueLevel;
			else break;
				
			iter++;
		}
		iter--;

		for (var i = CurrentSeasonData.ActiveBpLevel - 1; i > iter - 1; i--)
			lost.Add("Battlepass Level " + i.ToString());

		for (var i = CurrentSeasonData.ActiveBpLevel; i < iter; i++)
			completed.Add("Battlepass Level " + i.ToString());

		CurrentSeasonData.ActiveBpLevel = iter;

		if (iter < Constants.BattlepassLevels)
			cxp += Constants.Level2Offset + (iter * Constants.XpPerLevel);
		else if (iter < Constants.BattlepassLevels + Constants.EpilogueLevels + 2)
			cxp += Constants.XpPerEpilogueLevel;

		CurrentSeasonData.Cxp = cxp;

		//Calculate difference in XP and apply to goals
		var currTotalXp = CalcHelper.CalcTotalCollected(CurrentSeasonData.ActiveBpLevel, CurrentSeasonData.Cxp);
		var deltaXp = currTotalXp - prevTotalXp;

			
		foreach (var contract in Contracts) // TODO: Move contents to update() function within contracts
		{
			if (contract.Paused) continue;
			var xpPool = Math.Abs(deltaXp);
				
			foreach (var goal in contract.Goals)
			{
				var goalLimit = deltaXp > 0 ? goal.Total - goal.Collected : goal.Collected;

				var appliedXp = xpPool > goalLimit ? goalLimit : xpPool;
				appliedXp *= Math.Sign(deltaXp);
				xpPool -= appliedXp;
						
				var newCollected = goal.Collected + appliedXp;
				if (newCollected < 0) newCollected = 0;

				goal.Collected = newCollected;

				if (goal.Collected >= goal.Total && !completedGoals.Contains(goal.Uuid)) completed.Add(goal.Name);
				if (goal.Collected < goal.Total && completedGoals.Contains(goal.Uuid)) lost.Add(goal.Name);
			}
		}

		if (completed.Count == 0 && lost.Count == 0) return;
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		var paPopupVm = (ProgressActivityPopupViewModel)ViewModelManager.ViewModels[nameof(ProgressActivityPopupViewModel)];

		paPopupVm.SetData(completed, lost);
		mainVm.QueuePopup(paPopupVm);
	}

	private static void CallUpdate()
	{
		UserDataSaver.SaveUserData(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History);
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		mainVm.Update();
	}

	public static void ResetData()
	{
		InitData();
		UserDataSaver.SaveUserData(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History);
		UserDataLoader.LoadUserData();
		CallUpdate();
	}

		
		
	// ================================
	//  Modifying
	// ================================
		
	public static void AddHistoryEntry(HistoryEntry data)
	{
		var seasonUuid = Seasons.Find(s => data.Time > s.StartTimestamp && data.Time < s.EndTimestamp)?.Uuid;

		if (string.IsNullOrEmpty(seasonUuid))
		{
			seasonUuid = CurrentSeasonData.Uuid;
			data.Time = TimeHelper.NowTimestamp;
		}
			
		var date = TimeHelper.IsolateTimestampDate(data.Time);
		var groupUuid = History.Find(hg => hg.Date == date && hg.SeasonUuid == seasonUuid)?.Uuid;
					
		if (string.IsNullOrEmpty(groupUuid))
		{
			groupUuid = Guid.NewGuid().ToString();
			History.Add(new HistoryGroup(seasonUuid, groupUuid, date, new List<HistoryEntry>()));
		}

		data.GroupUuid = groupUuid;
		History.Find(hg => hg.Uuid == groupUuid).Entries.Add(data);

		HistoryHelper.SortHistory();
			
		Recalculate();
		CallUpdate();
	}

	public static void RemoveHistoryEntry(string groupUuid, string uuid)
	{
		var hgIdx = History.FindIndex(hg => hg.Uuid == groupUuid);
		var heIdx = History[hgIdx].Entries.FindIndex(he => he.Uuid == uuid);
		History[hgIdx].Entries.RemoveAt(heIdx);

		if(History[hgIdx].Entries.Count < 1) History.RemoveAt(hgIdx);
			
		Recalculate();
		CallUpdate();
	}

	public static void EditHistoryEntry(string groupUuid, HistoryEntry data)
	{
		RemoveHistoryEntry(groupUuid, data.Uuid);
		AddHistoryEntry(data);
			
		Recalculate();
		CallUpdate();
	}

		
		
	public static void AddContract(Contract data)
	{
		Contracts.Add(data);
		CallUpdate();
	}

	// TODO: Update for Contracts
	public static void RemoveContract(string uuid)
	{
		Contracts.RemoveAt(Contracts.FindIndex(gg => gg.Uuid == uuid));
		CallUpdate();
	}

	// TODO: Update for Contracts
	public static void EditContract(string uuid, Contract data)
	{
		var index = Contracts.FindIndex(c => c.Uuid == uuid);
		if (index < 0) return;
			
		Contracts[index] = data;
		CallUpdate();
	}



	public static void AddSeason(Season data)
	{
		Seasons.Add(data);
		CallUpdate();
	}

	public static void EndSeason(string uuid)
	{
		// Set end date to today
		Seasons[Seasons.FindIndex(s => s.Uuid == uuid)].EndTimestamp = TimeHelper.TodayTimestamp;
		CallUpdate();
	}


	internal static void CreateDataInitPopup()
	{
		var dataInitPopup = (DataInitPopupViewModel)ViewModelManager.ViewModels[nameof(DataInitPopupViewModel)];
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];

		dataInitPopup.InitData();
		dataInitPopup.CanCancel = false;
		mainVm.InterruptUpdate = true;

		mainVm.QueuePopup(dataInitPopup);
	}
}
	
	
	
public class LegacyStreakEntry
{
	public long Date { get; set; }
	public string Status { get; set; }

	public LegacyStreakEntry(long date, string status)
	{
		Date = date;
		Status = status;
	}
}