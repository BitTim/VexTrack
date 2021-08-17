using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VexTrack.MVVM.Model
{
	public class ProgressArcModel : Control
	{
		public static DependencyProperty ShowPercentageProperty = DependencyProperty.Register("ShowPercentage", typeof(bool), typeof(ProgressArcModel), new PropertyMetadata(true, OnPropertyChanged));
		public static DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Point), typeof(ProgressArcModel), new PropertyMetadata(new Point(4, 4), OnPropertyChanged));

		public static DependencyProperty BackgroundOriginProperty = DependencyProperty.Register("BackgroundOrigin", typeof(Point), typeof(ProgressArcModel), new PropertyMetadata(new Point(62, 0)));
		public static DependencyProperty BackgroundStartPointProperty = DependencyProperty.Register("BackgroundStartPoint", typeof(Point), typeof(ProgressArcModel), new PropertyMetadata(new Point(62, 0)));
		public static DependencyProperty BackgroundEndPointProperty = DependencyProperty.Register("BackgroundEndPoint", typeof(Point), typeof(ProgressArcModel), new PropertyMetadata(new Point(61.99, 0)));
		public static DependencyProperty BackgroundRadiusProperty = DependencyProperty.Register("BackgroundRadius", typeof(Size), typeof(ProgressArcModel), new PropertyMetadata(new Size(62, 62)));
		public static DependencyProperty BackgroundThicknessProperty = DependencyProperty.Register("BackgroundThickness", typeof(double), typeof(ProgressArcModel), new PropertyMetadata(8.0));

		public static DependencyProperty ForegroundOriginProperty = DependencyProperty.Register("ForegroundOrigin", typeof(Point), typeof(ProgressArcModel), new PropertyMetadata(new Point(62, 0)));
		public static DependencyProperty ForegroundStartPointProperty = DependencyProperty.Register("ForegroundStartPoint", typeof(Point), typeof(ProgressArcModel), new PropertyMetadata(new Point(62, 0)));
		public static DependencyProperty ForegroundEndPointProperty = DependencyProperty.Register("ForegroundEndPoint", typeof(Point), typeof(ProgressArcModel), new PropertyMetadata(new Point(62, 0)));
		public static DependencyProperty ForegroundRadiusProperty = DependencyProperty.Register("ForegroundRadius", typeof(Size), typeof(ProgressArcModel), new PropertyMetadata(new Size(62, 62)));
		public static DependencyProperty ForegroundThicknessProperty = DependencyProperty.Register("ForegroundThickness", typeof(double), typeof(ProgressArcModel), new PropertyMetadata(8.0));
		public static DependencyProperty ForegroundAngleProperty = DependencyProperty.Register("ForegroundAngle", typeof(double), typeof(ProgressArcModel), new PropertyMetadata(0.0));
		public static DependencyProperty ForegroundBrushProperty = DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(ProgressArcModel), new PropertyMetadata(Application.Current.FindResource("Accent")));

		public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressArcModel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPropertyChanged));
		public static DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(string), typeof(ProgressArcModel), new PropertyMetadata("%", OnPropertyChanged));
		public static DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(ProgressArcModel), new PropertyMetadata(0.0, OnPropertyChanged));
		public static DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(ProgressArcModel), new PropertyMetadata(100.0, OnPropertyChanged));
		public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(string), typeof(ProgressArcModel), new PropertyMetadata("", OnPropertyChanged));

		public bool ShowPercentage
		{
			get => (bool)GetValue(ShowPercentageProperty);
			set => SetValue(ShowPercentageProperty, value);
		}

		public Point Offset
		{
			get => (Point)GetValue(OffsetProperty);
			set => SetValue(OffsetProperty, value);
		}



		public Point BackgroundOrigin
		{
			get => (Point)GetValue(BackgroundOriginProperty);
			set => SetValue(BackgroundOriginProperty, value);
		}
		
		public Point BackgroundStartPoint
		{
			get => (Point)GetValue(BackgroundStartPointProperty);
			set => SetValue(BackgroundStartPointProperty, value);
		}

		public Point BackgroundEndPoint
		{
			get => (Point)GetValue(BackgroundEndPointProperty);
			set => SetValue(BackgroundEndPointProperty, value);
		}

		public Size BackgroundRadius
		{
			get => (Size)GetValue(BackgroundRadiusProperty);
			set => SetValue(BackgroundRadiusProperty, value);
		}

		public double BackgroundThickness
		{
			get => (double)GetValue(BackgroundThicknessProperty);
			set => SetValue(BackgroundThicknessProperty, value);
		}


		public Point ForegroundOrigin
		{
			get => (Point)GetValue(ForegroundOriginProperty);
			set => SetValue(ForegroundOriginProperty, value);
		}

		public Point ForegroundStartPoint
		{
			get => (Point)GetValue(ForegroundStartPointProperty);
			set => SetValue(ForegroundStartPointProperty, value);
		}

		public Point ForegroundEndPoint
		{
			get => (Point)GetValue(ForegroundEndPointProperty);
			set => SetValue(ForegroundEndPointProperty, value);
		}

		public Size ForegroundRadius
		{
			get => (Size)GetValue(ForegroundRadiusProperty);
			set => SetValue(ForegroundRadiusProperty, value);
		}

		public double ForegroundThickness
		{
			get => (double)GetValue(ForegroundThicknessProperty);
			set => SetValue(ForegroundThicknessProperty, value);
		}

		public double ForegroundAngle
		{
			get => (double)GetValue(ForegroundAngleProperty);
			set => SetValue(ForegroundAngleProperty, value);
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
		
		public string Symbol
		{
			get => (string)GetValue(SymbolProperty);
			set => SetValue(SymbolProperty, value);
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

		public string Color
		{
			get => (string)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		static ProgressArcModel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressArcModel), new FrameworkPropertyMetadata(typeof(ProgressArcModel)));
		}

		public ProgressArcModel()
		{
			Update();
		}

		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ProgressArcModel)
			{
				ProgressArcModel p = d as ProgressArcModel;
				p.Update();
			}
		}

		private static bool _isUpdating = false;

		public void Update()
		{
			if (_isUpdating) return;
			_isUpdating = true;

			if (Value > MaxValue) Value = MaxValue;
			if (Value < MinValue) Value = MinValue;

			double percent = (Value - MinValue) * 100 / (MaxValue - MinValue);

			double angle = percent / 100 * 360;
			angle = angle == 360 ? 359.99 : angle;
			ForegroundAngle = angle;

			double px;
			double py;

			(px, py) = CalcPointFromProgress(percent, ForegroundRadius.Width);

			ForegroundEndPoint = new Point(px + Offset.X, py + Offset.Y);

			ForegroundStartPoint = new Point(ForegroundOrigin.X + Offset.X, ForegroundOrigin.Y + Offset.Y);
			BackgroundStartPoint = new Point(BackgroundOrigin.X + Offset.X, BackgroundOrigin.Y + Offset.Y);

			(px, py) = CalcPointFromProgress(100, BackgroundRadius.Width);

			BackgroundEndPoint = new Point(px + Offset.X, py + Offset.Y);

			if (Color == "" || Color == null) ForegroundBrush = (Brush)Application.Current.FindResource("Accent");
			else
			{
				Brush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(Color);
				ForegroundBrush = brush;
			}

			_isUpdating = false;
		}

		public (double, double) CalcPointFromProgress(double percent, double rad)
		{
			double angle = percent / 100 * 360;

			angle = angle == 360 ? 359.99 : angle;
			double angleInRadians = angle * Math.PI / 180;

			double px = rad + (Math.Sin(angleInRadians) * rad);
			double py = rad + (-Math.Cos(angleInRadians) * rad);

			return (px, py);
		}
	}
}
