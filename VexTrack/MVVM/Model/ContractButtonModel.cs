using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using VexTrack.Core;

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

    public static readonly DependencyProperty NextUnlockNameProperty = DependencyProperty.Register(nameof(NextUnlockName), typeof(string), typeof(ContractButtonModel), new PropertyMetadata("None"));
    public static readonly DependencyProperty NextUnlockProgressProperty = DependencyProperty.Register(nameof(NextUnlockProgress), typeof(double), typeof(ContractButtonModel), new PropertyMetadata(0.0));
    public static readonly DependencyProperty NextUnlockRemainingProperty = DependencyProperty.Register(nameof(NextUnlockRemaining), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    
    //TODO: My suspicion is that the SegmentStops don't get passed to SegmentedProgressModel because of incorrect binding. The line below was an attempt at trying to fix, but might lead to nothing.
    public static readonly DependencyProperty SegmentStopsProperty = DependencyProperty.Register(nameof(SegmentsStops), typeof(List<decimal>), typeof(ContractButtonModel), new PropertyMetadata(new List<decimal>()));
    
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

    public List<decimal> SegmentsStops => CalcUtil.CalcStops(Goals.Select(goal => goal.Total).ToList(), false);

    public int NumGoals => Goals.Count;
}