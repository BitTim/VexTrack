using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VexTrack.Core.Util;

namespace VexTrack.MVVM.Model
{
	public class ProgressButtonModel : Button
	{
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ProgressButtonModel), new PropertyMetadata(""));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(ProgressButtonModel), new PropertyMetadata(0.0));
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(ProgressButtonModel), new PropertyMetadata(0.0));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(ProgressButtonModel), new PropertyMetadata(100.0));
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(string), typeof(ProgressButtonModel), new PropertyMetadata(""));
		public static readonly DependencyProperty BadgeRightDataProperty = DependencyProperty.Register(nameof(BadgeRightData), typeof(Geometry), typeof(ProgressButtonModel), new PropertyMetadata(null));
		public static readonly DependencyProperty BadgeRightColorProperty = DependencyProperty.Register(nameof(BadgeRightColor), typeof(Brush), typeof(ProgressButtonModel), new PropertyMetadata(SettingsHelper.Data.Theme.ForegroundBrush));
		public static readonly DependencyProperty BadgeLeftDataProperty = DependencyProperty.Register(nameof(BadgeLeftData), typeof(Geometry), typeof(ProgressButtonModel), new PropertyMetadata(null));
		public static readonly DependencyProperty BadgeLeftColorProperty = DependencyProperty.Register(nameof(BadgeLeftColor), typeof(Brush), typeof(ProgressButtonModel), new PropertyMetadata(SettingsHelper.Data.Theme.ForegroundBrush));

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			init => SetValue(TitleProperty, value);
		}

		public double Value
		{
			get => (double)GetValue(ValueProperty);
			init => SetValue(ValueProperty, value);
		}

		public double MinValue
		{
			get => (double)GetValue(MinValueProperty);
			init => SetValue(MinValueProperty, value);
		}

		public double MaxValue
		{
			get => (double)GetValue(MaxValueProperty);
			init => SetValue(MaxValueProperty, value);
		}

		public string Color
		{
			get => (string)GetValue(ColorProperty);
			init => SetValue(ColorProperty, value);
		}

		public Geometry BadgeRightData
		{
			get => (Geometry)GetValue(BadgeRightDataProperty);
			init => SetValue(BadgeRightDataProperty, value);
		}

		public Brush BadgeRightColor
		{
			get => (Brush)GetValue(BadgeRightColorProperty);
			init => SetValue(BadgeRightColorProperty, value);
		}

		public Geometry BadgeLeftData
		{
			get => (Geometry)GetValue(BadgeLeftDataProperty);
			init => SetValue(BadgeLeftDataProperty, value);
		}

		public Brush BadgeLeftColor
		{
			get => (Brush)GetValue(BadgeLeftColorProperty);
			init => SetValue(BadgeLeftColorProperty, value);
		}

		public bool IsCompleted => Math.Abs(Value - MaxValue) < 0.001;

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
