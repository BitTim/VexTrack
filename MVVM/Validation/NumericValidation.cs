using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VexTrack.MVVM.Validation
{
    public class NumericValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, $"This field cannot be empty, last known value will be used");
            bool canConvert = false;

            canConvert = int.TryParse(strValue, out _);
            return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input must be a number, last known value will be used");
        }
    }
}
