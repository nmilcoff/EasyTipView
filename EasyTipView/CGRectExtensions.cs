using System;
using CoreGraphics;
using UIKit;

namespace EasyTipView
{
    public static class CGRectExtensions
    {
        public static CGPoint GetCenter(this CGRect rect)
        {
            return new CGPoint(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
