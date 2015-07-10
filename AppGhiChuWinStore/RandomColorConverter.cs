using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AppGhiChuWinStore
{
    public class RandomColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var rd = new Random();
            string n = "";
            switch (rd.Next(0, 4))
            {
                case 0:
                    n = "#43609c";
                    break;
                case 1:
                    n = "#E8A856";
                    break;
                case 2:
                    n = "#FF4B49";
                    break;
                case 3:
                    n = "#9843E8";
                    break;
                case 4:
                    n = "#62B1FF";
                    break;
            }
            return n;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
