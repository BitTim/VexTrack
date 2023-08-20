using System;
using System.Collections.Generic;
using System.Linq;
using VexTrack.Core;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.Game.Templates;
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
				
			var goals = new List<Goal>();
			var maxLevel = Constants.BattlepassLevels + Constants.EpilogueLevels;
			var startLevel = ActiveBpLevel - 1;
			var endLevel = ActiveBpLevel + 2;

			if (ActiveBpLevel > maxLevel - 3)
			{
				startLevel = maxLevel - 2;
				endLevel = maxLevel + 1;
			}

			for (var i = startLevel; i < endLevel; i++)
			{
				if (i > maxLevel) break;
				var levelTotal = CalcHelper.CalcMaxForLevel(i);
				var goal = new Goal(new GoalTemplate(new List<Reward> {new("", "", 0, true)}, false, 0, levelTotal, true, 300), Guid.NewGuid().ToString(), ActiveBpLevel <= i ? ActiveBpLevel == i ? Collected : 0 : levelTotal);
				goals.Add(goal);
			}
			
			UserData.AddSeason(new Season(seasonUuid, Name, -1, EndTimestamp, goals));
			UserData.AddHistoryEntry(new HistoryEntry("", Guid.NewGuid().ToString(), TimeHelper.TodayDate.AddDays(-1).ToUnixTimeSeconds(), ApiData.GameModes.Find(gm => gm.Name == "Custom"), totalCollectedXp, ApiData.Maps.Last(), "Initialization", -1, -1, false, false));

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