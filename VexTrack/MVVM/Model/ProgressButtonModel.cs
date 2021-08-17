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
		public static DependencyProperty BadgeDataProperty = DependencyProperty.Register("BadgeData", typeof(Geometry), typeof(ProgressButtonModel), new PropertyMetadata(null));
		public static DependencyProperty BadgeColorProperty = DependencyProperty.Register("BadgeColor", typeof(Brush), typeof(ProgressButtonModel), new PropertyMetadata(Application.Current.FindResource("Foreground")));

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

		public Geometry BadgeData
		{
			get => (Geometry)GetValue(BadgeDataProperty);
			set => SetValue(BadgeDataProperty, value);
		}

		public Brush BadgeColor
		{
			get => (Brush)GetValue(BadgeColorProperty);
			set => SetValue(BadgeColorProperty, value);
		}

		public bool IsCompleted { get => Value == MaxValue; }

		static ProgressButtonModel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressButtonModel), new FrameworkPropertyMetadata(typeof(ProgressButtonModel)));
		}
		public ProgressButtonModel() { }
		public ProgressButtonModel(string title, double value, string color, double minValue = 0, double maxValue = 100, Geometry badgeData = null, Brush badgeColor = null)
		{
			Title = title;
			Value = value;
			Color = color;
			MinValue = minValue;
			MaxValue = maxValue;

			if(badgeData != null) BadgeData = badgeData;
			if(badgeColor != null) BadgeColor = badgeColor;
		}
	}
}
