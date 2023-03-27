using System;
using System.Globalization;
using System.Windows.Data;

namespace VexTrack.MVVM.Converter
{
	class TimestampToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var timestamp = (long)value;
			var noTime = (string)parameter;

			switch (timestamp)
			{
				case -1:
					return "Completed";
				case -2:
					return "Never";			// -2 is passed when no timestamp could be calculated because average is 0
			}

			var str = "";
			var dt = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime();

			if (noTime.ToLower() == "true") str = dt.ToString("d");
			else str = dt.ToString("g");

			if (str == "01.01.0001") str = "-";

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = value as string;

			DateTimeOffset dto;
			if (DateTimeOffset.TryParse(str, out dto) == false) return DateTimeOffset.Now.ToUnixTimeSeconds();

			return dto.ToUnixTimeSeconds();
		}
	}
}
