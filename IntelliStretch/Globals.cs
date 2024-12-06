using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IntelliStretch
{
    class Globals
    {
        public class Sound
        {
            public static readonly Uri ButtonClickUri = new Uri(@"pack://application:,,/Sounds/CLICK18B.WAV");
            public static readonly Uri SelectClickUri = new Uri(@"pack://application:,,/Sounds/CLICK10B.WAV");
            public static readonly Uri PageFlipUri = new Uri(@"pack://application:,,/Sounds/PageFlip.WAV");
            public static IntelliStretch.UserControls.AsyncSoundPlayer buttonSound, clickSound, pageSound;
        }

        
    }
}
