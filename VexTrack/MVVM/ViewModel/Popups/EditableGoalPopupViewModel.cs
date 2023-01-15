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
		public bool EditMode { get; set; }
		public bool Paused { get; set; }


		private string PrevColor { get; set; }

		private string _name;
		private int _total;
		private int _collected;
		private string _color;
		private bool _useAccentColor;
		private double _progress;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
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

		public bool UseAccentColor
		{
			get => _useAccentColor;
			set
			{
				_useAccentColor = value;
				if (value) Color = "";
				else if (!string.IsNullOrEmpty(PrevColor)) Color = PrevColor;
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
				var goals = TrackingData.Contracts[TrackingData.Contracts.FindIndex(contract => contract.Uuid == Uuid)].Goals;
				
				// TODO: When creating new Contract, a goal should be initialized as well
				// TODO: This might require separating "ContractInitPopup" from "ContractEditPopup"
				
				if (EditMode) TrackingData.EditContract(Uuid, new Contract(Uuid, Name, Color, Paused, goals));
				else TrackingData.AddContract(new Contract(Uuid, Name, Color, Paused, new List<Goal>()));
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
			Name = "";
			Total = 0;
			Collected = 0;
			UseAccentColor = false;
			Color = "#000000";
			Paused = false;
			
			IsInitialized = true;
		}

		public void SetData(Goal data)
		{
			Uuid = data.Uuid;
			Name = data.Name;
			Total = data.Total;
			Collected = data.Collected;

			IsInitialized = true;
		}

		private void RecalcProgress()
		{
			Progress = CalcUtil.CalcProgress(Total, Collected);
		}
	}
}
