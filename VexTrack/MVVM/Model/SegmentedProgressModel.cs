using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VexTrack.Core.Helper;

namespace VexTrack.MVVM.Model
{
    public class SegmentedProgressModel : ItemsControl
    {
        public static readonly DependencyProperty BackgroundThicknessProperty = DependencyProperty.Register(nameof(BackgroundThickness), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(8.0));
        public static readonly DependencyProperty ForegroundThicknessProperty = DependencyProperty.Register(nameof(ForegroundThickness), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(8.0));
        public static readonly DependencyProperty ForegroundBrushProperty = DependencyProperty.Register(nameof(ForegroundBrush), typeof(Brush), typeof(SegmentedProgressModel), new PropertyMetadata(SettingsHelper.Data.Theme.AccentBrush));

        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(Spacing), typeof(int), typeof(SegmentedProgressModel), new PropertyMetadata(2));
        
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(SegmentedProgressModel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(0.0, OnPropertyChanged));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(100.0, OnPropertyChanged));
        public static readonly DependencyProperty LogicalSegmentsStopsProperty = DependencyProperty.Register(nameof(LogicalSegmentsStops), typeof(List<decimal>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<decimal> {1}, OnPropertyChanged));
        public static readonly DependencyProperty VisualSegmentsStopsProperty = DependencyProperty.Register(nameof(VisualSegmentsStops), typeof(List<decimal>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<decimal> {1}, OnPropertyChanged));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("", OnPropertyChanged));

        public static readonly DependencyProperty SegmentedDataProperty = DependencyProperty.Register(nameof(SegmentedData), typeof(List<SegmentData>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<SegmentData>() {new(0, 1.0)}));
        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(nameof(Percentage), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("0 %"));
        public static readonly DependencyProperty ShowPercentageProperty = DependencyProperty.Register(nameof(ShowPercentage), typeof(bool), typeof(SegmentedProgressModel), new PropertyMetadata(true));
        public static readonly DependencyProperty NumSegmentsProperty = DependencyProperty.Register(nameof(NumSegments), typeof(int), typeof(SegmentedProgressModel), new PropertyMetadata(1));
        public static readonly DependencyProperty DebugNameProperty = DependencyProperty.Register(nameof(DebugName), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("None", OnPropertyChanged));
        
        public double BackgroundThickness
        {
            get => (double)GetValue(BackgroundThicknessProperty);
            set => SetValue(BackgroundThicknessProperty, value);
        }

        public double ForegroundThickness
        {
            get => (double)GetValue(ForegroundThicknessProperty);
            set => SetValue(ForegroundThicknessProperty, value);
        }
        
        public Brush ForegroundBrush
        {
            get => (Brush)GetValue(ForegroundBrushProperty);
            set => SetValue(ForegroundBrushProperty, value);
        }



        public int Spacing
        {
            get => (int)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }



        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public List<decimal> LogicalSegmentsStops
        {
            get => (List<decimal>)GetValue(LogicalSegmentsStopsProperty);
            set => SetValue(LogicalSegmentsStopsProperty, value);
        }

        public List<decimal> VisualSegmentsStops
        {
            get => (List<decimal>)GetValue(VisualSegmentsStopsProperty);
            set => SetValue(VisualSegmentsStopsProperty, value);
        }

        public List<SegmentData> SegmentedData
        {
            get => (List<SegmentData>)GetValue(SegmentedDataProperty);
            set => SetValue(SegmentedDataProperty, value);
        }
        
        public string Color
        {
            get => (string)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }



        public string Percentage
        {
            get => (string)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }

        public bool ShowPercentage
        {
            get => (bool)GetValue(ShowPercentageProperty);
            set => SetValue(ShowPercentageProperty, value);
        }

        public string DebugName
        {
            get => (string)GetValue(DebugNameProperty);
            set => SetValue(DebugNameProperty, value);
        }

        public int NumSegments
        {
            get => (int)GetValue(NumSegmentsProperty);
            set => SetValue(NumSegmentsProperty, value);
        }
        
        private ItemsPresenter _presenterReference;



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _presenterReference = (ItemsPresenter)Template.FindName("segmentsPresenter", this);
            
            var descriptor = DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(ItemsPresenter));
            descriptor?.AddValueChanged(this, ActualWidth_ValueChanged);
        }

        private void ActualWidth_ValueChanged(object aSender, EventArgs aE)
        {
            Update();
        }
        
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentedProgressModel model)
            {
                model.Update();
            }
        }
        
        private void Update()
        {
            if (Value > MaxValue) Value = MaxValue;
            if (Value < MinValue) Value = MinValue;

            var valuePercent = Math.Round((Value - MinValue) / MaxValue - MinValue, 2);
            Percentage = (int)(valuePercent * 100) + " %";

            if (string.IsNullOrEmpty(Color)) ForegroundBrush = SettingsHelper.Data.Theme.AccentBrush;
            else
            {
                Brush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(Color);
                ForegroundBrush = brush;
            }

            if (_presenterReference == null) return;
            SegmentedData = CalcSegmentedData(LogicalSegmentsStops, VisualSegmentsStops, (decimal) valuePercent, (double)GetValue(ActualWidthProperty) - 48, Spacing); //48 Because that is the set width for the percentage text
            NumSegments = SegmentedData.Count;
        }

        private static List<SegmentData> CalcSegmentedData(IReadOnlyList<decimal> logicalStops, IReadOnlyList<decimal> visualStops, decimal val, double actualWidth, int spacing)
        {
            if (logicalStops.Count == 0 || visualStops.Count == 0 || logicalStops.Count != visualStops.Count) return new List<SegmentData> { new(1, actualWidth - spacing) };
            
            var segmentedData = new List<SegmentData>();
            decimal prevLogicalStop = 0;
            decimal prevVisualStop = 0;
            var i = 0;
            
            while (val > (decimal)0.001)
            {
                var logicalStopDiff = logicalStops[i] - prevLogicalStop;
                var visualStopDiff = visualStops[i] - prevVisualStop;
                prevLogicalStop = logicalStops[i];
                prevVisualStop = visualStops[i];
                
                var length = (double)Math.Ceiling(visualStopDiff * (decimal)actualWidth - spacing);
                
                if (logicalStopDiff > val)
                {
                    segmentedData.Add(new SegmentData(val / logicalStopDiff, length));
                    i++;
                    break;
                }
                
                segmentedData.Add(new SegmentData(1, length));
                i++;
                val -= logicalStopDiff;
            }

            while (segmentedData.Count < logicalStops.Count)
            {
                var visualStopDiff = visualStops[i] - prevVisualStop;
                prevVisualStop = visualStops[i++];
                
                var length = (double)Math.Ceiling(visualStopDiff * (decimal)actualWidth - spacing);
                segmentedData.Add(new SegmentData(0, length));
            }
            return segmentedData;
        }
    }

    public class SegmentData
    {
        public bool Visible { get; }
        public double Length { get; }
        public GridLength Filled { get; }
        public GridLength Space { get; }

        public SegmentData(decimal value, double length)
        {
            Visible = value > 0;
            Length = length;
            Filled = new GridLength(decimal.ToDouble(value * 100), GridUnitType.Star);
            Space = new GridLength(decimal.ToDouble(100 - value * 100), GridUnitType.Star);
        }
    }
}