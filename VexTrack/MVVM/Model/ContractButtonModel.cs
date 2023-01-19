using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VexTrack.Core;

namespace VexTrack.MVVM.Model;

public class ContractButtonModel : Button
{
    private static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ContractButtonModel), new PropertyMetadata(""));
    private static readonly DependencyProperty CollectedProperty = DependencyProperty.Register(nameof(Collected), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    private static readonly DependencyProperty TotalProperty = DependencyProperty.Register(nameof(Total), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    private static readonly DependencyProperty RemainingProperty = DependencyProperty.Register(nameof(Remaining), typeof(int), typeof(ContractButtonModel), new PropertyMetadata(0));
    private static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(ContractButtonModel), new PropertyMetadata(0.0));

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
}