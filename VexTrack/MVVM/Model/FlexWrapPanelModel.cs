using System;
using System.Windows;
using System.Windows.Controls;

namespace VexTrack.MVVM.Model;

public class FlexWrapPanelModel : WrapPanel
{
    public static readonly DependencyProperty MinItemWidthProperty = DependencyProperty.Register(nameof(MinItemWidth), typeof(double), typeof(FlexWrapPanelModel), new PropertyMetadata(0.0));
    public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(nameof(ItemMargin), typeof(Thickness), typeof(FlexWrapPanelModel), new PropertyMetadata(new Thickness(0)));
    
    public double MinItemWidth
    {
        get => (double)GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    public Thickness ItemMargin
    {
        get => (Thickness)GetValue(ItemMarginProperty);
        set => SetValue(ItemMarginProperty, value);
    }
    
    protected override Size MeasureOverride(Size constraint)
    {
        var panelWidth = constraint.Width;

        foreach (UIElement child in InternalChildren)
        {
            var minWidth = MinItemWidth > 0 ? MinItemWidth : (double)child.GetValue(WidthProperty);
            minWidth += ItemMargin.Left + ItemMargin.Right;
            
            var appliedWidth = panelWidth / Math.Floor(panelWidth / minWidth);
            child.SetValue(WidthProperty, appliedWidth);
        }
        
        return base.MeasureOverride(constraint);
    }
}