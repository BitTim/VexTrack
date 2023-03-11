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
        
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(SegmentedProgressModel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged));
        private static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(0.0, OnPropertyChanged));
        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(SegmentedProgressModel), new PropertyMetadata(100.0, OnPropertyChanged));
        private static readonly DependencyProperty SegmentsStopsProperty = DependencyProperty.Register(nameof(SegmentsStops), typeof(List<decimal>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<decimal> {1}, OnPropertyChanged));
        private static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("", OnPropertyChanged));

        private static readonly DependencyProperty SegmentedDataProperty = DependencyProperty.Register(nameof(SegmentedData), typeof(List<SegmentData>), typeof(SegmentedProgressModel), new PropertyMetadata(new List<SegmentData>() {new(0)}));
        private static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(nameof(Percentage), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("0 %"));
        private static readonly DependencyProperty ShowPercentageProperty = DependencyProperty.Register(nameof(ShowPercentage), typeof(bool), typeof(SegmentedProgressModel), new PropertyMetadata(true));
        private static readonly DependencyProperty DebugNameProperty = DependencyProperty.Register(nameof(DebugName), typeof(string), typeof(SegmentedProgressModel), new PropertyMetadata("None"));
        
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

        public string DebugName
        {
            get => (string)GetValue(DebugNameProperty);
            set => SetValue(DebugNameProperty, value);
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

            if (string.IsNullOrEmpty(Color)) ForegroundBrush = (Brush)Application.Current.FindResource("Accent");
            else
            {
                Brush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(Color);
                ForegroundBrush = brush;
            }
            
            SegmentedData = CalcSegmentedData(SegmentsStops, (decimal) valuePercent);

            decimal prevStep = 0;
            
            //var segmentsGrid = (Grid) typeof(ItemsControl).InvokeMember("ItemsHost",
            //    BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance, null, this, null);
            var presenterReference = (ItemsPresenter)GetTemplateChild("segmentsPresenter");
            if (presenterReference == null) return;
            
            var segmentsGrid = (Grid)VisualTreeHelper.GetChild(presenterReference, 0);
            //if (segmentsGrid == null) return;
            
            segmentsGrid?.ColumnDefinitions.Clear();
            
            for (var i = 0; i < SegmentsStops.Count; i++)
            {
                var colDef = new ColumnDefinition() { Width = new GridLength(decimal.ToDouble(SegmentsStops[i] - (decimal) prevStep) * 100, GridUnitType.Star) };
                prevStep = SegmentsStops[i];

                segmentsGrid.ColumnDefinitions.Add(colDef);
                Grid.SetColumn(segmentsGrid.Children[i], i);
            }
        }

        private static List<SegmentData> CalcSegmentedData(IReadOnlyList<decimal> stops, decimal val)
        {
            if (stops.Count == 0) return new List<SegmentData>() { new(1) };
            
            var segmentedData = new List<SegmentData>();
            decimal prevStop = 0;
            var i = 0;

            while (val > 0)
            {
                var stopDiff = stops[i] - prevStop;
                if (stopDiff > val)
                {
                    segmentedData.Add(new SegmentData(val / stops[i]));
                    break;
                }
                
                segmentedData.Add(new SegmentData(1));
                val -= stopDiff;
                prevStop = stops[i];
                i++;
            }
            
            while (segmentedData.Count < stops.Count) segmentedData.Add(new SegmentData(0));
            return segmentedData;
        }
    }

    public class SegmentData
    {
        public bool Visible { get; }
        public GridLength Length { get; }
        public GridLength Space { get; }

        public SegmentData(decimal value)
        {
            Visible = value > 0;
            Length = new GridLength(decimal.ToDouble(value * 100), GridUnitType.Star);
            Space = new GridLength(decimal.ToDouble(100 - value * 100), GridUnitType.Star);
        }
    }
}