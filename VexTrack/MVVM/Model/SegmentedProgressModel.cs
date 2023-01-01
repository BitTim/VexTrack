using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VexTrack.MVVM.Model
{
    public class SegmentedProgressModel : ItemsControl
    {
        private static readonly DependencyProperty BackgroundThicknessProperty = DependencyProperty.Register(nameof(BackgroundThickness), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(8.0));
        private static readonly DependencyProperty ForegroundThicknessProperty = DependencyProperty.Register(nameof(ForegroundThickness), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(8.0));
        private static readonly DependencyProperty ForegroundBrushProperty = DependencyProperty.Register(nameof(ForegroundBrush), typeof(Brush), typeof(SegmentedProgressModel), new PropertyMetadata(Application.Current.FindResource("Accent")));

        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(SegmentedProgressModel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged));
        private static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(0.0, OnPropertyChanged));
        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(100.0, OnPropertyChanged));
        private static readonly DependencyProperty SegmentsStopsProperty = DependencyProperty.Register(nameof(SegmentsStops), typeof(List<double>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<double> {1.0}, OnPropertyChanged));
        private static readonly DependencyProperty SegmentedValueProperty = DependencyProperty.Register(nameof(SegmentedValue), typeof(List<double>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<double>() {0.0}, OnPropertyChanged));
        private static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("", OnPropertyChanged));

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

        public List<double> SegmentsStops
        {
            get => (List<double>)GetValue(SegmentsStopsProperty);
            set => SetValue(SegmentsStopsProperty, value);
        }

        public List<double> SegmentedValue
        {
            get => (List<double>)GetValue(SegmentedValueProperty);
            set => SetValue(SegmentedValueProperty, value);
        }
        
        public string Color
        {
            get => (string)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        
        
        
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentedProgressModel model)
            {
                model.Update();
            }
        }
        
        private static bool _isUpdating;

        private void Update()
        {
            if (_isUpdating) return;
            _isUpdating = true;

            if (Value > MaxValue) Value = MaxValue;
            if (Value < MinValue) Value = MinValue;

            var valuePercent = Math.Round((Value - MinValue) / MaxValue - MinValue, 2);

            if (string.IsNullOrEmpty(Color)) ForegroundBrush = (Brush)Application.Current.FindResource("Accent");
            else
            {
                Brush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(Color);
                ForegroundBrush = brush;
            }
            
            SegmentedValue = CalcSegmentedData(SegmentsStops, valuePercent);

            var segmentsGrid = (Grid) typeof(ItemsControl).InvokeMember("ItemsHost",
                BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance, null, this, null);
            
            for (var i = 0; i < SegmentsStops.Count; i++)
            {
                var colDef = new ColumnDefinition() { Width = new GridLength(SegmentsStops[i] * 100, GridUnitType.Star) };
                segmentsGrid.ColumnDefinitions.Add(colDef);
                Grid.SetColumn(segmentsGrid.Children[i], i);
            }


            _isUpdating = false;
        }

        private static List<double> CalcSegmentedData(IReadOnlyList<double> stops, double val)
        {
            var segmentedValue = new List<double>();
            var i = 0;
            
            while (val > 0)
            {
                if (stops[i] > val)
                {
                    segmentedValue.Add(val);
                    break;
                }
                
                segmentedValue.Add(1.0);
                val -= stops[i];
                i++;
            }
            
            while (segmentedValue.Count < stops.Count) segmentedValue.Add(0);
            return segmentedValue;
        }
    }
}