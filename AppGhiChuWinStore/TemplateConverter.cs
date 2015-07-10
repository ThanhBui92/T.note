using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AppGhiChuWinStore
{
    public class TemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
           object parameter, string language)
        {
            // The value parameter is the data from the source object.
            DateTime thisdate = DateTime.ParseExact(value.ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None);
            if (thisdate < DateTime.Now.AddMinutes(-1))
            {
                return "Visible";
            }
            else
                return "Collapsed";
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
