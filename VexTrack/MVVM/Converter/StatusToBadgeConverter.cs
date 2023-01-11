using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VexTrack.MVVM.Converter
{
	class StatusToBadgeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var status = (string)value;
			var mode = (string)parameter;

			var checkIcon = (Path)Application.Current.FindResource("CheckIcon");
			var crossIcon = (Path)Application.Current.FindResource("CrossIcon");
			var pauseIcon = (Path)Application.Current.FindResource("LockIcon");
			var linkedIcon = (Path)Application.Current.FindResource("ChainIcon");

			var green = (Brush)Application.Current.FindResource("AccGreen");
			var red = (Brush)Application.Current.FindResource("AccRed");
			var foreground = (Brush)Application.Current.FindResource("Foreground");

			if (mode == "Data")
			{
				if (status == "Done") return checkIcon.Data;
				if (status == "Failed") return crossIcon.Data;
				if (status == "Paused") return pauseIcon.Data;
				if (status == "Linked") return linkedIcon.Data;
				return null;
			}

			if (mode == "Color")
			{
				if (status == "Done") return green;
				if (status == "Failed") return red;
				if (status == "Paused") return foreground;
				if (status == "Linked") return foreground;
				return null;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
