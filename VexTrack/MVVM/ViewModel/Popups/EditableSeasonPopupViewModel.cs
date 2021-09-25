using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableSeasonPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string Title { get; set; }
		public string UUID { get; set; }
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
		public int RemainingDays => TrackingDataHelper.GetRemainingDays(UUID, DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date, true);

		public EditableSeasonPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o => {
				CanCancel = true;
				MainVM.InterruptUpdate = false;

				string endDate = DateTimeOffset.FromUnixTimeSeconds(EndDate).ToLocalTime().Date.ToString("d");
				List<HistoryEntry> initList = new();
				initList.Add(new HistoryEntry(Guid.NewGuid().ToString(), DateTimeOffset.Now.AddDays(-1).ToLocalTime().ToUnixTimeSeconds(), "Custom", 0, "", "Initialization", -1, -1, false, false));

				if (EditMode) TrackingDataHelper.EditSeason(UUID, new Season(UUID, Name, endDate, TrackingDataHelper.GetSeason(UUID).ActiveBPLevel, TrackingDataHelper.GetSeason(UUID).CXP, TrackingDataHelper.GetSeason(UUID).History));
				else TrackingDataHelper.AddSeason(new Season(UUID, Name, endDate, 2, 0, initList));

				Close();
			});
		}

		public void SetParameters(string title, bool editMode)
		{
			Title = title;
			EditMode = editMode;

			if (!EditMode) InitData();
		}

		public void InitData()
		{
			EndDate = DateTimeOffset.Now.AddDays(61).ToUnixTimeSeconds();

			UUID = Guid.NewGuid().ToString();
			Name = "";
			Progress = 0;

			IsInitialized = true;
		}

		public void SetData(SeasonEntryData data)
		{
			EndDate = data.EndDate;

			UUID = data.UUID;
			Name = data.Title;
			Progress = data.Progress;

			IsInitialized = true;
		}
	}
}
