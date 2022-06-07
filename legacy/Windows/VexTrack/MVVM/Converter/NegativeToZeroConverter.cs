using System;
using System.Globalization;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class NegativeToZeroConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int val = (int)value;

			if (val < 0) return 0;
			return val;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
