using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableGoalGroupPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string Title { get; set; }
		public string UUID { get; set; }
		public bool EditMode { get; set; }

		private string _name;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public EditableGoalGroupPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o => {
				if (EditMode) TrackingDataHelper.EditGoalGroup(UUID, Name);
				else TrackingDataHelper.AddGoalGroup(new GoalGroup(UUID, Name, new List<Goal>()));

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
			UUID = Guid.NewGuid().ToString();
			Name = "";

			IsInitialized = true;
		}

		public void SetData(GoalGroupData data)
		{
			UUID = data.UUID;
			Name = data.Name;

			IsInitialized = true;
		}
	}
}
