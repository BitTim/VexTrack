using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VexTrack.Core;

namespace VexTrack.MVVM.Model;

public class ContractButtonModel : Control
{
    private static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ContractButtonModel), new PropertyMetadata(""));
    private static readonly DependencyProperty CollectedProperty = DependencyProperty.Register(nameof(Collected), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    private static readonly DependencyProperty TotalProperty = DependencyProperty.Register(nameof(Total), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    private static readonly DependencyProperty RemainingProperty = DependencyProperty.Register(nameof(Remaining), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    private static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(ContractButtonModel), new PropertyMetadata(0.0));
    private static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(ContractButtonModel), new PropertyMetadata(""));
    private static readonly DependencyProperty GoalsProperty = DependencyProperty.Register(nameof(Goals), typeof(ObservableCollection<Goal>), typeof(ContractButtonModel), new PropertyMetadata(new ObservableCollection<Goal>()));

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
}