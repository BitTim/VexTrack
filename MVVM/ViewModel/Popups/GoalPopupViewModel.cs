﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class GoalPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnEditClicked { get; set; }
		public RelayCommand OnDeleteClicked { get; set; }
		//private EditableGoalPopupViewModel EditableGoalPopup { get; set; }

		private GoalEntryData RawData { get; set; }
		public int Index { get; set; }
		public string UUID { get; set; }
		public string Title { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }
		public double Progress { get; set; }
		public int Active { get; set; }
		public string Color { get; set; }

		public GoalPopupViewModel()
		{
			//EditableGoalPopup = (EditableGoalPopupViewModel)ViewModelManager.ViewModels["EditableGoalPopup"];
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { Close(); });
			OnEditClicked = new RelayCommand(o => {
				//EditableGoalPopup.SetParameters("Edit History Entry", true);
				//EditableGoalPopup.SetData(RawData);
				//MainVM.QueuePopup(EditableGoalPopup);
			});
			OnDeleteClicked = new RelayCommand(o => {
				IsInitialized = false;
				TrackingDataHelper.RemoveGoal(UUID);
			});
		}

		//public void SetData(HistoryEntryData data)
		//{
		//	RawData = data;

		//	Index = data.Index;
		//	UUID = data.UUID;
		//	Description = data.Description;
		//	Time = data.Time;
		//	Amount = data.Amount;
		//	Map = data.Map;
		//	Result = data.Result;

		//	if (Result == "") Result = "-";
		//	if (Map == "" || Map == null) Map = "-";

		//	IsInitialized = true;
		//}

		//public override void Close()
		//{
		//	EditableHEPopup.Close();
		//	base.Close();
		//}
	}
}
