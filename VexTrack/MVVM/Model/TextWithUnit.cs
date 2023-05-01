using System.Windows;
using System.Windows.Controls;

namespace VexTrack.MVVM.Model
{
	public class TextWithUnit : Control
	{
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextWithUnit), new PropertyMetadata(""));
		public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(TextWithUnit), new PropertyMetadata(""));

		public static readonly DependencyProperty IsUnitSingularProperty = DependencyProperty.Register(nameof(IsUnitSingular), typeof(bool), typeof(TextWithUnit), new PropertyMetadata(false));

		public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register(nameof(TextFontSize), typeof(int), typeof(TextWithUnit), new PropertyMetadata(14));
		public static readonly DependencyProperty TextFontWeightProperty = DependencyProperty.Register(nameof(TextFontWeight), typeof(FontWeight), typeof(TextWithUnit), new PropertyMetadata(FontWeights.Normal));

		public static readonly DependencyProperty UnitFontSizeProperty = DependencyProperty.Register(nameof(UnitFontSize), typeof(int), typeof(TextWithUnit), new PropertyMetadata(14));
		public static readonly DependencyProperty UnitFontWeightProperty = DependencyProperty.Register(nameof(UnitFontWeight), typeof(FontWeight), typeof(TextWithUnit), new PropertyMetadata(FontWeights.Normal));

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}
		public string Unit
		{
			get => (string)GetValue(UnitProperty);
			set => SetValue(UnitProperty, value);
		}

		public bool IsUnitSingular
		{
			get => (bool)GetValue(IsUnitSingularProperty);
			set => SetValue(IsUnitSingularProperty, value);
		}

		public int TextFontSize
		{
			get => (int)GetValue(TextFontSizeProperty);
			set => SetValue(TextFontSizeProperty, value);
		}
		public FontWeight TextFontWeight
		{
			get => (FontWeight)GetValue(TextFontWeightProperty);
			set => SetValue(TextFontWeightProperty, value);
		}

		public int UnitFontSize
		{
			get => (int)GetValue(UnitFontSizeProperty);
			set => SetValue(UnitFontSizeProperty, value);
		}
		public FontWeight UnitFontWeight
		{
			get => (FontWeight)GetValue(UnitFontWeightProperty);
			set => SetValue(UnitFontWeightProperty, value);
		}

		static TextWithUnit()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TextWithUnit), new FrameworkPropertyMetadata(typeof(TextWithUnit)));
		}
	}
}