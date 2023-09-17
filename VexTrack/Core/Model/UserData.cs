using System;
using System.Collections.Generic;
using System.IO;
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

	public static Season CurrentSeasonData => Seasons?.FirstOrDefault();
		
		

	// ================================
	//  Init and Convert
	// ================================

	internal static void SetData(int streak, long lastStreakUpdateTimestamp, List<Contract> contracts, List<Season> seasons, List<HistoryGroup> history)
	{
		(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History) = (streak, lastStreakUpdateTimestamp ,contracts, seasons, history);
		Recalculate();
	}
		
		
		
	// ================================
	//  Updating
	// ================================

	private static void Recalculate() // TODO: Restructure this to have Update() methods in Season and Contracts
	{
		List<string> completed = new();
		List<string> lost = new();

		//Get Completed goals
		var completedSeasonGoals = (from season in Seasons from goal in season.Goals where goal.Collected >= goal.Total select goal.Uuid).ToList();
		var completedContractGoals = (from contract in Contracts from goal in contract.Goals where goal.Collected >= goal.Total select goal.Uuid).ToList();

		// Cancel when no current Season data is available
		if (CurrentSeasonData == null) return;
		
		//Calculate XP delta
		var collectedXp = CurrentSeasonData.Collected;
		var prevCollectedXp = CurrentSeasonData.Goals.Sum(g => g.Collected);
		var deltaXp = collectedXp - prevCollectedXp;

		if (prevCollectedXp >= CurrentSeasonData.Goals.Sum(g => g.Total)) deltaXp = 0;

		//Calculate difference in XP and apply to goals

		var xpPool = Math.Abs(deltaXp);

		for (var i = CurrentSeasonData.Goals.Count - 1; i >= 0; i--) // TODO: This sets collected of all goals to 0 when removing XP
		{
			var goal = CurrentSeasonData.Goals[i];
			var goalLimit = deltaXp > 0 ? goal.Remaining : goal.Collected;

			var appliedXp = xpPool > goalLimit ? goalLimit : xpPool;
			appliedXp *= Math.Sign(deltaXp);
			xpPool -= appliedXp;

			var newCollected = goal.Collected + appliedXp;
			if (newCollected < 0) newCollected = 0;

			goal.Collected = newCollected;

			if (goal.Collected >= goal.Total && !completedSeasonGoals.Contains(goal.Uuid)) completed.Add(goal.Name);
			if (goal.Collected < goal.Total && completedSeasonGoals.Contains(goal.Uuid)) lost.Add(goal.Name);
		}

		for (var i = Contracts.Count - 1; i >= 0; i--)
		{
			var contract = Contracts[i];
			xpPool = Math.Abs(deltaXp);

			foreach (var goal in contract.Goals)
			{
				var goalLimit = deltaXp > 0 ? goal.Remaining : goal.Collected;

				var appliedXp = xpPool > goalLimit ? goalLimit : xpPool;
				appliedXp *= Math.Sign(deltaXp);
				xpPool -= appliedXp;

				var newCollected = goal.Collected + appliedXp;
				if (newCollected < 0) newCollected = 0;

				goal.Collected = newCollected;

				if (goal.Collected >= goal.Total && !completedContractGoals.Contains(goal.Uuid))
					completed.Add(goal.Name);
				if (goal.Collected < goal.Total && completedContractGoals.Contains(goal.Uuid)) lost.Add(goal.Name);
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
		if(File.Exists(Constants.DataPath)) File.Delete(Constants.DataPath);
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



	public static void AddSeason(Season data, bool skipUpdate = false)
	{
		var idx = Seasons.FindIndex(s => s.Uuid == data.Uuid);
			
		if (idx != -1) { Seasons[idx] = data; }
		else { Seasons.Insert(0, data); }
		
		if(!skipUpdate) CallUpdate();
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