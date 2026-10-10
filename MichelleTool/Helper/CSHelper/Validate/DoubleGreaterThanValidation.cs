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
    public class DoubleGreaterThanValidation : System.Windows.Controls.ValidationRule
    {
        private double _Min = 0;

        public double Min
        {
            get { return _Min; }
            set { _Min = value; }
        }
        private string message = "";
        public DoubleGreaterThanValidation()
        {
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            message = MngLanguage.GetNameFromCurrentResource(() => Resources_ja_JP.GEN_InputRequireDoubleAndGreaterValue);
            message = string.Format(message, Min.ToString());
            try
            {
                string input = (string)value;
                if (input.EndsWith(".") || input == "-0" || (input.Length == 1 && input.StartsWith("-")) || (input.Contains(".") && input.EndsWith("0"))) return new ValidationResult(false, message);
                double output = 0;
                bool check = double.TryParse((string)value, out output);
                if (check && (output > Min && NumberUtils.IsEquals(output, Min) == false) && string.IsNullOrEmpty((string)value) == false)
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
