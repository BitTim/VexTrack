using System;
using System.Collections.Generic;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class DataInitPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnDoneClicked { get; set; }

		private string _name;
		private long _endDate;
		private double _progress;
		private int _collected;
		private int _activeBpLevel;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}
		public long EndDate
		{
			get => _endDate;
			set
			{
				_endDate = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(RemainingDays));
			}
		}
		public double Progress
		{
			get => _progress;
			set
			{
				_progress = value;
				OnPropertyChanged();
			}
		}
		public int Collected
		{
			get => _collected;
			set
			{
				_collected = value;

				var maxForLevel = CalcUtil.CalcMaxForLevel(ActiveBpLevel);
				if (_collected >= maxForLevel) _collected = maxForLevel - 1;

				CalcProgress();

				OnPropertyChanged();
				OnPropertyChanged(nameof(Progress));
			}
		}
		public int ActiveBpLevel
		{
			get => _activeBpLevel;
			set
			{
				_activeBpLevel = value;
				CalcProgress();
				OnPropertyChanged();
				OnPropertyChanged(nameof(Progress));
			}
		}
		public int RemainingDays => TrackingData.GetRemainingDays("", DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date, true);

		public DataInitPopupViewModel()
		{
			CanCancel = false;

			OnDoneClicked = new RelayCommand(o =>
			{
				CanCancel = true;
				MainVm.InterruptUpdate = false;

				List<HistoryEntry> initList = new();
				var totalCollectedXp = CalcUtil.CalcTotalCollected(ActiveBpLevel, Collected);
				var seasonUuid = Guid.NewGuid().ToString();
				
				initList.Add(new HistoryEntry(seasonUuid, Guid.NewGuid().ToString(), DateTimeOffset.Now.AddDays(-1).ToLocalTime().ToUnixTimeSeconds(), "Custom", totalCollectedXp, "", "Initialization", -1, -1, false, false));
				TrackingData.AddSeason(new Season(seasonUuid, Name, EndDate, ActiveBpLevel, Collected, initList));

				Close();
			});
		}

		public void CalcProgress()
		{
			var total = CalcUtil.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XpPerLevel);
			var collected = CalcUtil.CalcTotalCollected(ActiveBpLevel, Collected);
			Progress = CalcUtil.CalcProgress(total, collected);
		}

		public void InitData()
		{
			EndDate = DateTimeOffset.Now.AddDays(61).ToUnixTimeSeconds();

			Name = "";
			Collected = 0;
			ActiveBpLevel = 2;

			IsInitialized = true;
		}
	}
}
