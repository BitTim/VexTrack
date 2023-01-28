using System;
using System.Windows;
using System.Windows.Controls;

namespace VexTrack.MVVM.Model;

public class GoalDisplayModel : Control
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(GoalDisplayModel), new PropertyMetadata(""));
    public static readonly DependencyProperty CollectedProperty = DependencyProperty.Register(nameof(Collected), typeof(int), typeof(GoalDisplayModel), new PropertyMetadata(0));
    public static readonly DependencyProperty TotalProperty = DependencyProperty.Register(nameof(Total), typeof(int), typeof(GoalDisplayModel), new PropertyMetadata(0));
    public static readonly DependencyProperty RemainingProperty = DependencyProperty.Register(nameof(Remaining), typeof(int), typeof(GoalDisplayModel), new PropertyMetadata(0));
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(GoalDisplayModel), new PropertyMetadata(0.0));
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(GoalDisplayModel), new PropertyMetadata(""));
    
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
}