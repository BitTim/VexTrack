using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
