using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliStretch.Games
{
    public class LevelSelector
    {
        public LevelSelector() { }

        readonly int targetCountMax = 3;

        public int NewLevel { get; set; }

        public bool CheckGameLevel(int targetReachedCount)
        {
            bool IsLevelChanged = false;

            if (targetReachedCount > targetCountMax)
            {
                NewLevel++;
                IsLevelChanged = true;
            }
            else if (targetReachedCount < -2 && NewLevel > 1)
            {
                NewLevel--;
                IsLevelChanged = true;
            }

            return IsLevelChanged;
        }
    }
}
