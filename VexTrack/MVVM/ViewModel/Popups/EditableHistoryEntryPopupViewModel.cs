using System;
using VexTrack.Core;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableHistoryEntryPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; }
		public RelayCommand OnDoneClicked { get; }

		public string Title { get; private set; }
		private string Uuid { get; set; }
		private string GroupUuid { get; set; }
		public string ScoreType => GameMode != null ? Constants.ScoreTypes[GameMode] : "";
		private bool EditMode { get; set; }

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

		public EditableHistoryEntryPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(_ => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(_ =>
			{
				if (ScoreType is "Placement" or "None") EnemyScore = -1;
				if (ScoreType == "None") Score = -1;
				if (ScoreType == "Score") Description = "";

				if (EditMode) Tracking.EditHistoryEntry(GroupUuid, Uuid, new HistoryEntry(GroupUuid, Uuid, Time, GameMode, Amount, Map, Description, Score, EnemyScore, SurrenderedWin, SurrenderedLoss));
				else Tracking.AddHistoryEntry(new HistoryEntry(GroupUuid, Uuid, Time, GameMode, Amount, Map, Description, Score, EnemyScore, SurrenderedWin, SurrenderedLoss));
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
			Time = DateTimeOffset.Now.ToUnixTimeSeconds();

			GroupUuid = "";
			Uuid = Guid.NewGuid().ToString();
			Description = "";
			GameMode = Constants.GameModes[0];
			Map = Constants.Maps[0];
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
}