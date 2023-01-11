using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace VexTrack.MVVM.Validation
{
	public class ColorCodeValidation : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var strValue = Convert.ToString(value);

			if (string.IsNullOrEmpty(strValue))
				return new ValidationResult(false, $"This field cannot be empty, last known value will be used");
			bool canConvert;

			try
			{
				Brush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(strValue);
				canConvert = true;
			}
			catch
			{
				canConvert = false;
			}

			return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input is not a valid color");
		}
	}
}
