using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class HistoryViewModel : ObservableObject
	{
		private string initUUID;

		public RelayCommand HistoryButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }
		private HistoryEntryPopupViewModel HEPopup { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHEPopup { get; set; }
		private MainViewModel MainVM { get; set; }

		private ObservableCollection<HistoryGroupData> _groups = new();
		public ObservableCollection<HistoryGroupData> Groups
		{
			get => _groups;
			set
			{
				if (_groups != value)
				{
					_groups = value;
					OnPropertyChanged();
				}
			}
		}

		public HistoryViewModel()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			HEPopup = (HistoryEntryPopupViewModel)ViewModelManager.ViewModels["HEPopup"];
			EditableHEPopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];

			HistoryButtonClick = new RelayCommand(OnHistoryButtonClick);
			OnAddClicked = new RelayCommand(o =>
			{
				EditableHEPopup.SetParameters("Create History Entry", false);
				MainVM.QueuePopup(EditableHEPopup);
			});

			Update();
		}

		public void Update()
		{
			List<HistoryGroupData> removedGroups = new();
			foreach (HistoryGroupData g in Groups)
			{
				List<HistoryEntryData> removedEntries = g.Entries.Where(ge => TrackingDataHelper.CurrentSeasonData.History.All(e => e.UUID != ge.HUUID)).ToList();
				foreach (HistoryEntryData r in removedEntries) g.Entries.Remove(r);

				if (g.Entries.Count == 0) removedGroups.Add(g);
			}

			foreach (HistoryGroupData g in removedGroups)
				Groups.Remove(g);

			foreach (HistoryEntry he in TrackingDataHelper.CurrentSeasonData.History)
			{
				if (!(from g in Groups
					  from e in g.Entries
					  where e.HUUID == he.UUID
					  select e).Any())
				{
					string result = HistoryDataCalc.CalcHistoryResultFromScores(Constants.ScoreTypes[he.GameMode], he.Score, he.EnemyScore, he.SurrenderedWin, he.SurrenderedLoss);
					HistoryEntryData hed = new HistoryEntryData(TrackingDataHelper.CurrentSeasonUUID, he.UUID, he.GameMode, he.Time, he.Amount, he.Map, result, he.Description, he.Score, he.EnemyScore, he.SurrenderedWin, he.SurrenderedLoss);

					InsertEntry(hed);
				}
			}

			initUUID = Groups.Last().Entries.Last().HUUID;

			HistoryEntryData entry = (from g in Groups
									  from e in g.Entries
									  where e.HUUID == HEPopup.HUUID
									  select e).FirstOrDefault();

			if (HEPopup.IsInitialized && entry != null) HEPopup.SetData(entry, initUUID);
			else HEPopup.Close();
		}

		public void InsertEntry(HistoryEntryData data)
		{
			DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(data.Time).ToLocalTime().Date;

			HistoryGroupData activeGroup = Groups.FirstOrDefault(g => g.Date == date.ToUnixTimeSeconds());
			if (activeGroup != null)
			{
				int gIndex = Groups.IndexOf(activeGroup);
				List<HistoryEntryData> entries = Groups[gIndex].Entries.ToList();
				entries.Add(data);
				entries = entries.OrderByDescending(e => e.Time).ToList();

				int eIndex = entries.IndexOf(data);
				Groups[gIndex].Entries.Insert(eIndex, data);
			}
			else
			{
				ObservableCollection<HistoryEntryData> entries = new();
				entries.Insert(0, data);

				HistoryGroupData groupData = new(TrackingDataHelper.CurrentSeasonUUID, Guid.NewGuid().ToString(), date.ToUnixTimeSeconds(), entries);

				List<HistoryGroupData> groups = Groups.ToList();
				groups.Add(groupData);
				groups = groups.OrderByDescending(g => g.Date).ToList();

				int iIndex = groups.IndexOf(groupData);
				Groups.Insert(iIndex, groupData);
			}
		}

		public void EditEntry(HistoryEntryData data)
		{
			foreach(HistoryGroupData g in Groups)
			{
				HistoryEntryData obj = g.Entries.FirstOrDefault(e => e.HUUID == data.HUUID);
				if(obj != null)
				{
					g.Entries.Remove(obj);
					break;
				}
			}

			InsertEntry(data);
		}

		public void OnHistoryButtonClick(object parameter)
		{
			string hUUID = (string)parameter;

			HEPopup.SetData((from g in Groups
							 from e in g.Entries
							 where e.HUUID == hUUID
							 select e).FirstOrDefault(), initUUID);

			MainVM.QueuePopup(HEPopup);
		}
	}
}
