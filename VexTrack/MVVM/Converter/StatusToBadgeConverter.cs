using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VexTrack.MVVM.Converter;

internal class StatusToBadgeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var status = (string)value;
		var mode = (string)parameter;

		var doneIcon = (Path)Application.Current.FindResource("DoneIcon");
		var doneAllIcon = (Path)Application.Current.FindResource("DoneAllIcon");
		var activeIcon = (Path)Application.Current.FindResource("ActiveIcon");
		var warnIcon = (Path)Application.Current.FindResource("WarnIcon");
		var crossIcon = (Path)Application.Current.FindResource("CrossIcon");

		var blue = (Brush)Application.Current.FindResource("Blue");
		var green = (Brush)Application.Current.FindResource("Win");
		var yellow = (Brush)Application.Current.FindResource("Yellow");
		var red = (Brush)Application.Current.FindResource("Loss");

		return mode switch
		{
			"Data" => status switch
			{
				"DoneAll" => doneAllIcon?.Data,
				"Done" => doneIcon?.Data,
				"Active" => activeIcon?.Data,
				"Warning" => warnIcon?.Data,
				"Failed" => crossIcon?.Data,
				_ => null
			},
			"Color" => status switch
			{
				"DoneAll" => blue,
				"Done" => green,
				"Active" => yellow,
				"Warning" => yellow,
				"Failed" => red,
				_ => null
			},
			_ => null
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}