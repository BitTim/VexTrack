using System;
using System.Collections.Generic;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class DataInitPopupViewModel : BasePopupViewModel
{
	public RelayCommand OnDoneClicked { get; }

	private double _progress;
	private int _collected;
	private int _activeLevel;

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

			var maxForLevel = ApiData.ActiveSeasonTemplate.Goals[ActiveLevel - 1].XpTotal;
			if (_collected >= maxForLevel) _collected = maxForLevel - 1;

			CalcProgress();

			OnPropertyChanged();
			OnPropertyChanged(nameof(Progress));
		}
	}
	public int ActiveLevel
	{
		get => _activeLevel;
		set
		{
			_activeLevel = value;
			CalcProgress();
			OnPropertyChanged();
			OnPropertyChanged(nameof(Progress));
		}
	}
	public string Name => ApiData.ActiveSeasonTemplate.Name;
	public long EndTimestamp => ApiData.ActiveSeasonTemplate.EndTimestamp;
	public int RemainingDays => (TimeHelper.TimestampToDate(EndTimestamp) - TimeHelper.TodayDate).Days;

	public DataInitPopupViewModel()
	{
		CanCancel = false;

		OnDoneClicked = new RelayCommand(_ =>
		{
			CanCancel = true;
			MainVm.InterruptUpdate = false;

			var totalCollectedXp = 0;
			List<Goal> goals = new();
			for (var i = 0; i < ApiData.ActiveSeasonTemplate.Goals.Count; i++)
			{
				var template = ApiData.ActiveSeasonTemplate.Goals[i];
				var collected = 0;
				if (i < ActiveLevel - 1) collected = template.XpTotal;
				else if (i == ActiveLevel - 1) collected = Collected;

				totalCollectedXp += collected;
				goals.Add(new Goal(template, Guid.NewGuid().ToString(), collected));
			}
			
			UserData.AddSeason(new Season(ApiData.ActiveSeasonTemplate.Uuid, Name, ApiData.ActiveSeasonTemplate.StartTimestamp, EndTimestamp, totalCollectedXp, goals));
			Close();
		});
	}

	private void CalcProgress()
	{
		var total = ApiData.ActiveSeasonTemplate.XpTotal;
		var collected = CalcHelper.CalcTotalCollected(ActiveLevel, Collected, ApiData.ActiveSeasonTemplate.Goals);
		Progress = CalcHelper.CalcProgress(total, collected);
	}

	public void InitData()
	{
		ActiveLevel = 2;
		Collected = 0;

		IsInitialized = true;
	}
}