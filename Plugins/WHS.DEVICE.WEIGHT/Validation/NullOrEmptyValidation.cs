using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WHS.Infrastructure;

namespace WHS.DEVICE.WEIGHT.Validation
{
    public class NullOrEmptyValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, LocalizationHelp.GetLocalizedString(this.GetType().Assembly,"Validation.Commons.NotEmpty"));
            }
            return new ValidationResult(true, "");
        }
    }
}
