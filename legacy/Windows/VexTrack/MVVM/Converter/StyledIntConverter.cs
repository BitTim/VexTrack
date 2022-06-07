using System;
using System.Globalization;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class StyledIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string val = value.ToString();
			string param = (string)parameter;
			string str = val + " " + param;

			if (param == "NegativeToNone")
			{
				if ((int)value < 0) str = "-";
				else str = val;
			}

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string val = (string)value;
			string num = val.Split(" ")[0];
			return int.Parse(num);
		}
	}
}
