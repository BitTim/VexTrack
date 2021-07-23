using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class TimestampToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			long timestamp = (long)value;
			string noTime = (string)parameter;

			string str = "";
			DateTimeOffset dt = DateTimeOffset.FromUnixTimeSeconds(timestamp);

			if (noTime.ToLower() == "true") str = dt.ToString("d");
			else str = dt.ToString("g");

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = value as string;
			str += " +00:00";
			DateTimeOffset dto = DateTimeOffset.Parse(str);

			return dto.ToUnixTimeSeconds();
		}
	}
}
