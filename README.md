
**Note:** This repository contains a port to C# of the original library, which is written in Swift.

[![NuGet](https://img.shields.io/nuget/v/EasyTipView.svg?label=NuGet)](https://www.nuget.org/packages/EasyTipView/)
[![Build Status](https://www.bitrise.io/app/03eed555a70b3001/status.svg?token=bQC2er8s6MQ4oAMR23RtHQ&branch=master)](https://www.bitrise.io/app/03eed555a70b3001/status.svg?token=bQC2er8s6MQ4oAMR23RtHQ&branch=master)

--------------

<img src="https://raw.githubusercontent.com/teodorpatras/EasyTipView/master/assets/easytipview.png" alt="EasyTipView: fully customisable tooltip view written in Swift" style="width: 500px;"/>

Description
--------------

```EasyTipView``` is a fully customisable tooltip view that can be used as a call to action or informative tip.

|<img src="https://raw.githubusercontent.com/teodorpatras/EasyTipView/master/assets/easytipview.gif" width="320">|<img src="https://raw.githubusercontent.com/teodorpatras/EasyTipView/master/assets/static.png" width="320">|
|----------|-------------|

# Contents
1. [Features](#features)
3. [Installation](#installation)
4. [Supported OS & SDK versions](#supported-versions)
5. [Usage](#usage)
6. [Customising the appearance](#customising)
7. [Customising the presentation and dismissal animations](#customising-animations)
8. [License](#license)

## <a name="features"> Features </a>

- [x] Can be shown pointing to any ``UIView`` subclass.
- [x] support for any arrow direction `←, →, ↑, ↓`
- [x] Automatic orientation change adjustments.
- [x] Fully customisable appearance.
- [x] Fully customisable presentation and dismissal animations.


<a name="installation"> Installation </a>
--------------

#### Nuget

> Install-Package EasyTipView

#### Manually

If you prefer not to use either of the aforementioned dependency managers, you can integrate EasyTipView into your project manually.

<a name="supported-versions"> Supported OS & SDK Versions </a>
-----------------------------

* Supported build target - iOS 8+ (Xcode 8)

<a name="usage"> Usage </a>
--------------

1. Install package into your application project
2. Create a new EasyTipView control:

```c#
_myTooltip = new EasyTipView.EasyTipView();
_myTooltip.Text = new Foundation.NSString("This is a tooltip sample!");
_myTooltip.ArrowPosition = EasyTipView.ArrowPosition.Right;
_myTooltip.DidDismiss += (sender, e) => 
            {
                // do something on dismiss
            };

_myButton1.TouchUpInside += (sender, e) =>
{
    _myTooltip.Show(_myButton1, this.View, true);
};

_myButton2.TouchUpInside += (sender, e) =>
{
    _myTooltip.Dismiss();
};
```

As you can see from the example above, `Show` method takes three parameters:
- View to which the tooltip will be anchored
- Superview where the tooltip will be added on screen
- Animated will display / hide the tooltip using animations

<a name="customising"> Customising the appearance </a>
--------------
In order to customise the `EasyTipView` appearance and behaviour, you can play with the properties the widget exposes.

All customization possibilities can be splitted into three groups:
* **Drawing** - Properties related to the way ```EastTipView``` will be drawn on screen.
* **Positioning** - Properties related to the way ```EasyTipView``` will be drawn within its own bounds.
* **Animating** - Properties that will tell ```EasyTipView``` how to animate itself on and off screen.

#### Drawing

| Property   | Type | Default value |       Description      | 
|----------|----|----|------------|
|`CornerRadius`|float|5f| The corner radius of the tip view bubble.|
|`ArrowHeight`|float|5f| The height of the arrow positioned at the top or bottom of the bubble.|
|`ArrowWidth`|float|10f| The width of the above mentioned arrow.|
|`ForegroundColor`|UIColor|Black| The text color.|
|`BubbleColor`|UIColor|Red| The background color of the bubble.|
|`ArrowPosition`|ArrowPosition|ArrowPosition.Any| The position of the arrow. This can be: <br /> **+** `Top`: on top of the bubble <br /> **+** `Bottom`: at the bottom of the bubble.<br /> **+** `Left`: on the left of the bubble <br /> **+** `Right`: on the right of the bubble <br /> **+** `Any`: use this option to let the `EasyTipView` automatically find the best arrow position. <br />**If the passed in arrow cannot be applied due to layout restrictions, a different arrow position will be automatically assigned.**|
|`TextAlignment`|UITextAlignment|UITextAlignment.Center| The alignment of the text.|
|`BorderWidth`|float|0| Width of the optional border to be applied on the bubble.|
|`BorderColor`|UIColor|Clear| Color of the optional border to be applied on the bubble. **In order for the border to be applied, `BorderColor` needs to be different that `UIColor.Clear` and `BorderWidth` > 0**|
|`Font`|UIFont|UIFont.SystemFontOfSize(15f)| Font to be applied on the text. |


#### Positioning

| Property   | Type | Default value |       Description      | 
|----------|----|----|------------|
|`BubbleHInset`|float|1f| Horizontal bubble inset witin its container.|
|`BubbleVInset`|float|1f| Vertical bubble inset within its container.|
|`TextHInset`|float|10f| Text horizontal inset within the bubble.|
|`TextVInset`|float|10f| Text vertical inset within the bubble.|
|`MaxWidth`|float|200f| Max bubble width.|


#### Animating

| Property   | Type | Default value |       Description      | 
|----------|----|----|------------|
|`DismissTransform`|CGAffineTransform|CGAffineTransform.MakeScale(0.1f, 0.1f)| `CGAffineTransform` specifying how the bubble will be dismissed. |
|`ShowInitialTransform`|CGAffineTransform|CGAffineTransform.MakeScale(0f, 0f)| `CGAffineTransform` specifying the initial transform to be applied on the bubble before it is animated on screen. |
|`ShowFinalTransform`|CGAffineTransform|CGAffineTransform.MakeIdentity()| `CGAffineTransform` specifying how the bubble will be animated on screen. |
|`SpringDamping`|float|0.7f| Spring animation damping.|
|`SpringVelocity`|float|0.7f| Spring animation velocity.|
|`ShowInitialAlpha`|float|0|Initial alpha to be applied on the tip view before it is animated on screen.|
|`DismissFinalAlpha`|float|0|The alpha to be applied on the tip view when it is animating off screen.|
|`ShowDuration`|float|0.7f|Show animation duration.|
|`DismissDuration`|float|0.7f|Dismiss animation duration.|
|`DismissOnTap`|bool|true|Prevents view from dismissing on tap if it is set to false.|

<a name="customising-animations"> Customising the presentation or dismissal animations </a>
--------------

The default animations for showing or dismissing are scale up and down. If you want to change the default behaviour, you need to change some animating attributes. An example could be:

```C#
DismissTransform = CGAffineTransform.MakeTranslation(0,-15);
ShowInitialTransform = CGAffineTransform.MakeTranslation(0, -15);
ShowInitialAlpha = 0f;
ShowDuration = 1.5f;
DismissDuration = 1.5f;
```

This produces the following animations:
<br><br><img src="https://raw.githubusercontent.com/teodorpatras/EasyTipView/master/assets/animation.gif" width="200">


<a name="license"> License </a>
--------------

```EasyTipView``` is released under the MIT license. See the ```LICENSE``` file for details.

Logo was created using Bud Icons Launch graphic by [Budi Tanrim](http://buditanrim.co) from [FlatIcon](http://www.flaticon.com/) which is licensed under [Creative Commons BY 3.0](http://creativecommons.org/licenses/by/3.0/). Made with [Logo Maker](http://logomakr.com).
