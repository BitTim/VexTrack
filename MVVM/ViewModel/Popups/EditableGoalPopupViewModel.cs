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

		private string _title;
		private int _remaining;
		private double _progress;
		private string _color;
		private bool _resetStart;

		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}
		public int Remaining
		{
			get => _remaining;
			set
			{
				_remaining = value;
				OnPropertyChanged();
			}
		}
		public string Color
		{
			get => _color;
			set
			{
				_color = value;
				OnPropertyChanged();
			}
		}
		public bool ResetStart
		{
			get => _resetStart;
			set
			{
				_resetStart = value;
				OnPropertyChanged();
			}
		}

		public EditableGoalPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o => {
				int totalCollected = CalcUtil.CalcTotalCollected(TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP);
				if (ResetStart) StartXP = totalCollected;
					
				if (EditMode) TrackingDataHelper.EditGoal(UUID, new Goal(UUID, Title, Remaining, StartXP, Color));
				else TrackingDataHelper.AddGoal(new Goal(UUID, Title, Remaining, totalCollected, Color));
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
			Remaining = 0;
			Color = "#000000";
			ResetStart = false;

			IsInitialized = true;
		}

		public void SetData(GoalEntryData data)
		{
			//TODO: Implement Progress for ProgressArc

			UUID = data.UUID;
			Title = data.Title;
			Remaining = data.Remaining;
			Color = data.Color;
			ResetStart = false;
			StartXP = data.StartXP;

			IsInitialized = true;
		}
	}
}
