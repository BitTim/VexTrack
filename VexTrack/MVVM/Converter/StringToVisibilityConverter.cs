using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class StringToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = (string)value;
			var param = (string)parameter;

			if (val == param) return Visibility.Visible;
			else return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
