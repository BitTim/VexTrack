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
using VexTrack.Core;

namespace VexTrack.MVVM.Model
{
	public class ColorPickerModel : Control
	{
		public static DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(int), typeof(ColorPickerModel), new PropertyMetadata(0, OnHSVChanged));
		public static DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(float), typeof(ColorPickerModel), new PropertyMetadata(0f, OnHSVChanged));
		public static DependencyProperty ValueProperty = DependencyProperty.Register("Lightness", typeof(float), typeof(ColorPickerModel), new PropertyMetadata(1f, OnHSVChanged));
		
		public static DependencyProperty RedProperty = DependencyProperty.Register("Red", typeof(byte), typeof(ColorPickerModel), new PropertyMetadata((byte)255, OnRGBChanged));
		public static DependencyProperty GreenProperty = DependencyProperty.Register("Green", typeof(byte), typeof(ColorPickerModel), new PropertyMetadata((byte)255, OnRGBChanged));
		public static DependencyProperty BlueProperty = DependencyProperty.Register("Blue", typeof(byte), typeof(ColorPickerModel), new PropertyMetadata((byte)255, OnRGBChanged));

		public static DependencyProperty HexProperty = DependencyProperty.Register("Hex", typeof(string), typeof(ColorPickerModel), new PropertyMetadata("#FFFFFF", OnHexChanged));

		public static DependencyProperty HighlightMarginProperty = DependencyProperty.Register("HighlightMargin", typeof(Thickness), typeof(ColorPickerModel), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

		public static DependencyProperty ColorBrushProperty = DependencyProperty.Register("ColorBrush", typeof(Brush), typeof(ColorPickerModel), new PropertyMetadata(null));
		public static DependencyProperty ValueBrushProperty = DependencyProperty.Register("ValueBrush", typeof(Brush), typeof(ColorPickerModel), new PropertyMetadata(null));

		public int Hue
		{
			get => (int)GetValue(HueProperty);
			set => SetValue(HueProperty, value);
		}
		public float Saturation
		{
			get => (float)GetValue(SaturationProperty);
			set => SetValue(SaturationProperty, value);
		}
		public float Value
		{
			get => (float)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public byte Red
		{
			get => (byte)GetValue(RedProperty);
			set => SetValue(RedProperty, value);
		}
		public byte Green
		{
			get => (byte)GetValue(GreenProperty);
			set => SetValue(GreenProperty, value);
		}
		public byte Blue
		{
			get => (byte)GetValue(BlueProperty);
			set => SetValue(BlueProperty, value);
		}

		public string Hex
		{
			get => (string)GetValue(HexProperty);
			set => SetValue(HexProperty, value);
		}


		public Thickness HighlightMargin
		{
			get => (Thickness)GetValue(HighlightMarginProperty);
			set => SetValue(HighlightMarginProperty, value);
		}

		public Brush ColorBrush
		{
			get => (Brush)GetValue(ColorBrushProperty);
			set => SetValue(ColorBrushProperty, value);
		}
		public Brush ValueBrush
		{
			get => (Brush)GetValue(ValueBrushProperty);
			set => SetValue(ValueBrushProperty, value);
		}

		private Border clickableBorder { get; set; }

		private static void OnHSVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ColorPickerModel)
			{
				ColorPickerModel p = d as ColorPickerModel;
				p.Update("HSV");
			}
		}

		private static void OnRGBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ColorPickerModel)
			{
				ColorPickerModel p = d as ColorPickerModel;
				p.Update("RGB");
			}
		}

		private static void OnHexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ColorPickerModel)
			{
				ColorPickerModel p = d as ColorPickerModel;
				p.Update("HEX");
			}
		}

		public Brush CreateBrush(Color startCol, Color endCol, string dir)
		{
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.StartPoint = new Point(0, 0);
			
			if (dir == "Horizontal") brush.EndPoint = new Point(1, 0);
			if (dir == "Vertical") brush.EndPoint = new Point(0, 1);

			GradientStop startBorder = new();
			startBorder.Color = startCol;
			startBorder.Offset = 0.0;
			brush.GradientStops.Add(startBorder);

			GradientStop start = new();
			start.Color = startCol;
			start.Offset = 0.04;
			brush.GradientStops.Add(start);

			GradientStop end = new();
			end.Color = endCol;
			end.Offset = 0.96;
			brush.GradientStops.Add(end);

			GradientStop endBorder = new();
			endBorder.Color = endCol;
			endBorder.Offset = 1.0;
			brush.GradientStops.Add(endBorder);

			return brush;
		}

		public ColorPickerModel()
		{
			ValueBrush = CreateBrush(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 0, 0), "Vertical");

			Update("HSV");
		}

		static ColorPickerModel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerModel), new FrameworkPropertyMetadata(typeof(ColorPickerModel)));
		}

		private void UpdateRGB(Color currentColor)
		{
			Red = currentColor.R;
			Green = currentColor.G;
			Blue = currentColor.B;
		}

		private void UpdateHSV(Color currentColor)
		{
			System.Drawing.Color convertedColor = ColorConv.ToSDColor(currentColor);

			Hue = (int)convertedColor.GetHue();
			Saturation = convertedColor.GetSaturation();
			Value = ColorUtil.getValue(convertedColor);

			if(clickableBorder != null) HighlightMargin = new Thickness(Saturation * clickableBorder.ActualWidth, (1f - Value) * clickableBorder.ActualHeight, 0, 0);
		}

		private void UpdateHex(Color currentColor)
		{
			string rawHex = currentColor.ToString();
			rawHex = rawHex.Remove(1, 2);
			Hex = rawHex;
		}

		private static bool _isUpdating = false;

		public void Update(string source)
		{
			if (_isUpdating) return;
			_isUpdating = true;

			Color currentColor;

			if (source == "RGB")
			{
				currentColor = Color.FromArgb(255, Red, Green, Blue);

				UpdateHSV(currentColor);
				UpdateHex(currentColor);
			}
			else if (source == "HSV")
			{
				currentColor = ColorConv.ToSWMColor(ColorUtil.ColorFromHSV(new ColorUtil.HSV(Hue, Saturation, Value)));

				UpdateRGB(currentColor);
				UpdateHex(currentColor);
			}
			else if (source == "HEX")
			{
				if (Hex == "" || Hex == null)
				{
					_isUpdating = false;
					return;
				}

				currentColor = (Color)ColorConverter.ConvertFromString(Hex);

				UpdateRGB(currentColor);
				UpdateHSV(currentColor);
			}

			Color colorMatrixHueColor = ColorConv.ToSWMColor(ColorUtil.ColorFromHSV(new ColorUtil.HSV(Hue, 1, 1)));
			ColorBrush = CreateBrush(Color.FromArgb(255, 255, 255, 255), colorMatrixHueColor, "Horizontal");

			_isUpdating = false;
		}

		public override void OnApplyTemplate()
		{
			clickableBorder = Template.FindName("PART_ClickableBorder", this) as Border;
			clickableBorder.MouseLeftButtonDown += new MouseButtonEventHandler(PART_ColorMatrix_MouseLeftButtonDown);
			clickableBorder.MouseLeftButtonUp += new MouseButtonEventHandler(PART_ColorMatrix_MouseLeftButtonUp);
			clickableBorder.MouseMove += new MouseEventHandler(PART_ColorMatrix_MouseMove);

			base.OnApplyTemplate();
		}

		private bool _mouseDown = false;

		private void PART_ColorMatrix_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) { _mouseDown = false; }
		private void PART_ColorMatrix_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_mouseDown = true;
			UpdateMouse(sender, e);
		}

		private void PART_ColorMatrix_MouseMove(object sender, MouseEventArgs e)
		{
			if (!_mouseDown) return;
			UpdateMouse(sender, e);
		}
		private void UpdateMouse(object sender, MouseEventArgs e)
		{
			Border clickableBorder = sender as Border;
			Point dimensions = new(clickableBorder.ActualWidth, clickableBorder.ActualHeight);
			Point mousePos = e.GetPosition(clickableBorder);

			if (mousePos.X < 0) mousePos.X = 0;
			if (mousePos.X > dimensions.X) mousePos.X = dimensions.X;
			if (mousePos.Y < 0) mousePos.X = 0;
			if (mousePos.Y > dimensions.Y) mousePos.X = dimensions.Y;

			Saturation = (float)mousePos.X / (float)dimensions.X;
			Value = 1f - ((float)mousePos.Y / (float)dimensions.Y);

			HighlightMargin = new Thickness(mousePos.X, mousePos.Y, 0, 0);
		}
	}
}
