using System;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class EditableGoalPopupViewModel : BasePopupViewModel
{
	public RelayCommand OnBackClicked { get; }
	public RelayCommand OnDoneClicked { get; }

	public string PopupTitle { get; set; }
	private string Uuid { get; set; }
	private bool EditMode { get; set; }
	private bool Paused { get; set; }


	private string PrevColor { get; set; }

	private string _name;
	private int _total;
	private int _collected;
	private string _color;
	private bool _useAccentColor;

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

	public double Progress => CalcHelper.CalcProgress(Total, Collected);
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

		OnBackClicked = new RelayCommand(_ => { if (CanCancel) Close(); });
		OnDoneClicked = new RelayCommand(_ =>
		{
			//var goals = UserData.Contracts[UserData.Contracts.FindIndex(contract => contract.Uuid == Uuid)].Goals;
				
			// TODO: Make only Contract instance editable, not the template
			//if (EditMode) UserData.EditContract(Uuid, new Contract(Uuid, Name, Color, Paused, goals));
			//else UserData.AddContract(new Contract(Uuid, Name, Color, Paused, new List<Goal>()));
			Close();
		});
	}
		
		
		
	// TODO: These three are currently unused, but will be used again at a later point in time
		
	public void SetParameters(string popupTitle, bool editMode)
	{
		PopupTitle = popupTitle;
		EditMode = editMode;

		if (!EditMode) InitData();
	}

	private void InitData()
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
}