using System;
using System.Globalization;
using System.Windows.Controls;

namespace VexTrack.MVVM.Validation;

public class PercentageValidationRule : ValidationRule
{
	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		var strValue = Convert.ToString(value);

		if (string.IsNullOrEmpty(strValue)) return new ValidationResult(false, $"This field cannot be empty, last known value will be used");

		var canConvert = double.TryParse(strValue.Replace(',', '.'), CultureInfo.InvariantCulture, out var val);
		if (!canConvert) return new ValidationResult(false, $"Input must be a number, last known value will be used");

		return val switch
		{
			< 0.0 => new ValidationResult(false, $"Input cannot be negative"),
			> 100.0 => new ValidationResult(false, $"Cannot be more than 100 percent"),
			_ => new ValidationResult(true, null)
		};
	}
}