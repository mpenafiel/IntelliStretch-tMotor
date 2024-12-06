using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntelliStretch.Data;

namespace IntelliStretch.Interfaces
{
    interface IUpdateUI
    {
        void Update_UI(IntelliSerialPort.AnkleData newAnkleData);
        void Set_Initial(bool IsInitial);
        void Set_DataMode(DataInfo.DataMode dataMode);
    }
}
