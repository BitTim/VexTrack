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
		public double Progress => CalcUtil.CalcProgress(Total, Collected);
		public bool EditMode { get; set; }

		private string _title;
		private int _total;
		private int _collected;
		private string _color;

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
				OnPropertyChanged();
				OnPropertyChanged(nameof(Progress));
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

		public EditableGoalPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o => {
				if (EditMode) TrackingDataHelper.EditGoal(UUID, new Goal(UUID, Title, Total, Collected, Color));
				else TrackingDataHelper.AddGoal(new Goal(UUID, Title, Total, Collected, Color));
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
			Color = "#000000";

			IsInitialized = true;
		}

		public void SetData(GoalEntryData data)
		{
			UUID = data.UUID;
			Title = data.Title;
			Total = data.Total;
			Collected = data.Collected;
			Color = data.Color;
			StartXP = data.StartXP;

			IsInitialized = true;
		}
	}
}
