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

		public RelayCommand HistoryButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }
		private HistoryEntryPopupViewModel HePopup { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }
		private MainViewModel MainVm { get; set; }

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
			MainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			HePopup = (HistoryEntryPopupViewModel)ViewModelManager.ViewModels["HEPopup"];
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];

			HistoryButtonClick = new RelayCommand(OnHistoryButtonClick);
			OnAddClicked = new RelayCommand(o =>
			{
				EditableHePopup.SetParameters("Create History Entry", false);
				MainVm.QueuePopup(EditableHePopup);
			});

			Update();
		}

		public void Update()
		{
			List<HistoryGroupData> removedGroups = new();
			foreach (var g in Groups)
			{
				var removedEntries = g.Entries.Where(ge => TrackingDataHelper.CurrentSeasonData.History.All(e => e.Uuid != ge.Huuid)).ToList();
				foreach (var r in removedEntries) g.Entries.Remove(r);

				if (g.Entries.Count == 0) removedGroups.Add(g);
			}

			foreach (var g in removedGroups)
				Groups.Remove(g);

			List<Season> seasons = new();
			if (SettingsHelper.Data.SingleSeasonHistory) seasons.Add(TrackingDataHelper.CurrentSeasonData);
			else seasons = TrackingDataHelper.Data.Seasons;

			foreach (var season in seasons)
			{
				foreach (var he in season.History)
				{
					if (!(from g in Groups
						  from e in g.Entries
						  where e.Huuid == he.Uuid
						  select e).Any())
					{
						var result = HistoryDataCalc.CalcHistoryResultFromScores(Constants.ScoreTypes[he.GameMode], he.Score, he.EnemyScore, he.SurrenderedWin, he.SurrenderedLoss);
						var hed = new HistoryEntryData(TrackingDataHelper.CurrentSeasonUuid, he.Uuid, he.GameMode, he.Time, he.Amount, he.Map, result, he.Description, he.Score, he.EnemyScore, he.SurrenderedWin, he.SurrenderedLoss);

						InsertEntry(hed);
					}
				}
			}

			_initUuid = Groups.Last().Entries.Last().Huuid;

			var entry = (from g in Groups
									  from e in g.Entries
									  where e.Huuid == HePopup.Huuid
									  select e).FirstOrDefault();

			if (HePopup.IsInitialized && entry != null) HePopup.SetData(entry, _initUuid);
			else HePopup.Close();
		}

		public void InsertEntry(HistoryEntryData data)
		{
			DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(data.Time).ToLocalTime().Date;

			var activeGroup = Groups.FirstOrDefault(g => g.Date == date.ToUnixTimeSeconds());
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
				ObservableCollection<HistoryEntryData> entries = new();
				entries.Insert(0, data);

				HistoryGroupData groupData = new(TrackingDataHelper.CurrentSeasonUuid, Guid.NewGuid().ToString(), date.ToUnixTimeSeconds(), entries);

				var groups = Groups.ToList();
				groups.Add(groupData);
				groups = groups.OrderByDescending(g => g.Date).ToList();

				var iIndex = groups.IndexOf(groupData);
				Groups.Insert(iIndex, groupData);
			}
		}

		public void EditEntry(HistoryEntryData data)
		{
			foreach (var g in Groups)
			{
				var obj = g.Entries.FirstOrDefault(e => e.Huuid == data.Huuid);
				if (obj != null)
				{
					g.Entries.Remove(obj);
					break;
				}
			}

			InsertEntry(data);
		}

		public void OnHistoryButtonClick(object parameter)
		{
			var hUuid = (string)parameter;

			HePopup.SetData((from g in Groups
							 from e in g.Entries
							 where e.Huuid == hUuid
							 select e).FirstOrDefault(), _initUuid);

			MainVm.QueuePopup(HePopup);
		}
	}
}
