using System;
using System.Collections.Generic;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableHistoryEntryPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string Title { get; set; }
		public string SUUID { get; set; }
		public string HUUID { get; set; }
		public List<string> Maps => Constants.Maps;
		public List<string> GameModes => Constants.Gamemodes;
		public string ScoreType => Constants.ScoreTypes[GameMode];
		public bool EditMode { get; set; }

		private string _description;
		private string _gamemode;
		private int _score;
		private int _enemyScore;
		private long _time;
		private int _amount;
		private string _map;
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
		public string GameMode
		{
			get => _gamemode;
			set
			{
				_gamemode = value;
				Score = 0;
				EnemyScore = 0;
				Description = "";
				OnPropertyChanged();
				OnPropertyChanged(nameof(Score));
				OnPropertyChanged(nameof(EnemyScore));
				OnPropertyChanged(nameof(Description));
				OnPropertyChanged(nameof(ScoreType));
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
		public string Map
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
				if (_surrenderedWin == true) SurrenderedLoss = false;

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
				if(_surrenderedLoss == true) SurrenderedWin = false;

				OnPropertyChanged();
				OnPropertyChanged(nameof(Result));
				OnPropertyChanged(nameof(SurrenderedWin));
			}
		}

		public string Result => HistoryDataCalc.CalcHistoryResultFromScores(ScoreType, Score, EnemyScore, SurrenderedWin, SurrenderedLoss);

		public EditableHistoryEntryPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o =>
			{
				if (ScoreType == "Placement" || ScoreType == "None") EnemyScore = -1;
				if (ScoreType == "None") Score = -1;
				if (ScoreType == "Score") Description = "";

				if (EditMode) TrackingDataHelper.EditHistoryEntry(SUUID, HUUID, new HistoryEntry(HUUID, Time, GameMode, Amount, Map, Description, Score, EnemyScore, SurrenderedWin, SurrenderedLoss));
				else TrackingDataHelper.AddHistoryEntry(SUUID, new HistoryEntry(HUUID, Time, GameMode, Amount, Map, Description, Score, EnemyScore, SurrenderedWin, SurrenderedLoss));
				Close();
			});
		}

		public void SetParameters(string title, bool editMode)
		{
			Title = title;
			EditMode = editMode;

			if (!EditMode)
				InitData();
		}

		public void InitData()
		{
			Time = DateTimeOffset.Now.ToUnixTimeSeconds();

			SUUID = TrackingDataHelper.CurrentSeasonUUID;
			HUUID = Guid.NewGuid().ToString();
			Description = "";
			GameMode = Constants.Gamemodes[0];
			Map = Constants.Maps[0];
			Amount = 0;
			Score = 0;
			EnemyScore = 0;
			SurrenderedWin = false;
			SurrenderedLoss = false;

			IsInitialized = true;
		}

		public void SetData(HistoryEntryData data)
		{
			SUUID = data.SUUID;
			HUUID = data.HUUID;
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
}