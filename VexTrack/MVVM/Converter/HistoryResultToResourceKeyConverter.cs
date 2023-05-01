using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VexTrack.Core.Util;

namespace VexTrack.MVVM.Converter
{
	internal class HistoryResultToResourceKeyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var result = value as string;
			var property = parameter as string;

			if (result == null) return new SolidColorBrush(Colors.Transparent);
			
			if (result!.Split()[0] == "Surrendered") result = result.Split()[1];
			if (result == "Win" || result == "Loss")
			{
				switch (property)
				{
					case "Foreground":
						return new SolidColorBrush(Colors.White);
					case "Background":
						return Application.Current.FindResource(result);
				}
			}

			return property switch
			{
				"Foreground" => SettingsHelper.Data.Theme.ForegroundBrush,
				"Background" => SettingsHelper.Data.Theme.BackgroundBrush,
				_ => Colors.Transparent
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return "";
		}
	}
}
