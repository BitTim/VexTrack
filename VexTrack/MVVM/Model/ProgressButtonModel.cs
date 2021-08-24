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
	public class ProgressButtonModel : Button
	{
		public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ProgressButtonModel), new PropertyMetadata(""));
		public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressButtonModel), new PropertyMetadata(0.0));
		public static DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(ProgressButtonModel), new PropertyMetadata(0.0));
		public static DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(ProgressButtonModel), new PropertyMetadata(100.0));
		public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(string), typeof(ProgressButtonModel), new PropertyMetadata(""));
		public static DependencyProperty BadgeRightDataProperty = DependencyProperty.Register("BadgeRightData", typeof(Geometry), typeof(ProgressButtonModel), new PropertyMetadata(null));
		public static DependencyProperty BadgeRightColorProperty = DependencyProperty.Register("BadgeRightColor", typeof(Brush), typeof(ProgressButtonModel), new PropertyMetadata(Application.Current.FindResource("Foreground")));
		public static DependencyProperty BadgeLeftDataProperty = DependencyProperty.Register("BadgeLeftData", typeof(Geometry), typeof(ProgressButtonModel), new PropertyMetadata(null));
		public static DependencyProperty BadgeLeftColorProperty = DependencyProperty.Register("BadgeLeftColor", typeof(Brush), typeof(ProgressButtonModel), new PropertyMetadata(Application.Current.FindResource("Foreground")));

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
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

		public string Color
		{
			get => (string)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public Geometry BadgeRightData
		{
			get => (Geometry)GetValue(BadgeRightDataProperty);
			set => SetValue(BadgeRightDataProperty, value);
		}

		public Brush BadgeRightColor
		{
			get => (Brush)GetValue(BadgeRightColorProperty);
			set => SetValue(BadgeRightColorProperty, value);
		}

		public Geometry BadgeLeftData
		{
			get => (Geometry)GetValue(BadgeLeftDataProperty);
			set => SetValue(BadgeLeftDataProperty, value);
		}

		public Brush BadgeLeftColor
		{
			get => (Brush)GetValue(BadgeLeftColorProperty);
			set => SetValue(BadgeLeftColorProperty, value);
		}

		public bool IsCompleted { get => Value == MaxValue; }

		static ProgressButtonModel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressButtonModel), new FrameworkPropertyMetadata(typeof(ProgressButtonModel)));
		}
		public ProgressButtonModel() { }
		public ProgressButtonModel(string title, double value, string color, double minValue = 0, double maxValue = 100, Geometry badgeRightData = null, Brush badgeRightColor = null, Geometry badgeLeftData = null, Brush badgeLeftColor = null)
		{
			Title = title;
			Value = value;
			Color = color;
			MinValue = minValue;
			MaxValue = maxValue;

			if (badgeRightData != null) BadgeRightData = badgeRightData;
			if (badgeRightColor != null) BadgeRightColor = badgeRightColor;
			if (badgeLeftData != null) BadgeLeftData = badgeLeftData;
			if (badgeLeftColor != null) BadgeLeftColor = badgeLeftColor;
		}
	}
}
