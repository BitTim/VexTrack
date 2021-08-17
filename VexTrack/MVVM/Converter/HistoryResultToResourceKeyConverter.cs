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
	class HistoryResultToResourceKeyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string result = value as string;
			string property = parameter as string;

			if (result == "Win" || result == "Loss")
			{
				if (property == "Foreground") return Application.Current.FindResource("White");
				if (property == "Background") return Application.Current.FindResource(result + "Color");
			}

			if (property == "Foreground") return Application.Current.FindResource("Foreground");
			if (property == "Background") return Application.Current.FindResource("Background");
			return Application.Current.FindResource("Transparent");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return "";
		}
	}
}
