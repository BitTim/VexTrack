using System;
using System.Globalization;
using System.Windows.Controls;

namespace VexTrack.MVVM.Validation;

public class NumericValidationRule : ValidationRule
{
	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		var strValue = Convert.ToString(value);

		if (string.IsNullOrEmpty(strValue)) return new ValidationResult(false, $"This field cannot be empty, last known value will be used");

		var canConvert = int.TryParse(strValue, out var val);
		if (!canConvert) return new ValidationResult(false, $"Input must be a number, last known value will be used");

		if (val < 0) return new ValidationResult(false, $"Input cannot be negative");
		return new ValidationResult(true, null);
	}
}