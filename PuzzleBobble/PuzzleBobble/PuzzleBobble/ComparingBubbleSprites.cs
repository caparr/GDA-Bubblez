using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBobble
{
    class ComparingBubbleSprites : IComparer<BubbleSprite>
    {
        public int Compare(BubbleSprite bs1, BubbleSprite bs2)
        {
            int compareYPos = bs2.position.Y.CompareTo(bs1.position.Y);
            if (compareYPos == 0)
            {
                return bs1.position.X.CompareTo(bs2.position.X);
            }
            return compareYPos;
        }
    }
}
