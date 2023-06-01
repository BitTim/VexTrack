using System;
using VexTrack.Core;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class DataInitPopupViewModel : BasePopupViewModel
{
	public RelayCommand OnDoneClicked { get; }

	private string _name;
	private long _endTimestamp;
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
	public long EndTimestamp
	{
		get => _endTimestamp;
		set
		{
			_endTimestamp = value;
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
	public int RemainingDays => (TimeHelper.TimestampToDate(EndTimestamp) - TimeHelper.TodayDate).Days;

	public DataInitPopupViewModel()
	{
		CanCancel = false;

		OnDoneClicked = new RelayCommand(_ =>
		{
			CanCancel = true;
			MainVm.InterruptUpdate = false;

			var totalCollectedXp = CalcHelper.CalcTotalCollected(ActiveBpLevel, Collected);
			var seasonUuid = Guid.NewGuid().ToString();
				
			UserData.AddSeason(new Season(seasonUuid, Name, EndTimestamp, ActiveBpLevel, Collected));
			UserData.AddHistoryEntry(new HistoryEntry("", Guid.NewGuid().ToString(), TimeHelper.TodayDate.AddDays(-1).ToUnixTimeSeconds(), "Custom", totalCollectedXp, "", "Initialization", -1, -1, false, false));

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
		EndTimestamp = TimeHelper.TodayDate.AddDays(61).ToUnixTimeSeconds();

		Name = "";
		Collected = 0;
		ActiveBpLevel = 2;

		IsInitialized = true;
	}
}