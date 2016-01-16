using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualRx.Client.WPF
{
    public class BoolArrayToScaleTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 6)
                return ScaleType.Seconds;

            var res = Enum.GetValues(typeof(ScaleType));
            for (int i = 0; i < res.Length; i++)
            {
                if ((bool)values[i])
                    return res.GetValue(i);
            }

            return ScaleType.Seconds;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
