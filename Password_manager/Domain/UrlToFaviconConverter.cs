using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Password_manager.Domain
{
    public class UrlToFaviconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string websiteUrl)
            {
                try
                {
                    Uri websiteUri = new Uri(websiteUrl);
                    return new Uri(websiteUri.Scheme + "://" + websiteUri.Host + "/favicon.ico");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}