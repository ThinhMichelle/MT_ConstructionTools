using MichelleTool;
using MichelleTool.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kajima
{
    public class RequireValidation : System.Windows.Controls.ValidationRule
    {
        private string message = "";
        public RequireValidation()
        {
            message = MngLanguage.GetNameFromCurrentResource(() => Resources_en_US.GEN_InputRequireValidation);
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(((string)value).Trim()) == false)
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, message);
                }
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, ex.Message);
            }
        }
    }
}
