using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VexTrack.Core.Helper;

namespace VexTrack.MVVM.Model;

public class ColorPickerModel : Control
{
	public static readonly DependencyProperty HueProperty = DependencyProperty.Register(nameof(Hue), typeof(int), typeof(ColorPickerModel), new PropertyMetadata(0, OnHSVChanged));
	public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(nameof(Saturation), typeof(float), typeof(ColorPickerModel), new PropertyMetadata(0f, OnHSVChanged));
	public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Lightness", typeof(float), typeof(ColorPickerModel), new PropertyMetadata(1f, OnHSVChanged));

	public static readonly DependencyProperty RedProperty = DependencyProperty.Register(nameof(Red), typeof(byte), typeof(ColorPickerModel), new PropertyMetadata((byte)255, OnRGBChanged));
	public static readonly DependencyProperty GreenProperty = DependencyProperty.Register(nameof(Green), typeof(byte), typeof(ColorPickerModel), new PropertyMetadata((byte)255, OnRGBChanged));
	public static readonly DependencyProperty BlueProperty = DependencyProperty.Register(nameof(Blue), typeof(byte), typeof(ColorPickerModel), new PropertyMetadata((byte)255, OnRGBChanged));

	public static readonly DependencyProperty HexProperty = DependencyProperty.Register(nameof(Hex), typeof(string), typeof(ColorPickerModel), new PropertyMetadata("#FFFFFF", OnHexChanged));

	public static readonly DependencyProperty HighlightMarginProperty = DependencyProperty.Register(nameof(HighlightMargin), typeof(Thickness), typeof(ColorPickerModel), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

	public static readonly DependencyProperty ColorBrushProperty = DependencyProperty.Register(nameof(ColorBrush), typeof(Brush), typeof(ColorPickerModel), new PropertyMetadata(null));
	public static readonly DependencyProperty ValueBrushProperty = DependencyProperty.Register(nameof(ValueBrush), typeof(Brush), typeof(ColorPickerModel), new PropertyMetadata(null));

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

	private float Value
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
		init => SetValue(ValueBrushProperty, value);
	}

	private Border ClickableBorder { get; set; }

	private static void OnHSVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not ColorPickerModel model) return;
		model.Update("HSV");
	}

	private static void OnRGBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not ColorPickerModel model) return;
		model.Update("RGB");
	}

	private static void OnHexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not ColorPickerModel model) return;
		model.Update("HEX");
	}

	private static Brush CreateBrush(Color startCol, Color endCol, string dir)
	{
		var brush = new LinearGradientBrush
		{
			StartPoint = new Point(0, 0)
		};

		brush.EndPoint = dir switch
		{
			"Horizontal" => new Point(1, 0),
			"Vertical" => new Point(0, 1),
			_ => brush.EndPoint
		};

		GradientStop startBorder = new()
		{
			Color = startCol,
			Offset = 0.0
		};
		brush.GradientStops.Add(startBorder);

		GradientStop start = new()
		{
			Color = startCol,
			Offset = 0.04
		};
		brush.GradientStops.Add(start);

		GradientStop end = new()
		{
			Color = endCol,
			Offset = 0.96
		};
		brush.GradientStops.Add(end);

		GradientStop endBorder = new()
		{
			Color = endCol,
			Offset = 1.0
		};
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

	private void UpdateRgb(Color currentColor)
	{
		Red = currentColor.R;
		Green = currentColor.G;
		Blue = currentColor.B;
	}

	private void UpdateHsv(Color currentColor)
	{
		var convertedColor = currentColor.ToSdColor();

		Hue = (int)convertedColor.GetHue();
		Saturation = convertedColor.GetSaturation();
		Value = ColorHelper.GetValue(convertedColor);

		if (ClickableBorder != null) HighlightMargin = new Thickness(Saturation * ClickableBorder.ActualWidth, (1f - Value) * ClickableBorder.ActualHeight, 0, 0);
	}

	private void UpdateHex(Color currentColor)
	{
		var rawHex = currentColor.ToString();
		rawHex = rawHex.Remove(1, 2);
		Hex = rawHex;
	}

	private static bool _isUpdating;

	private void Update(string source)
	{
		if (_isUpdating) return;
		_isUpdating = true;

		Color currentColor;

		switch (source)
		{
			case "RGB":
				currentColor = Color.FromArgb(255, Red, Green, Blue);

				UpdateHsv(currentColor);
				UpdateHex(currentColor);
				break;
			case "HSV":
				currentColor = ColorHelper.ColorFromHsv(new ColorHelper.Hsv(Hue, Saturation, Value)).ToSwmColor();

				UpdateRgb(currentColor);
				UpdateHex(currentColor);
				break;
			case "HEX" when string.IsNullOrEmpty(Hex):
				_isUpdating = false;
				return;
			case "HEX":
				currentColor = (Color)ColorConverter.ConvertFromString(Hex)!;

				UpdateRgb(currentColor);
				UpdateHsv(currentColor);
				break;
		}

		var colorMatrixHueColor = ColorHelper.ColorFromHsv(new ColorHelper.Hsv(Hue, 1, 1)).ToSwmColor();
		ColorBrush = CreateBrush(Color.FromArgb(255, 255, 255, 255), colorMatrixHueColor, "Horizontal");

		_isUpdating = false;
	}

	public override void OnApplyTemplate()
	{
		ClickableBorder = Template.FindName("PART_ClickableBorder", this) as Border;
		ClickableBorder!.MouseLeftButtonDown += PART_ColorMatrix_MouseLeftButtonDown;
		ClickableBorder.MouseLeftButtonUp += PART_ColorMatrix_MouseLeftButtonUp;
		ClickableBorder.MouseMove += PART_ColorMatrix_MouseMove;

		base.OnApplyTemplate();
	}

	private bool _mouseDown;

	private void PART_ColorMatrix_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		_mouseDown = false;
		Mouse.Capture(null);
	}
	private void PART_ColorMatrix_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		_mouseDown = true;
		Mouse.Capture(ClickableBorder);
		UpdateMouse(sender, e);
	}

	private void PART_ColorMatrix_MouseMove(object sender, MouseEventArgs e)
	{
		if (!_mouseDown) return;
		UpdateMouse(sender, e);
	}
	private void UpdateMouse(object sender, MouseEventArgs e)
	{
		var clickableBorder = sender as Border;
		Point dimensions = new(clickableBorder!.ActualWidth, clickableBorder.ActualHeight);
		var mousePos = e.GetPosition(clickableBorder);

		if (mousePos.X < 0) mousePos.X = 0;
		if (mousePos.X > dimensions.X) mousePos.X = dimensions.X;
		if (mousePos.Y < 0) mousePos.Y = 0;
		if (mousePos.Y > dimensions.Y) mousePos.Y = dimensions.Y;

		Saturation = (float)mousePos.X / (float)dimensions.X;
		Value = 1f - ((float)mousePos.Y / (float)dimensions.Y);

		HighlightMargin = new Thickness(mousePos.X, mousePos.Y, 0, 0);
	}
}