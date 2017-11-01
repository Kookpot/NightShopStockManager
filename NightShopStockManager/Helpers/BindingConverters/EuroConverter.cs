using System;
using System.Globalization;
using Xamarin.Forms;

namespace NightShopStockManager.Helpers.BindingConverters
{
    public class EuroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = (decimal)value;
            return "€" + eventArgs.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}