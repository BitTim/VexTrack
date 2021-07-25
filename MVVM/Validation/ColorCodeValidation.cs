using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace VexTrack.MVVM.Validation
{
	public class ColorCodeValidation : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			string strValue = Convert.ToString(value);

			if (string.IsNullOrEmpty(strValue))
				return new ValidationResult(false, $"This field cannot be empty, last known value will be used");
			bool canConvert;

			Brush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(strValue);

			canConvert = brush != null;
			return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input is not a valid color");
		}
	}
}
