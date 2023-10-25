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
	}
		
		
		
	// ================================
	//  Updating
	// ================================

	private static void Update(int amount)
	{
		List<string> completed = new();
		List<string> lost = new();

		var allContracts = Seasons.Concat(Contracts);
		
		foreach (var contract in allContracts)
		{
			var xpPool = amount;
			var activeGoalIndex = contract.NextUnlockIndex;

			while (xpPool != 0)
			{
				var goal = contract.Goals[activeGoalIndex];
				goal.Collected += xpPool;

				if (goal.Collected >= goal.Total)
				{
					xpPool = goal.Collected - goal.Total;
					goal.Collected = goal.Total;

					completed.Add(goal.Name);
					activeGoalIndex++;
				}
				else if (goal.Collected <= 0)
				{
					xpPool = goal.Collected;
					goal.Collected = 0;

					if (activeGoalIndex > 1)
					{
						activeGoalIndex--;
						lost.Add(contract.Goals[activeGoalIndex].Name);
					}
					else xpPool = 0;
				}
				else xpPool = 0;
			}
		}
		
		if (completed.Count == 0 && lost.Count == 0) return;
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		var paPopupVm = (ProgressActivityPopupViewModel)ViewModelManager.ViewModels[nameof(ProgressActivityPopupViewModel)];

		paPopupVm.SetData(completed, lost);
		mainVm.QueuePopup(paPopupVm);
	}

	private static void CallMainUpdate()
	{
		UserDataSaver.SaveUserData(Streak, LastStreakUpdateTimestamp, Contracts, Seasons, History);
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		mainVm.Update();
	}

	public static void ResetData()
	{
		if(File.Exists(Constants.DataPath)) File.Delete(Constants.DataPath);
		UserDataLoader.LoadUserData();
		CallMainUpdate();
	}

		
		
	// ================================
	//  Modifying
	// ================================
		
	public static void AddHistoryEntry(HistoryEntry data, bool suppressUpdate = false)
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

		if(!suppressUpdate) Update(data.Amount);
		CallMainUpdate();
	}

	public static void RemoveHistoryEntry(string groupUuid, string uuid, bool suppressUpdate = false)
	{
		var hgIdx = History.FindIndex(hg => hg.Uuid == groupUuid);
		var heIdx = History[hgIdx].Entries.FindIndex(he => he.Uuid == uuid);
		
		var amount = History[hgIdx].Entries[heIdx].Amount;
		History[hgIdx].Entries.RemoveAt(heIdx);

		if(History[hgIdx].Entries.Count < 1) History.RemoveAt(hgIdx);
			
		if(!suppressUpdate) Update(-amount);
		CallMainUpdate();
	}

	public static void EditHistoryEntry(string groupUuid, HistoryEntry data)
	{
		var hgIdx = History.FindIndex(hg => hg.Uuid == groupUuid);
		var heIdx = History[hgIdx].Entries.FindIndex(he => he.Uuid == data.Uuid);
		
		var amount = History[hgIdx].Entries[heIdx].Amount;
		
		RemoveHistoryEntry(groupUuid, data.Uuid, true);
		AddHistoryEntry(data, true);

		Update(data.Amount - amount);
		CallMainUpdate();
	}

		
		
	public static void AddContract(Contract data)
	{
		Contracts.Add(data);
		CallMainUpdate();
	}



	public static void AddSeason(Season data, bool skipUpdate = false)
	{
		var idx = Seasons.FindIndex(s => s.Uuid == data.Uuid);
			
		if (idx != -1) { Seasons[idx] = data; }
		else { Seasons.Insert(0, data); }
		
		if(!skipUpdate) CallMainUpdate();
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