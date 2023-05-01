using System;
using System.Globalization;
using System.Windows.Data;
using VexTrack.Core.Helper;

namespace VexTrack.MVVM.Converter
{
	internal class TimestampToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var timestamp = (long?)value ?? 0;
			var noTime = (string)parameter ?? "";

			switch (timestamp)
			{
				case -1:
					return "Completed";
				case -2:
					return "Never";			// -2 is passed when no timestamp could be calculated because average is 0
			}

			var dt = TimeHelper.TimestampToTime(timestamp);

			var str = dt.ToString(noTime.ToLower() == "true" ? "d" : "g");
			if (str == "01.01.0001") str = "-";

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = value as string;

			return DateTimeOffset.TryParse(str, out var dto) == false
				? TimeHelper.NowTimestamp
				: dto.ToLocalTime().ToUnixTimeSeconds();
		}
	}
}
