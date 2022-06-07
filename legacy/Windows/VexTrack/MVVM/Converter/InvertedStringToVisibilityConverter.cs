using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class InvertedStringToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string val = (string)value;
			string param = (string)parameter;

			if (val == param) return Visibility.Hidden;
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
