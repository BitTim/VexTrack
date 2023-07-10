using System;
using System.Globalization;
using System.Windows.Controls;

namespace VexTrack.MVVM.Validation;

public class TierValidationRule : ValidationRule
{
	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		var strValue = Convert.ToString(value);

		if (string.IsNullOrEmpty(strValue)) return new ValidationResult(false, $"This field cannot be empty, last known value will be used");

		var canConvert = int.TryParse(strValue, out var val);
		if (!canConvert) return new ValidationResult(false, $"Input must be a number, last known value will be used");

		if (val < 1) return new ValidationResult(false, $"Input cannot be less than 1");
		if (val > 10) return new ValidationResult(false, $"Input cannot be larger than 10");
		return new ValidationResult(true, null);
	}
}