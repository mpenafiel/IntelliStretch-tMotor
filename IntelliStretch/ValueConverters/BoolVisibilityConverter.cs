using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace IntelliStretch.ValueConverters
{
    class BoolVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsForward = System.Convert.ToBoolean(parameter);

            if (IsForward)
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            else
                return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsForward = System.Convert.ToBoolean(parameter);

            if (IsForward)
                return ((Visibility)value == Visibility.Visible);
            else
                return ((Visibility)value == Visibility.Collapsed);  
            
        }


        #endregion
    }
}
