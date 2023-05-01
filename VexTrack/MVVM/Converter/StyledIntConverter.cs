using System;
using System.Globalization;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	internal class StyledIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = value!.ToString();
			var param = (string)parameter;
			var str = val + " " + param;

			if (param != "NegativeToNone") return str;
			str = (int)value < 0 ? "-" : val;

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = (string)value;
			var num = val!.Split(" ")[0];
			return int.Parse(num);
		}
	}
}
