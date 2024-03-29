using System;
using System.Collections.Generic;
using System.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.Game;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class EditableHistoryEntryPopupViewModel : BasePopupViewModel
{
	public RelayCommand OnBackClicked { get; }
	public RelayCommand OnDoneClicked { get; }

	public string Title { get; private set; }
	private string Uuid { get; set; }
	private string GroupUuid { get; set; }
	public string ScoreType => GameMode != null ? GameMode.ScoreType : "";
	private bool EditMode { get; set; }

	private string _description;
	private GameMode _gamemode;
	private int _score;
	private int _enemyScore;
	private long _time;
	private int _amount;
	private Map _map;
	private bool _surrenderedWin;
	private bool _surrenderedLoss;

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			OnPropertyChanged();
		}
	}
	public GameMode GameMode
	{
		get => _gamemode;
		set
		{
			_gamemode = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(Score));
			OnPropertyChanged(nameof(EnemyScore));
			OnPropertyChanged(nameof(Description));
			OnPropertyChanged(nameof(ScoreType));
			OnPropertyChanged(nameof(Maps));

			Map = Maps.First();
			OnPropertyChanged(nameof(Map));
		}
	}
	public int Score
	{
		get => _score;
		set
		{
			_score = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(Result));
		}
	}
	public int EnemyScore
	{
		get => _enemyScore;
		set
		{
			_enemyScore = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(Result));
		}
	}
	public long Time
	{
		get => _time;
		set
		{
			_time = value;
			OnPropertyChanged();
		}
	}
	public int Amount
	{
		get => _amount;
		set
		{
			_amount = value;
			OnPropertyChanged();
		}
	}
	public Map Map
	{
		get => _map;
		set
		{
			_map = value;
			OnPropertyChanged();
		}
	}
	public bool SurrenderedWin
	{
		get => _surrenderedWin;
		set
		{
			_surrenderedWin = value;
			if (_surrenderedWin) SurrenderedLoss = false;

			OnPropertyChanged();
			OnPropertyChanged(nameof(Result));
			OnPropertyChanged(nameof(SurrenderedLoss));
		}
	}
	public bool SurrenderedLoss
	{
		get => _surrenderedLoss;
		set
		{
			_surrenderedLoss = value;
			if (_surrenderedLoss) SurrenderedWin = false;

			OnPropertyChanged();
			OnPropertyChanged(nameof(Result));
			OnPropertyChanged(nameof(SurrenderedWin));
		}
	}

	public string Result => HistoryEntry.CalcHistoryResultFromScores(ScoreType, Score, EnemyScore, SurrenderedWin, SurrenderedLoss);
	public List<Map> Maps => ApiData.Maps.Where(map => map.Type == GameMode.MapType || map.Type == "all" || GameMode.MapType == "all").ToList();

	public EditableHistoryEntryPopupViewModel()
	{
		CanCancel = true;

		OnBackClicked = new RelayCommand(_ => { if (CanCancel) Close(); });
		OnDoneClicked = new RelayCommand(_ =>
		{
			if (ScoreType is "Placement" or "None") EnemyScore = -1;
			if (ScoreType == "None") Score = -1;
			if (ScoreType == "Score") Description = "";

			if (EditMode) UserData.EditHistoryEntry(GroupUuid, new HistoryEntry(GroupUuid, Uuid, Time, GameMode, Amount, Map, Description, Score, EnemyScore, SurrenderedWin, SurrenderedLoss));
			else UserData.AddHistoryEntry(new HistoryEntry(GroupUuid, Uuid, Time, GameMode, Amount, Map, Description, Score, EnemyScore, SurrenderedWin, SurrenderedLoss));
			Close();
		});
	}

	public void SetParameters(string title, bool editMode)
	{
		Title = title;
		EditMode = editMode;

		if (!editMode) InitData();
	}

	private void InitData()
	{
		Time = TimeHelper.NowTimestamp;

		GroupUuid = "";
		Uuid = Guid.NewGuid().ToString();
		Description = "";
		GameMode = ApiData.GameModes.First();
		Map = Maps.First();
		Amount = 0;
		Score = 0;
		EnemyScore = 0;
		SurrenderedWin = false;
		SurrenderedLoss = false;

		IsInitialized = true;
	}

	public void SetData(HistoryEntry data)
	{
		GroupUuid = data.GroupUuid;
		Uuid = data.Uuid;
		GameMode = data.GameMode;
		Score = data.Score;
		EnemyScore = data.EnemyScore;
		Time = data.Time;
		Amount = data.Amount;
		Map = data.Map;
		Description = data.Description;
		SurrenderedWin = data.SurrenderedWin;
		SurrenderedLoss = data.SurrenderedLoss;

		IsInitialized = true;
	}
}