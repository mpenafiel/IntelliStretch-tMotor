using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace IntelliStretch.ValueConverters
{
    class ImageSourceConverter : IMultiValueConverter
    {


        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string imgSrc = AppDomain.CurrentDomain.BaseDirectory + @"Games\" + (string)values[0] + @"\" + (string)values[1];
                return new BitmapImage(new Uri(imgSrc));
            }
            catch (Exception)
            {
                return new BitmapImage();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
