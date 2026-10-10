using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MichelleTool
{
    public class SubstringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intparam = System.Convert.ToInt32(parameter.ToString());
            if (intparam <= 0) return value;
            if (value is string valueStr)
            {
                if (valueStr.Length <= intparam) return value;
                var substr = valueStr.Substring(valueStr.Length - intparam, intparam);
                return $"...{substr}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
