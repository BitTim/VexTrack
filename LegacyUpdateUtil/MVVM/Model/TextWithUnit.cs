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

namespace LegacyUpdateUtil.MVVM.Model
{
	public class TextWithUnit : Control
	{
		public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextWithUnit), new PropertyMetadata(""));
		public static DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(TextWithUnit), new PropertyMetadata(""));

		public static DependencyProperty IsUnitSingularProperty = DependencyProperty.Register("IsUnitSingular", typeof(bool), typeof(TextWithUnit), new PropertyMetadata(false));

		public static DependencyProperty TextFontSizeProperty = DependencyProperty.Register("TextFontSize", typeof(int), typeof(TextWithUnit), new PropertyMetadata(14));
		public static DependencyProperty TextFontWeightProperty = DependencyProperty.Register("TextFontWeight", typeof(FontWeight), typeof(TextWithUnit), new PropertyMetadata(FontWeights.Normal));

		public static DependencyProperty UnitFontSizeProperty = DependencyProperty.Register("UnitFontSize", typeof(int), typeof(TextWithUnit), new PropertyMetadata(14));
		public static DependencyProperty UnitFontWeightProperty = DependencyProperty.Register("UnitFontWeight", typeof(FontWeight), typeof(TextWithUnit), new PropertyMetadata(FontWeights.Normal));

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