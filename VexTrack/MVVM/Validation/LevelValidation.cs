using System;
using System.Globalization;
using System.Windows.Controls;
using VexTrack.Core.Model;

namespace VexTrack.MVVM.Validation;

public class LevelInitValidationRule : ValidationRule
{
	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		var strValue = Convert.ToString(value);
		var numGoals = ApiData.ActiveSeasonTemplate.Goals.Count + 1;

		if (string.IsNullOrEmpty(strValue)) return new ValidationResult(false, $"This field cannot be empty, last known value will be used");

		var canConvert = int.TryParse(strValue, out var val);
		if (!canConvert) return new ValidationResult(false, $"Input must be a number, last known value will be used");

		if (val < 2) return new ValidationResult(false, $"Input cannot be less than 2");
		if (val > numGoals) return new ValidationResult(false, $"Input cannot be larger than " + numGoals.ToString());
		return new ValidationResult(true, null);
	}
}