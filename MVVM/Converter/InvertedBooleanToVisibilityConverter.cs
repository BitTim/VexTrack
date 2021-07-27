using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class InvertedBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool val = (bool)value;
			bool param = bool.Parse((string)parameter);

			if (val)
			{
				if (param) return Visibility.Collapsed;
				return Visibility.Hidden;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
