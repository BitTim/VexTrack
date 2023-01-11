using System;
using System.Globalization;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class StyledIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = value.ToString();
			var param = (string)parameter;
			var str = val + " " + param;

			if (param == "NegativeToNone")
			{
				if ((int)value < 0) str = "-";
				else str = val;
			}

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = (string)value;
			var num = val.Split(" ")[0];
			return int.Parse(num);
		}
	}
}
