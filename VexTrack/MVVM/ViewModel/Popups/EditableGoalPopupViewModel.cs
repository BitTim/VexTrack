using System;
using System.Collections.Generic;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableGoalPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string PopupTitle { get; set; }
		public string Uuid { get; set; }
		public int StartXp { get; set; }
		public bool EditMode { get; set; }
		public bool Paused { get; set; }


		private string PrevColor { get; set; }

		private string _title;
		private int _total;
		private int _collected;
		private string _color;
		private bool _useAccentColor;
		private double _progress;

		private string _group;
		private string _dependency;

		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}
		public int Total
		{
			get => _total;
			set
			{
				_total = value;
				RecalcProgress();
				OnPropertyChanged();
				OnPropertyChanged(nameof(Progress));
			}
		}
		public int Collected
		{
			get => _collected;
			set
			{
				_collected = value;
				RecalcProgress();
				OnPropertyChanged();
				OnPropertyChanged(nameof(Progress));
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
		public string Color
		{
			get => _color;
			set
			{
				PrevColor = _color;
				_color = value;
				OnPropertyChanged();
			}
		}


		public List<Contract> AvailableGroups => TrackingDataHelper.Data.Contracts;
		public List<Goal> AvailableDependencies
		{
			get
			{
				List<Goal> goals;
				if (Group == null) goals = new();

				goals = new(TrackingDataHelper.Data.Contracts[TrackingDataHelper.Data.Contracts.FindIndex(gg => gg.Uuid == Group)].Goals);
				goals.Insert(0, new Goal("", "No Dependency", 0, 0, "#000000", "", true));

				var index = goals.FindIndex(gg => gg.Uuid == Uuid);
				if (index >= 0) goals.RemoveAt(index);
				return goals;
			}
		}
		public string Group
		{
			get => _group;
			set
			{
				_group = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(AvailableDependencies));
			}
		}
		public string Dependency
		{
			get => _dependency;
			set
			{
				_dependency = value;
				OnPropertyChanged();
			}
		}


		public bool UseAccentColor
		{
			get => _useAccentColor;
			set
			{
				_useAccentColor = value;
				if (value) Color = "";
				else if (PrevColor != null && PrevColor != "") Color = PrevColor;
				else Color = "#000000";

				OnPropertyChanged();
				OnPropertyChanged(Color);
			}
		}

		public EditableGoalPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o =>
			{
				if (EditMode) TrackingDataHelper.EditGoal(Group, Uuid, new Goal(Uuid, Title, Total, Collected, Color, Dependency, Paused));
				else TrackingDataHelper.AddGoal(Group, new Goal(Uuid, Title, Total, Collected, Color, Dependency, Paused));
				Close();
			});
		}

		public void SetParameters(string popupTitle, bool editMode)
		{
			PopupTitle = popupTitle;
			EditMode = editMode;

			if (!EditMode) InitData();
		}

		public void InitData()
		{
			Uuid = Guid.NewGuid().ToString();
			Title = "";
			Total = 0;
			Collected = 0;
			UseAccentColor = false;
			Color = "#000000";
			Paused = false;

			if (AvailableGroups.Count == 0)
			{
				TrackingDataHelper.AddContract(new Contract(Constants.DefaultGroupUuid, "No Group", new List<Goal>()));
				Group = Constants.DefaultGroupUuid;
			}
			else Group = AvailableGroups[0].Uuid;

			Dependency = "";
			IsInitialized = true;
		}

		public void SetData(GoalEntryData data)
		{
			Uuid = data.Uuid;
			Title = data.Title;
			Total = data.Total;
			Collected = data.Collected;
			UseAccentColor = data.Color == "" ? true : false;
			Color = data.Color;
			StartXp = data.StartXp;
			Paused = data.Paused;

			if (AvailableGroups.FindIndex(gg => gg.Uuid == data.GroupUuid) == -1) Group = Constants.DefaultGroupUuid;
			else Group = data.GroupUuid;

			if (AvailableDependencies.FindIndex(d => d.Uuid == data.DepUuid) == -1) Dependency = "";
			else Dependency = data.DepUuid;

			IsInitialized = true;
		}

		private void RecalcProgress()
		{
			Progress = CalcUtil.CalcProgress(Total, Collected);
		}
	}
}
