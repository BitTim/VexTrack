using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VexTrack.MVVM.Validation
{
    public class LevelValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, $"This field cannot be empty, last known value will be used");
            bool canConvert = false;

            int val;
            canConvert = int.TryParse(strValue, out val);
            if (!canConvert) return new ValidationResult(false, $"Input must be a number, last known value will be used");

            if (val < 2) return new ValidationResult(false, $"Input cannot be less than 2");
            if (val > 55) return new ValidationResult(false, $"Input cannot be larger than 55");
            return new ValidationResult(true, null);
        }
    }
}
