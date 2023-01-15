using System;
using System.Collections.Generic;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableSeasonPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string Title { get; set; }
		public string Uuid { get; set; }
		public bool EditMode { get; set; }

		private string _name;
		private long _endDate;
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
		public long EndDate
		{
			get => _endDate;
			set
			{
				_endDate = value;
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
		public int RemainingDays => TrackingData.GetRemainingDays(Uuid, DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date, true);

		public EditableSeasonPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o =>
			{
				CanCancel = true;
				MainVm.InterruptUpdate = false;
				
				List<HistoryEntry> initList = new()
				{
					new HistoryEntry(Uuid, Guid.NewGuid().ToString(), DateTimeOffset.Now.AddDays(-1).ToLocalTime().ToUnixTimeSeconds(), "Custom", 0, "", "Initialization", -1, -1, false, false)
				};

				if (EditMode) TrackingData.EditSeason(Uuid, new Season(Uuid, Name, EndDate, TrackingData.GetSeason(Uuid).ActiveBpLevel, TrackingData.GetSeason(Uuid).Cxp, TrackingData.GetSeason(Uuid).History));
				else TrackingData.AddSeason(new Season(Uuid, Name, EndDate, 2, 0, initList));

				Close();
			});
		}

		public void SetParameters(string title, bool editMode)
		{
			Title = title;
			EditMode = editMode;

			if (!EditMode) InitData();
		}

		private void InitData()
		{
			EndDate = DateTimeOffset.Now.AddDays(61).ToUnixTimeSeconds();

			Uuid = Guid.NewGuid().ToString();
			Name = "";
			Progress = 0;

			IsInitialized = true;
		}

		public void SetData(Season data)
		{
			EndDate = data.EndDate;

			Uuid = data.Uuid;
			Name = data.Name;
			Progress = CalcUtil.CalcProgress(CalcUtil.CalcMaxForSeason(false), data.Cxp);

			IsInitialized = true;
		}
	}
}
