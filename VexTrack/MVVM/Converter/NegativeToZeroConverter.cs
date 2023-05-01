using System;
using System.Globalization;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	internal class NegativeToZeroConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = (int)value!;

			return val < 0 ? 0 : val;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
