using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VexTrack.MVVM.Converter
{
	class StatusToBadgeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var status = (string)value;
			var mode = (string)parameter;

			var doneIcon = (Path)Application.Current.FindResource("DoneIcon");
			var doneAllIcon = (Path)Application.Current.FindResource("DoneAllIcon");
			var warnIcon = (Path)Application.Current.FindResource("WarnIcon");
			var crossIcon = (Path)Application.Current.FindResource("CrossIcon");

			var blue = (Brush)Application.Current.FindResource("AccBlue");
			var green = (Brush)Application.Current.FindResource("AccGreen");
			var yellow = (Brush)Application.Current.FindResource("AccYellow");
			var red = (Brush)Application.Current.FindResource("AccRed");

			return mode switch
			{
				"Data" => status switch
				{
					"DoneAll" => doneAllIcon?.Data,
					"Done" => doneIcon?.Data,
					"Warning" => warnIcon?.Data,
					"Failed" => crossIcon?.Data,
					_ => null
				},
				"Color" => status switch
				{
					"DoneAll" => blue,
					"Done" => green,
					"Warning" => yellow,
					"Failed" => red,
					_ => null
				},
				_ => null
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
