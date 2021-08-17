using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VexTrack.MVVM.Validation
{
    public class DateTimeValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, $"This field cannot be empty, last known value will be used");
            bool canConvert = false;

            canConvert = DateTimeOffset.TryParse(strValue, out _);
            return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input must be a valid date and time, last known value will be used");
        }
    }
}
