using System;
using System.Globalization;
using System.Windows.Controls;

namespace VexTrack.MVVM.Validation
{
	public class DateTimeValidation : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var strValue = Convert.ToString(value);

			if (string.IsNullOrEmpty(strValue))
				return new ValidationResult(false, $"This field cannot be empty, last known value will be used");
			var canConvert = false;

			canConvert = DateTimeOffset.TryParse(strValue, out _);
			return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input must be a valid date and time, last known value will be used");
		}
	}
}
