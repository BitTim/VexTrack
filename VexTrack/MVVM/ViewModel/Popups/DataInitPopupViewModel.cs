using System;
using System.Collections.Generic;
using VexTrack.Core;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;
using VexTrack.Core.Util;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class DataInitPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnDoneClicked { get; }

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

				var maxForLevel = CalcHelper.CalcMaxForLevel(ActiveBpLevel);
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
		public int RemainingDays => (DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date - DateTimeOffset.Now.Date.ToLocalTime()).Days;

		public DataInitPopupViewModel()
		{
			CanCancel = false;

			OnDoneClicked = new RelayCommand(_ =>
			{
				CanCancel = true;
				MainVm.InterruptUpdate = false;

				List<HistoryEntry> initList = new();
				var totalCollectedXp = CalcHelper.CalcTotalCollected(ActiveBpLevel, Collected);
				var seasonUuid = Guid.NewGuid().ToString();
				
				Tracking.AddSeason(new Season(seasonUuid, Name, EndDate, ActiveBpLevel, Collected));
				Tracking.AddHistoryEntry(new HistoryEntry("", Guid.NewGuid().ToString(), DateTimeOffset.Now.AddDays(-1).ToLocalTime().ToUnixTimeSeconds(), "Custom", totalCollectedXp, "", "Initialization", -1, -1, false, false));

				Close();
			});
		}

		private void CalcProgress()
		{
			var total = CalcHelper.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XpPerLevel);
			var collected = CalcHelper.CalcTotalCollected(ActiveBpLevel, Collected);
			Progress = CalcHelper.CalcProgress(total, collected);
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
