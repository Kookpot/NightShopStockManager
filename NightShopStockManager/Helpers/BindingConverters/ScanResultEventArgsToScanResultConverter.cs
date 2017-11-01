using System;
using System.Globalization;
using Xamarin.Forms;

namespace NightShopStockManager.Helpers.BindingConverters
{
    public class ScanResultEventArgsToScanResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as ZXing.Result;
            return eventArgs.Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}