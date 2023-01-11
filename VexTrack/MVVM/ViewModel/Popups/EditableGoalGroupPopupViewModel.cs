using System;
using System.Collections.Generic;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableGoalGroupPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string Title { get; set; }
		public string Uuid { get; set; }
		public bool EditMode { get; set; }

		private string _name;
		private int _activeTier = 1;
		private int _collected;
		private bool _generateAgentGoals;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public int ActiveTier
		{
			get => _activeTier;
			set
			{
				_activeTier = value;
				OnPropertyChanged();
			}
		}

		public int Collected
		{
			get => _collected;
			set
			{
				_collected = value;

				var maxForTier = CalcUtil.CalcMaxForTier(ActiveTier);
				if (_collected >= maxForTier) _collected = maxForTier - 1;

				OnPropertyChanged();
			}
		}

		public bool GenerateAgentGoals
		{
			get => _generateAgentGoals;
			set
			{
				_generateAgentGoals = value;
				OnPropertyChanged();
			}
		}

		public EditableGoalGroupPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o =>
			{
				if (EditMode) TrackingDataHelper.EditContracts(Uuid, Name);
				else
				{
					TrackingDataHelper.AddContract(new Contract(Uuid, Name, new List<Goal>()));

					if (GenerateAgentGoals)
					{
						var prevUuid = "";

						for (var i = ActiveTier; i < Constants.AgentTiers + 1; i++)
						{
							var uuid = Guid.NewGuid().ToString();
							TrackingDataHelper.AddGoal(Uuid, new Goal(uuid, Name + " Tier " + i.ToString(), CalcUtil.CalcMaxForTier(i), i == ActiveTier ? Collected : 0, "", prevUuid, false));
							prevUuid = uuid;
						}

						ActiveTier = 1;
						Collected = 0;
						GenerateAgentGoals = false;
					}
				}

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
			Uuid = Guid.NewGuid().ToString();
			Name = "";

			IsInitialized = true;
		}

		public void SetData(ContractData data)
		{
			Uuid = data.Uuid;
			Name = data.Name;

			IsInitialized = true;
		}
	}
}
