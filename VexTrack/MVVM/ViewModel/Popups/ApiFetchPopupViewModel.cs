using System.Collections.Generic;
using System.Linq;
using VexTrack.Core.Helper;

namespace VexTrack.MVVM.ViewModel.Popups;

class ApiFetchPopupViewModel : BasePopupViewModel
{
    private string _currentFetchVersion;
    private string _currentFetchCategory;
    private string _currentStepItemLabel;
    private int _totalFetchSteps;
    private int _currentFetchStep;
    private int _totalStepItems;
    private int _currentStepItem;
    
    private double _fetchStepProgress;
    private double _fetchStepItemProgress;

    public string CurrentFetchVersion
    {
        get => _currentFetchVersion;
        set
        {
            _currentFetchVersion = value;
            OnPropertyChanged();
        }
    }

    public string CurrentFetchCategory
    {
        get => _currentFetchCategory;
        set
        {
            _currentFetchCategory = value;
            OnPropertyChanged();
        }
    }

    public string CurrentStepItemLabel
    {
        get => _currentStepItemLabel;
        set
        {
            _currentStepItemLabel = value;
            OnPropertyChanged();
        }
    }

    public int TotalFetchSteps
    {
        get => _totalFetchSteps;
        set
        {
            _totalFetchSteps = value;
            OnPropertyChanged();
            
            FetchStepProgress = CalcHelper.CalcProgress(TotalFetchSteps, CurrentFetchStep);
        }
    }

    public int CurrentFetchStep
    {
        get => _currentFetchStep;
        set
        {
            _currentFetchStep = value;
            OnPropertyChanged();
            
            FetchStepProgress = CalcHelper.CalcProgress(TotalFetchSteps, CurrentFetchStep);
        }
    }

    public int TotalStepItems
    {
        get => _totalStepItems;
        set
        {
            _totalStepItems = value;
            OnPropertyChanged();
            
            FetchStepItemProgress = CalcHelper.CalcProgress(TotalStepItems, CurrentStepItem);
        }
    }

    public int CurrentStepItem
    {
        get => _currentStepItem;
        set
        {
            _currentStepItem = value;
            OnPropertyChanged();
            
            FetchStepItemProgress = CalcHelper.CalcProgress(TotalStepItems, CurrentStepItem);
        }
    }

    public double FetchStepProgress
    {
        get => _fetchStepProgress;
        set
        {
            _fetchStepProgress = value;
            OnPropertyChanged();
        }
    }

    public double FetchStepItemProgress
    {
        get => _fetchStepItemProgress;
        set
        {
            _fetchStepItemProgress = value;
            OnPropertyChanged();
        }
    }

    
    
    
    public void InitData()
    {
        CurrentFetchVersion = "";
        CurrentFetchCategory = "";
        CurrentStepItemLabel = "";
        TotalFetchSteps = 0;
        CurrentFetchStep = 0;
        TotalStepItems = 0;
        CurrentStepItem = 0;

        IsInitialized = true;
    }
}