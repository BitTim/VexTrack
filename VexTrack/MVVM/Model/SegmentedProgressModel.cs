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

        private static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(Spacing), typeof(int), typeof(SegmentedProgressModel), new PropertyMetadata(2));
        private static readonly DependencyProperty AvailableWidthProperty = DependencyProperty.Register(nameof(AvailableWidth), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(0.0));
        
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(SegmentedProgressModel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged));
        private static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(0.0, OnPropertyChanged));
        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(100.0, OnPropertyChanged));
        private static readonly DependencyProperty SegmentsStopsProperty = DependencyProperty.Register(nameof(SegmentsStops), typeof(List<decimal>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<decimal> {1}, OnPropertyChanged));
        private static readonly DependencyProperty SegmentedDataProperty = DependencyProperty.Register(nameof(SegmentedData), typeof(List<SegmentData>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<SegmentData>() {new(0, 0)}, OnPropertyChanged));
        private static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("", OnPropertyChanged));

        private static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(nameof(Percentage), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("0 %"));
        private static readonly DependencyProperty ShowPercentageProperty = DependencyProperty.Register(nameof(ShowPercentage), typeof(bool), typeof(SegmentedProgressModel), new PropertyMetadata(true));
        private static readonly DependencyProperty PercentageWidthProperty = DependencyProperty.Register(nameof(PercentageWidth), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(48.0));
        
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

        public double AvailableWidth
        {
            get => (double)GetValue(AvailableWidthProperty);
            set => SetValue(AvailableWidthProperty, value);
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

        public List<decimal> SegmentsStops
        {
            get => (List<decimal>)GetValue(SegmentsStopsProperty);
            set => SetValue(SegmentsStopsProperty, value);
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

        public double PercentageWidth
        {
            get => (double)GetValue(PercentageWidthProperty);
            set => SetValue(PercentageWidthProperty, value);
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
            Percentage = (int)(valuePercent * 100) + " %";

            if (string.IsNullOrEmpty(Color)) ForegroundBrush = (Brush)Application.Current.FindResource("Accent");
            else
            {
                Brush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(Color);
                ForegroundBrush = brush;
            }
            
            var availableWidth = AvailableWidth == 0 ? (double)Parent.GetValue(ActualWidthProperty) - Margin.Left - Margin.Right - PercentageWidth : AvailableWidth;
            SegmentedData = CalcSegmentedData(SegmentsStops, (decimal)valuePercent, availableWidth);

            _isUpdating = false;
        }

        private static List<SegmentData> CalcSegmentedData(IReadOnlyList<decimal> stops, decimal val, double availableWidth)
        {
            if (stops.Count == 0) return new List<SegmentData>() { new(1, availableWidth) };
            
            var segmentedData = new List<SegmentData>();
            decimal prevStop = 0;

            foreach (var stop in stops)
            {
                var stopDiff = stop - prevStop;
                var segmentWidth = availableWidth * decimal.ToDouble(stopDiff); 
                prevStop = stop;

                if (val > 0)
                {
                    if (stopDiff > val)
                    {
                        segmentedData.Add(new SegmentData(val / stop, segmentWidth));
                        break;
                    }
                
                    segmentedData.Add(new SegmentData(1, segmentWidth));
                    val -= stopDiff;
                    continue;
                }
                
                segmentedData.Add(new SegmentData(0, segmentWidth));
            }
            
            return segmentedData;
        }
    }

    public class SegmentData
    {
        public bool Visible { get; }
        public GridLength Length { get; }
        public GridLength Space { get; }
        public double SegmentWidth { get;  }

        public SegmentData(decimal value, double segmentWidth)
        {
            Visible = value > 0;
            Length = new GridLength(decimal.ToDouble(value * 100), GridUnitType.Star);
            Space = new GridLength(decimal.ToDouble(100 - value * 100), GridUnitType.Star);
            SegmentWidth = segmentWidth;
        }
    }
}