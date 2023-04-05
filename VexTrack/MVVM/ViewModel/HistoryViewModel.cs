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
		private string _initUuid;

		public RelayCommand HistoryButtonClick { get; }
		public RelayCommand OnAddClicked { get; }
		private HistoryEntryPopupViewModel HePopup { get; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; }
		private MainViewModel MainVm { get; }
		public ObservableCollection<HistoryGroup> Groups { get; } = new();

		public HistoryViewModel()
		{
			MainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
			HePopup = (HistoryEntryPopupViewModel)ViewModelManager.ViewModels[nameof(HistoryEntryPopupViewModel)];
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels[nameof(EditableHistoryEntryPopupViewModel)];

			HistoryButtonClick = new RelayCommand(OnHistoryButtonClick);
			OnAddClicked = new RelayCommand(_ =>
			{
				EditableHePopup.SetParameters("Create History Entry", false);
				MainVm.QueuePopup(EditableHePopup);
			});

			Update();
		}

		public void Update()
		{
			List<HistoryGroup> removedGroups = new();
			foreach (var historyGroup in Groups)
			{
				var removedEntries = historyGroup.Entries.Where(ge => TrackingData.CurrentSeasonData.History.All(e => e.Uuid != ge.Uuid)).ToList();
				foreach (var r in removedEntries) historyGroup.Entries.Remove(r);

				if (historyGroup.Entries.Count == 0) removedGroups.Add(historyGroup);
			}

			foreach (var g in removedGroups)
				Groups.Remove(g);

			List<Season> seasons = new();
			if (SettingsHelper.Data.SingleSeasonHistory) seasons.Add(TrackingData.CurrentSeasonData);
			else seasons = TrackingData.Seasons;

			foreach (var season in seasons)
			{
				foreach (var he in season.History)
				{
					if (!(from g in Groups
						  from e in g.Entries
						  where e.Uuid == he.Uuid
						  select e).Any())
					{
						var hed = new HistoryEntry(he.SeasonUuid, he.Uuid, he.Time, he.GameMode, he.Amount, he.Map, he.Description, he.Score, he.EnemyScore, he.SurrenderedWin, he.SurrenderedLoss);

						InsertEntry(hed);
					}
				}
			}

			_initUuid = Groups.Last().Entries.Last().Uuid;

			var entry = (from g in Groups
									  from e in g.Entries
									  where e.Uuid == HePopup.Uuid
									  select e).FirstOrDefault();

			if (HePopup.IsInitialized && entry != null) HePopup.SetData(entry, _initUuid);
			else HePopup.Close();
		}

		private void InsertEntry(HistoryEntry data)
		{
			DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(data.Time).ToLocalTime().Date;

			var activeGroup = Groups.FirstOrDefault(historyGroup => historyGroup.Date == date.ToUnixTimeSeconds());
			if (activeGroup != null)
			{
				var gIndex = Groups.IndexOf(activeGroup);
				var entries = Groups[gIndex].Entries.ToList();
				entries.Add(data);
				entries = entries.OrderByDescending(e => e.Time).ToList();

				var eIndex = entries.IndexOf(data);
				Groups[gIndex].Entries.Insert(eIndex, data);
			}
			else
			{
				ObservableCollection<HistoryEntry> entries = new();
				entries.Insert(0, data);

				HistoryGroup groupData = new(date.ToUnixTimeSeconds(), entries);

				var groups = Groups.ToList();
				groups.Add(groupData);
				groups = groups.OrderByDescending(g => g.Date).ToList();

				var iIndex = groups.IndexOf(groupData);
				Groups.Insert(iIndex, groupData);
			}
		}

		public void EditEntry(HistoryEntry data)
		{
			foreach (var g in Groups)
			{
				var obj = g.Entries.FirstOrDefault(e => e.Uuid == data.Uuid);
				if (obj == null) continue;
				
				g.Entries.Remove(obj);
				break;
			}

			InsertEntry(data);
		}

		private void OnHistoryButtonClick(object parameter)
		{
			var hUuid = (string)parameter;

			HePopup.SetData((from g in Groups
							 from e in g.Entries
							 where e.Uuid == hUuid
							 select e).FirstOrDefault(), _initUuid);

			MainVm.QueuePopup(HePopup);
		}
	}
}
