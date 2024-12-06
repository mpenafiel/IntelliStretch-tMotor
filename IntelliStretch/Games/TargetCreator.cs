using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliStretch.Games
{
    public class TargetCreator
    {
        public TargetCreator() 
        {
            rndGen = new Random(DateTime.Now.Millisecond);
        }

        public int NewTarget { get; set; }

        Random rndGen;
        int prevTarget;

        public void CreateTarget(int min, int max)
        {
            int mid = (max + min) / 2;
            if (prevTarget > mid)
                max = mid;
            else
                min = mid;

            int newTarget = rndGen.Next(min, max);

            prevTarget = newTarget;
            this.NewTarget = newTarget;
        }
    }
}
