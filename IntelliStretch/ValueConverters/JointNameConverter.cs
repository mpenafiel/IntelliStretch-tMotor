using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IntelliStretch.ValueConverters
{
    class JointNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string jointName = "Undefined";

            switch ((Protocols.Joint) value)
            {
                case Protocols.Joint.All:
                    jointName = "Ankle & Elbow & Knee & Wrist";
                    break;
                case Protocols.Joint.Ankle:
                    jointName = "Ankle";
                    break;
                case Protocols.Joint.Knee:
                    jointName = "Knee";
                    break;
                case Protocols.Joint.Elbow:
                    jointName = "Elbow";
                    break;
                case Protocols.Joint.Wrist:
                    jointName = "Wrist";
                    break;
                case Protocols.Joint.Arm:
                    jointName = "Elbow & Wrist";
                    break;
                default:
                    break;
            }

            return jointName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
