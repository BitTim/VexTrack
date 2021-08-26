using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableGoalPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string PopupTitle { get; set; }
		public string UUID { get; set; }
		public int StartXP { get; set; }
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


		public List<GoalGroup> AvailableGroups => TrackingDataHelper.Data.Goals;
		public List<Goal> AvailableDependencies => Group != null ? TrackingDataHelper.Data.Goals[TrackingDataHelper.Data.Goals.FindIndex(gg => gg.UUID == Group)].Goals : new();
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
			OnDoneClicked = new RelayCommand(o => {
				if (EditMode) TrackingDataHelper.EditGoal(Group, UUID, new Goal(UUID, Title, Total, Collected, Color, Paused));
				else TrackingDataHelper.AddGoal(Group, new Goal(UUID, Title, Total, Collected, Color, Paused));
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
			UUID = Guid.NewGuid().ToString();
			Title = "";
			Total = 0;
			Collected = 0;
			UseAccentColor = false;
			Color = "#000000";
			Paused = false;

			//TODO: Handle case where no Group exists
			//      -> Ability to Create Groups
			Group = AvailableGroups[0].UUID;

			IsInitialized = true;
		}

		public void SetData(GoalEntryData data)
		{
			UUID = data.UUID;
			Group = data.GroupUUID;
			Title = data.Title;
			Total = data.Total;
			Collected = data.Collected;
			UseAccentColor = data.Color == "" ? true : false;
			Color = data.Color;
			StartXP = data.StartXP;
			Paused = data.Paused;

			IsInitialized = true;
		}

		private void RecalcProgress()
		{
			Progress = CalcUtil.CalcProgress(Total, Collected);
		}
	}
}
