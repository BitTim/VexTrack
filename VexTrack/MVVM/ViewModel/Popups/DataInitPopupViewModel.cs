using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private int _activeBPLevel;

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

				int maxForLevel = CalcUtil.CalcMaxForLevel(ActiveBPLevel);
				if(_collected >= maxForLevel) _collected = maxForLevel - 1;
				
				CalcProgress();

				OnPropertyChanged();
				OnPropertyChanged(nameof(Progress));
			}
		}
		public int ActiveBPLevel
		{
			get => _activeBPLevel;
			set
			{
				_activeBPLevel = value;
				CalcProgress();
				OnPropertyChanged();
				OnPropertyChanged(nameof(Progress));
			}
		}
		public int RemainingDays => TrackingDataHelper.GetRemainingDays("", DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date, true);

		public DataInitPopupViewModel()
		{
			CanCancel = false;

			OnDoneClicked = new RelayCommand(o => {
				CanCancel = true;
				MainVM.InterruptUpdate = false;

				string endDate = DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date.ToString("d");
				List<HistoryEntry> initList = new();
				int totalCollectedXP = CalcUtil.CalcTotalCollected(ActiveBPLevel, Collected);
				initList.Add(new HistoryEntry(Guid.NewGuid().ToString(), DateTimeOffset.Now.AddDays(-1).ToLocalTime().ToUnixTimeSeconds(), "Initialization", totalCollectedXP, ""));

				TrackingDataHelper.AddSeason(new Season(Guid.NewGuid().ToString(), Name, endDate, ActiveBPLevel, Collected, initList));

				Close();
			});
		}

		public void CalcProgress()
		{
			int total = CalcUtil.CumulativeSum(Constants.BattlepassLevels, Constants.Level2Offset, Constants.XPPerLevel);
			int collected = CalcUtil.CalcTotalCollected(ActiveBPLevel, Collected);
			Progress =  CalcUtil.CalcProgress(total, collected);
		}

		public void InitData()
		{
			EndDate = DateTimeOffset.Now.AddDays(61).ToUnixTimeSeconds();

			Name = "";
			Collected = 0;
			ActiveBPLevel = 2;

			IsInitialized = true;
		}
	}
}
