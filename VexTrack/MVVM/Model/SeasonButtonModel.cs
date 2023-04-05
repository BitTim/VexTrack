using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using LiveCharts;
using VexTrack.Core;

namespace VexTrack.MVVM.Model;

public class SeasonButtonModel : ToggleButton
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(SeasonButtonModel), new PropertyMetadata(""));
    public static readonly DependencyProperty CollectedProperty = DependencyProperty.Register(nameof(Collected), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty TotalProperty = DependencyProperty.Register(nameof(Total), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty RemainingProperty = DependencyProperty.Register(nameof(Remaining), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty DailyAverageProperty = DependencyProperty.Register(nameof(DailyAverage), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(SeasonButtonModel), new PropertyMetadata(0.0));
    
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty BufferDaysProperty = DependencyProperty.Register(nameof(BufferDays), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty GraphSeriesCollectionProperty = DependencyProperty.Register(nameof(GraphSeriesCollection), typeof(SeriesCollection), typeof(SeasonButtonModel), new PropertyMetadata(new SeriesCollection()));
    public static readonly DependencyProperty GoalsProperty = DependencyProperty.Register(nameof(Goals), typeof(ObservableCollection<Goal>), typeof(SeasonButtonModel), new PropertyMetadata(new ObservableCollection<Goal>()));
    public static readonly DependencyProperty GoalDisplayHeightProperty = DependencyProperty.Register(nameof(GoalDisplayHeight), typeof(double), typeof(SeasonButtonModel), new PropertyMetadata(72.0));
    public static readonly DependencyProperty EndDateTimestampProperty = DependencyProperty.Register(nameof(EndDateTimestamp), typeof(long), typeof(SeasonButtonModel), new PropertyMetadata((long) 0));
    
    public static readonly DependencyProperty StrongestDayAmountProperty = DependencyProperty.Register(nameof(StrongestDayAmount), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty StrongestDayTimestampProperty = DependencyProperty.Register(nameof(StrongestDayTimestamp), typeof(long), typeof(SeasonButtonModel), new PropertyMetadata((long) 0));
    public static readonly DependencyProperty WeakestDayAmountProperty = DependencyProperty.Register(nameof(WeakestDayAmount), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));
    public static readonly DependencyProperty WeakestDayTimestampProperty = DependencyProperty.Register(nameof(WeakestDayTimestamp), typeof(long), typeof(SeasonButtonModel), new PropertyMetadata((long) 0));
    
    public static readonly DependencyProperty NextUnlockNameProperty = DependencyProperty.Register(nameof(NextUnlockName), typeof(string), typeof(SeasonButtonModel), new PropertyMetadata("None"));
    public static readonly DependencyProperty NextUnlockProgressProperty = DependencyProperty.Register(nameof(NextUnlockProgress), typeof(double), typeof(SeasonButtonModel), new PropertyMetadata(0.0));
    public static readonly DependencyProperty NextUnlockRemainingProperty = DependencyProperty.Register(nameof(NextUnlockRemaining), typeof(int), typeof(SeasonButtonModel), new PropertyMetadata(0));

    public static readonly DependencyProperty StatusIconDataProperty = DependencyProperty.Register(nameof(StatusIconData), typeof(Geometry), typeof(SeasonButtonModel), new PropertyMetadata(null));
    public static readonly DependencyProperty StatusIconColorProperty = DependencyProperty.Register(nameof(StatusIconColor), typeof(Brush), typeof(SeasonButtonModel), new PropertyMetadata(SettingsHelper.Data.Theme.ForegroundBrush));
    
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
    
    public int DailyAverage
    {
        get => (int)GetValue(DailyAverageProperty);
        set => SetValue(DailyAverageProperty, value);
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }



    public int Duration
    {
        get => (int)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }
    
    public int BufferDays
    {
        get => (int)GetValue(BufferDaysProperty);
        set => SetValue(BufferDaysProperty, value);
    }
    
    public SeriesCollection GraphSeriesCollection
    {
        get => (SeriesCollection)GetValue(GraphSeriesCollectionProperty);
        set => SetValue(GraphSeriesCollectionProperty, value);
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

    public long EndDateTimestamp
    {
        get => (long)GetValue(EndDateTimestampProperty);
        set => SetValue(EndDateTimestampProperty, value);
    }
    
    
    
    public int StrongestDayAmount
    {
        get => (int)GetValue(StrongestDayAmountProperty);
        set => SetValue(StrongestDayAmountProperty, value);
    }

    public long StrongestDayTimestamp
    {
        get => (long)GetValue(StrongestDayTimestampProperty);
        set => SetValue(StrongestDayTimestampProperty, value);
    }
    
    public int WeakestDayAmount
    {
        get => (int)GetValue(WeakestDayAmountProperty);
        set => SetValue(WeakestDayAmountProperty, value);
    }

    public long WeakestDayTimestamp
    {
        get => (long)GetValue(WeakestDayTimestampProperty);
        set => SetValue(WeakestDayTimestampProperty, value);
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



    public Geometry StatusIconData
    {
        get => (Geometry)GetValue(StatusIconDataProperty);
        set => SetValue(StatusIconDataProperty, value);
    }

    public Brush StatusIconColor
    {
        get => (Brush)GetValue(StatusIconColorProperty);
        set => SetValue(StatusIconColorProperty, value);
    }
    
    

    public List<decimal> LogicalSegmentsStops => CalcUtil.CalcLogicalStops(CalcUtil.CalcSeasonSegments(), true);
    public List<decimal> VisualSegmentsStops => CalcUtil.CalcVisualStops(CalcUtil.CalcSeasonSegments(), true);
    public int NumGoals => Goals.Count;
    public int BufferDaysPosition => Duration - BufferDays;
}