using System;
using System.Globalization;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	internal class DataStringToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = (string)value;
			var param = (string)parameter;

			return val == param;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
