using System;
using System.Globalization;
using System.Windows.Data;

namespace LegacyUpdateUtil.MVVM.Converter
{
	class AngleToIsLargeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double angle = (double)value;

			if (angle >= 180) return true;
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
