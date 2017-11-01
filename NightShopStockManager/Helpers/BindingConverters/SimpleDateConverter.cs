using System;
using System.Globalization;
using Xamarin.Forms;

namespace NightShopStockManager.Helpers.BindingConverters
{
    public class SimpleDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime) value).ToString("dd/MM/yy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}