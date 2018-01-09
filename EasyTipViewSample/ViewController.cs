using System;
using Cirrious.FluentLayouts.Touch;
using UIKit;

namespace EasyTipViewSample
{
    public partial class ViewController : UIViewController
    {
        private UIButton btnTest;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            this.btnTest = new UIButton();
            this.btnTest.SetTitle("Tap me", UIControlState.Normal);
            this.btnTest.SetTitleColor(UIColor.Blue, UIControlState.Normal);

            this.View.AddSubview(this.btnTest);
            this.View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            this.View.AddConstraints(
                this.btnTest.AtRightOf(this.View),
                this.btnTest.WithSameCenterY(this.View)
            );

            var etv = new EasyTipView.EasyTipView();
            etv.Text = new Foundation.NSString("This is a tooltip sample!");
            etv.ArrowPosition = EasyTipView.ArrowPosition.Right;

            this.btnTest.TouchUpInside += (sender, e) =>
            {
                etv.Show(this.btnTest, this.View, true);
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
