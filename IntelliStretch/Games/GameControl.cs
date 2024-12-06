using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliStretch.Games
{
    class GameControl
    {
        public GameControl()
        {
            LevelSelector = new LevelSelector();
            TargetCreator = new TargetCreator();
        }

        public LevelSelector LevelSelector { get; set; }
        public TargetCreator TargetCreator { get; set; }
    }
}
