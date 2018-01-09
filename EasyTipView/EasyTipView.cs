using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Linq;

namespace EasyTipView
{
    public class EasyTipView : UIView
    {
        public event EventHandler DidDismiss;

        private CGPoint arrowTip = CGPoint.Empty;
        private NSObject orientationObserver;
        private UIView viewTarget;

        #region Drawing Properties

        public float CornerRadius { get; set; } = 5f;

        public float ArrowHeight { get; set; } = 5f;

        public float ArrowWidth { get; set; } = 10f;

        public UIColor ForegroundColor { get; set; } = UIColor.Black;

        public UIColor BubbleColor { get; set; } = UIColor.Red;

        public ArrowPosition ArrowPosition { get; set; } = ArrowPosition.Any;

        public UITextAlignment TextAlignment { get; set; } = UITextAlignment.Center;

        public float BorderWidth { get; set; } = 0f;

        public UIColor BorderColor { get; set; } = UIColor.Clear;

        public UIFont Font { get; set; } = UIFont.SystemFontOfSize(15f);

        #endregion

        #region Positioning Properties

        public float BubbleHInset { get; set; } = 1f;

        public float BubbleVInset { get; set; } = 1f;

        public float TextHInset { get; set; } = 10f;

        public float TextVInset { get; set; } = 10f;

        public float MaxWidth { get; set; } = 200f;

        #endregion

        #region Animating Properties

        public CGAffineTransform DismissTransform { get; set; } = CGAffineTransform.MakeScale(0.1f, 0.1f);

        public CGAffineTransform ShowInitialTransform { get; set; } = CGAffineTransform.MakeScale(0f, 0f);

        public CGAffineTransform ShowFinalTransform { get; set; } = CGAffineTransform.MakeIdentity();

        public float SpringDamping { get; set; } = 0.7f;

        public float SpringVelocity { get; set; } = 0.7f;

        public float ShowInitialAlpha { get; set; } = 0f;

        public float DismissFinalAlpha { get; set; } = 0f;

        public float ShowDuration { get; set; } = 0.7f;

        public float DismissDuration { get; set; } = 0.7f;

        public bool DismissOnTap { get; set; } = true;

        #endregion

        public EasyTipView()
        {
            this.orientationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, this.HandleRotation);

            this.BackgroundColor = UIColor.Clear;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(this.orientationObserver);
                this.orientationObserver?.Dispose();
                this.orientationObserver = null;
            }

            base.Dispose(disposing);
        }

        public NSString Text { get; set; }

        public void Show(UIView view, UIView superview, bool animated)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            var hostSuperview = superview ?? UIApplication.SharedApplication.KeyWindow;

            this.viewTarget = view;

            this.Arrange(hostSuperview);

            this.Transform = this.ShowInitialTransform;
            this.Alpha = this.ShowInitialAlpha;

            var tap = new UITapGestureRecognizer(
                () =>
                {
                    this.Dismiss();
                });
            tap.ShouldBegin = (recognizer) => this.DismissOnTap;
            this.AddGestureRecognizer(tap);

            hostSuperview.AddSubview(this);

            if (animated)
            {
                UIView.AnimateNotify(
                    this.ShowDuration,
                    0f,
                    this.SpringDamping,
                    this.SpringVelocity,
                    UIViewAnimationOptions.CurveEaseInOut,
                    () =>
                    {
                        this.Transform = this.ShowFinalTransform;
                        this.Alpha = 1f;
                    },
                    null);
            }
            else
            {
                this.Transform = this.ShowFinalTransform;
                this.Alpha = 1f;
            }
        }

        public void Dismiss()
        {
            UIView.AnimateNotify(
                this.DismissDuration,
                0f,
                this.SpringDamping,
                this.SpringVelocity,
                UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    this.Transform = this.DismissTransform;
                    this.Alpha = this.DismissFinalAlpha;
                },
                (finished) =>
                {
                    this.DidDismiss?.Invoke(this, new EventArgs());
                    this.RemoveFromSuperview();
                    this.Transform = CGAffineTransform.MakeIdentity();
                });
        }

        private void HandleRotation(NSNotification notification)
        {
            if (Superview == null)
                return;

            UIView.Animate(
                0.3f,
                () =>
                {
                    this.Arrange(Superview);
                    this.SetNeedsDisplay();
                });
        }

        private void Arrange(UIView superview)
        {
            var refFrame = this.viewTarget.ConvertRectToView(this.viewTarget.Bounds, superview);

            CGRect superviewFrame;
            if (superview is UIScrollView scrollView)
            {
                superviewFrame = new CGRect(scrollView.Frame.Location, scrollView.ContentSize);
            }
            else
            {
                superviewFrame = superview.Frame;
            }

            CGRect frame = this.ComputeFrame(this.ArrowPosition, refFrame, superviewFrame);

            if (!this.IsFrameValid(frame, refFrame))
            {
                foreach (ArrowPosition position in Enum.GetValues(typeof(ArrowPosition)))
                {
                    if (position == this.ArrowPosition)
                        continue;

                    var newFrame = this.ComputeFrame(position, refFrame, superviewFrame);

                    if (this.IsFrameValid(newFrame, refFrame))
                    {
                        if (position != ArrowPosition.Any)
                        {
                            Console.WriteLine($"EasyTipView - Info: The arrow position you chose {this.ArrowPosition} could not be applied. Instead, position {position} has been applied! Please specify position {ArrowPosition.Any} if you want EasyTipView to choose a position for you.");
                        }

                        frame = newFrame;
                        this.ArrowPosition = position;
                        break;
                    }
                }
            }

            nfloat arrowTipXOrigin = 0f;
            switch (this.ArrowPosition)
            {
                case ArrowPosition.Bottom:
                case ArrowPosition.Top:
                case ArrowPosition.Any:

                    if (frame.Width < refFrame.Width)
                        arrowTipXOrigin = this.ContentSize.Width / 2;
                    else
                        arrowTipXOrigin = Convert.ToSingle(Math.Abs(frame.X - refFrame.X) + refFrame.Width / 2);

                    arrowTip = new CGPoint(arrowTipXOrigin, this.ArrowPosition == ArrowPosition.Bottom ? this.ContentSize.Height - this.BubbleVInset : this.BubbleVInset);

                    break;

                case ArrowPosition.Left:
                case ArrowPosition.Right:

                    if (frame.Height < refFrame.Height)
                        arrowTipXOrigin = this.ContentSize.Height / 2;
                    else
                        arrowTipXOrigin = Convert.ToSingle(Math.Abs(frame.Y - refFrame.Y)) + refFrame.Height / 2;

                    arrowTip = new CGPoint(this.ArrowPosition == ArrowPosition.Left ? this.BubbleVInset : this.ContentSize.Width - this.BubbleVInset, arrowTipXOrigin);

                    break;
            }

            this.Frame = frame;
        }

        private CGRect ComputeFrame(ArrowPosition position, CGRect refFrame, CGRect superview)
        {
            nfloat xOrigin = 0, yOrigin = 0;

            switch (position)
            {
                case ArrowPosition.Top:
                case ArrowPosition.Any:

                    xOrigin = refFrame.GetCenter().X - this.ContentSize.Width / 2;
                    yOrigin = refFrame.Y + refFrame.Height;

                    break;

                case ArrowPosition.Bottom:

                    xOrigin = refFrame.GetCenter().X - this.ContentSize.Width / 2;
                    yOrigin = refFrame.Y - this.ContentSize.Height;

                    break;

                case ArrowPosition.Right:

                    xOrigin = refFrame.X - this.ContentSize.Width;
                    yOrigin = refFrame.GetCenter().Y - this.ContentSize.Height / 2;

                    break;

                case ArrowPosition.Left:

                    xOrigin = refFrame.X + refFrame.Width;
                    yOrigin = refFrame.Y - this.ContentSize.Height / 2;

                    break;
            }

            var frame = new CGRect(xOrigin, yOrigin, this.ContentSize.Width, this.ContentSize.Height);

            this.AdjustFrame(ref frame, superview);

            return frame;
        }

        private void AdjustFrame(ref CGRect frame, CGRect superview)
        {
            if (frame.X < 0)
            {
                frame.X = 0;
            }
            else if (frame.GetMaxX() > superview.Width)
            {
                frame.X = superview.Width - frame.Width;
            }

            if (frame.Y < 0)
            {
                frame.Y = 0;
            }
            else if (frame.GetMaxY() > superview.GetMaxY())
            {
                frame.Y = superview.Height - frame.Height;
            }
        }

        private bool IsFrameValid(CGRect frame, CGRect forRefViewFrame)
        {
            return !frame.IntersectsWith(forRefViewFrame);
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            nfloat bubbleWidth = 0, bubbleHeight = 0, bubbleXOrigin = 0, bubbleYOrigin = 0;

            switch (this.ArrowPosition)
            {
                case ArrowPosition.Bottom:
                case ArrowPosition.Top:
                case ArrowPosition.Any:

                    bubbleWidth = this.ContentSize.Width - 2f * this.BubbleHInset;
                    bubbleHeight = this.ContentSize.Height - 2f * this.BubbleVInset - this.ArrowHeight;

                    bubbleXOrigin = this.BubbleHInset;
                    bubbleYOrigin = this.ArrowPosition == ArrowPosition.Bottom
                                                                  ? this.BubbleVInset
                                                                  : this.BubbleVInset + this.ArrowHeight;

                    break;

                case ArrowPosition.Left:
                case ArrowPosition.Right:

                    bubbleWidth = this.ContentSize.Width - 2f * this.BubbleHInset - this.ArrowHeight;
                    bubbleHeight = this.ContentSize.Height - 2f * this.BubbleVInset;

                    bubbleXOrigin = this.ArrowPosition == ArrowPosition.Right
                                        ? this.BubbleHInset
                                        : this.BubbleHInset + this.ArrowHeight;
                    bubbleYOrigin = this.BubbleVInset;

                    break;
            }

            var bubbleFrame = new CGRect(bubbleXOrigin, bubbleYOrigin, bubbleWidth, bubbleHeight);
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();

            this.DrawBubble(bubbleFrame, context);
            this.DrawText(bubbleFrame, context);

            context.RestoreState();
        }

        private void DrawBubble(CGRect frame, CGContext context)
        {
            var contourPath = new CGPath();
            contourPath.MoveToPoint(this.arrowTip.X, this.arrowTip.Y);

            switch (this.ArrowPosition)
            {
                case ArrowPosition.Bottom:
                case ArrowPosition.Top:
                case ArrowPosition.Any:

                    contourPath.AddLineToPoint(
                        new CGPoint(this.arrowTip.X - this.ArrowWidth / 2,
                                    this.arrowTip.Y + (this.ArrowPosition == ArrowPosition.Bottom ? -1 : 1) * this.ArrowHeight));

                    if (this.ArrowPosition == ArrowPosition.Bottom)
                    {
                        this.DrawBubbleBottomShape(frame, contourPath);
                    }
                    else
                    {
                        this.DrawBubbleTopShape(frame, contourPath);
                    }

                    contourPath.AddLineToPoint(
                        new CGPoint(this.arrowTip.X + this.ArrowWidth / 2,
                                    this.arrowTip.Y + (this.ArrowPosition == ArrowPosition.Bottom ? -1 : 1) * this.ArrowHeight));

                    break;

                case ArrowPosition.Left:
                case ArrowPosition.Right:

                    contourPath.AddLineToPoint(
                        new CGPoint(this.arrowTip.X + (this.ArrowPosition == ArrowPosition.Right ? -1 : 1) * this.ArrowHeight,
                                    this.arrowTip.Y - this.ArrowWidth / 2));

                    if (this.ArrowPosition == ArrowPosition.Right)
                    {
                        DrawBubbleRightShape(frame, contourPath);
                    }
                    else
                    {
                        DrawBubbleLeftShape(frame, contourPath);
                    }

                    contourPath.AddLineToPoint(
                        new CGPoint(this.arrowTip.X + (this.ArrowPosition == ArrowPosition.Right ? -1 : 1) * this.ArrowHeight,
                                    this.arrowTip.Y + this.ArrowWidth / 2));

                    break;
            }

            contourPath.CloseSubpath();
            context.AddPath(contourPath);
            context.Clip();

            this.PaintBubble(context);

            if (BorderWidth > 0f && BorderColor != UIColor.Clear)
            {
                this.DrawBorder(contourPath, context);
            }
        }

        private void DrawText(CGRect rect, CGContext context)
        {
            var paragraph = new NSMutableParagraphStyle();
            paragraph.Alignment = this.TextAlignment;
            paragraph.LineBreakMode = UILineBreakMode.WordWrap;

            var textRect = new CGRect(
                rect.X + (rect.Size.Width - this.TextSize.Width) / 2,
                rect.Y + (rect.Size.Height - this.TextSize.Height) / 2,
                this.TextSize.Width,
                this.TextSize.Height);

            this.Text.DrawString(
                textRect,
                new UIStringAttributes { Font = this.Font, ForegroundColor = this.ForegroundColor, ParagraphStyle = paragraph });
        }

        private void PaintBubble(CGContext context)
        {
            context.SetFillColor(this.BubbleColor.CGColor);
            context.FillRect(this.Bounds);
        }

        private void DrawBorder(CGPath borderPath, CGContext context)
        {
            context.AddPath(borderPath);
            context.SetStrokeColor(this.BorderColor.CGColor);
            context.SetLineWidth(this.BorderWidth);
            context.StrokePath();
        }

        private CGSize ContentSize
        {
            get
            {
                var currentTextSize = this.TextSize;

                var contentSize = new CGSize(
                    currentTextSize.Width + this.TextHInset * 2 + this.BubbleHInset * 2,
                    currentTextSize.Height + this.TextVInset * 2 + this.BubbleVInset * 2 + this.ArrowHeight);

                return contentSize;
            }
        }

        private CGSize TextSize
        {
            get
            {
                var attributes = new UIStringAttributes { Font = this.Font };

                var size = this.Text.GetBoundingRect(
                    new CGSize(this.MaxWidth, nfloat.MaxValue),
                    NSStringDrawingOptions.UsesLineFragmentOrigin,
                    attributes,
                    null).Size;

                size.Width = Convert.ToSingle(Math.Ceiling(size.Width));
                size.Height = Convert.ToSingle(Math.Ceiling(size.Height));

                if (size.Width < this.ArrowWidth)
                {
                    size.Width = this.ArrowWidth;
                }

                return size;
            }
        }

        private void DrawBubbleBottomShape(CGRect frame, CGPath path)
        {
            path.AddArcToPoint(
                frame.X,
                frame.Y + frame.Height,
                frame.X,
                frame.Y,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X,
                frame.Y,
                frame.X + frame.Width,
                frame.Y,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y,
                frame.X + frame.Width,
                frame.Y + frame.Height,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y + frame.Height,
                frame.X,
                frame.Y + frame.Height,
                this.CornerRadius);
        }

        private void DrawBubbleTopShape(CGRect frame, CGPath path)
        {
            path.AddArcToPoint(
                frame.X,
                frame.Y,
                frame.X,
                frame.Y + frame.Height,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X,
                frame.Y + frame.Height,
                frame.X + frame.Width,
                frame.Y + frame.Height,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y + frame.Height,
                frame.X + frame.Width,
                frame.Y,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y,
                frame.X,
                frame.Y,
                this.CornerRadius);
        }

        private void DrawBubbleRightShape(CGRect frame, CGPath path)
        {
            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y,
                frame.X,
                frame.Y,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X,
                frame.Y,
                frame.X,
                frame.Y + frame.Height,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X,
                frame.Y + frame.Height,
                frame.X + frame.Width,
                frame.Y + frame.Height,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y + frame.Height,
                frame.X + frame.Width,
                frame.Height,
                this.CornerRadius);
        }

        private void DrawBubbleLeftShape(CGRect frame, CGPath path)
        {
            path.AddArcToPoint(
                frame.X,
                frame.Y,
                frame.X + frame.Width,
                frame.Y,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y,
                frame.X + frame.Width,
                frame.Y + frame.Height,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X + frame.Width,
                frame.Y + frame.Height,
                frame.X,
                frame.Y + frame.Height,
                this.CornerRadius);

            path.AddArcToPoint(
                frame.X,
                frame.Y + frame.Height,
                frame.X,
                frame.Y,
                this.CornerRadius);
        }
    }
}
