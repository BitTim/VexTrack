using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			string status = (string)value;
			string mode = (string)parameter;

			Path checkIcon = (Path)Application.Current.FindResource("CheckIcon");
			Path crossIcon = (Path)Application.Current.FindResource("CrossIcon");

			Brush green = (Brush)Application.Current.FindResource("AccGreen");
			Brush red = (Brush)Application.Current.FindResource("AccRed");

			if (mode == "Data")
			{
				if (status == "Done") return checkIcon.Data;
				if (status == "Failed") return crossIcon.Data;
				return null;
			}

			if(mode == "Color")
			{
				if (status == "Done") return green;
				if (status == "Failed") return red;
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
