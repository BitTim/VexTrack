using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;

namespace VexTrack.MVVM.Model;

public class ContractButtonModel : ToggleButton
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ContractButtonModel), new PropertyMetadata(""));
    public static readonly DependencyProperty CollectedProperty = DependencyProperty.Register(nameof(Collected), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty TotalProperty = DependencyProperty.Register(nameof(Total), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty RemainingProperty = DependencyProperty.Register(nameof(Remaining), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(ContractButtonModel), new PropertyMetadata(0.0));
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(ContractButtonModel), new PropertyMetadata(""));
    public static readonly DependencyProperty GoalsProperty = DependencyProperty.Register(nameof(Goals), typeof(ObservableCollection<Goal>), typeof(ContractButtonModel), new PropertyMetadata(new ObservableCollection<Goal>()));
    public static readonly DependencyProperty GoalDisplayHeightProperty = DependencyProperty.Register(nameof(GoalDisplayHeight), typeof(double), typeof(ContractButtonModel), new PropertyMetadata(72.0));
    public static readonly DependencyProperty CompletionForecastDaysProperty = DependencyProperty.Register(nameof(CompletionForecastDays), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty CompletionTimestampProperty = DependencyProperty.Register(nameof(CompletionTimestamp), typeof(long), typeof(ContractButtonModel), new PropertyMetadata((long) 0));
    
    public static readonly DependencyProperty NextUnlockNameProperty = DependencyProperty.Register(nameof(NextUnlockName), typeof(string), typeof(ContractButtonModel), new PropertyMetadata("None"));
    public static readonly DependencyProperty NextUnlockProgressProperty = DependencyProperty.Register(nameof(NextUnlockProgress), typeof(double), typeof(ContractButtonModel), new PropertyMetadata(0.0));
    public static readonly DependencyProperty NextUnlockRemainingProperty = DependencyProperty.Register(nameof(NextUnlockRemaining), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public int Collected
    {
        get => (int)GetValue(CollectedProperty);
        set => SetValue(CollectedProperty, value);
    }
    
    public int Total
    {
        get => (int)GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }
    
    public int Remaining
    {
        get => (int)GetValue(RemainingProperty);
        set => SetValue(RemainingProperty, value);
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public string Color
    {
        get => (string)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    
    public ObservableCollection<Goal> Goals
    {
        get => (ObservableCollection<Goal>)GetValue(GoalsProperty);
        set => SetValue(GoalsProperty, value);
    }

    public double GoalDisplayHeight
    {
        get => (double)GetValue(GoalDisplayHeightProperty);
        set => SetValue(GoalDisplayHeightProperty, value);
    }
    
    public int CompletionForecastDays
    {
        get => (int)GetValue(CompletionForecastDaysProperty);
        set => SetValue(CompletionForecastDaysProperty, value);
    }

    public long CompletionTimestamp
    {
        get => (long)GetValue(CompletionTimestampProperty);
        set => SetValue(CompletionTimestampProperty, value);
    }



    public string NextUnlockName
    {
        get => (string)GetValue(NextUnlockNameProperty);
        set => SetValue(NextUnlockNameProperty, value);
    }

    public double NextUnlockProgress
    {
        get => (double)GetValue(NextUnlockProgressProperty);
        set => SetValue(NextUnlockProgressProperty, value);
    }

    public int NextUnlockRemaining
    {
        get => (int)GetValue(NextUnlockRemainingProperty);
        set => SetValue(NextUnlockRemainingProperty, value);
    }

    public List<decimal> LogicalSegmentsStops => CalcHelper.CalcLogicalStops(Goals.Select(goal => goal.Total).ToList(), true);
    public List<decimal> VisualSegmentsStops => CalcHelper.CalcVisualStops(Goals.Select(goal => goal.Total).ToList(), true);
    public int NumGoals => Goals.Count;
}