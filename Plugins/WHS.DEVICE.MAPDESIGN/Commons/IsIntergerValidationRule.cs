using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WHS.DEVICE.MAPDESIGN.Commons
{
    public class IsIntergerValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int index = 0;
            if (int.TryParse(value.ToString(), out index))
            {
                if ((index >= 0) && (index <= 100))
                    return new ValidationResult(true, null);
            }
            return new ValidationResult(false, string.Format("{0} is not between 0 and 100!", value));
        }
    }
}
