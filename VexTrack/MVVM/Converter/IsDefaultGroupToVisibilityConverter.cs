using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VexTrack.Core;

namespace VexTrack.MVVM.Converter
{
	class IsDefaultGroupToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string val = value as string;

			if (val == Constants.DefaultGroupUUID) return Visibility.Collapsed;
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
