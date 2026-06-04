using System;
using System.Collections.Generic;
using System.Text;

//using Windows.UI.ViewManagement;
using Windows.UI.ViewManagement;

namespace PiViLityCore.Windows
{
    //internal enum UIColorType
    //{
    //    Background = Windows.UI.ViewManagement.UIColorType.Background,
    //    Foreground = Windows.UI.ViewManagement.UIColorType.Foreground,
    //    AccentDark3 = Windows.UI.ViewManagement.UIColorType.AccentDark3,
    //    AccentDark2 = Windows.UI.ViewManagement.UIColorType.AccentDark2,
    //    AccentDark1 = Windows.UI.ViewManagement.UIColorType.AccentDark1,
    //    Accent = Windows.UI.ViewManagement.UIColorType.Accent,
    //    AccentLight1 = Windows.UI.ViewManagement.UIColorType.AccentLight1,
    //    AccentLight2 = Windows.UI.ViewManagement.UIColorType.AccentLight2,
    //    AccentLight3 = Windows.UI.ViewManagement.UIColorType.AccentLight3,
    //    Complement = Windows.UI.ViewManagement.UIColorType.Complement,
    //};

    //internal enum UIElementType
    //{
    //    ActiveCaption = Windows.UI.ViewManagement.UIElementType.ActiveCaption,
    //    Background = Windows.UI.ViewManagement.UIElementType.Background,
    //    ButtonFace = Windows.UI.ViewManagement.UIElementType.ButtonFace,
    //    ButtonText = Windows.UI.ViewManagement.UIElementType.ButtonText,
    //    CaptionText = Windows.UI.ViewManagement.UIElementType.CaptionText,
    //    GrayText = Windows.UI.ViewManagement.UIElementType.GrayText,
    //    Highlight = Windows.UI.ViewManagement.UIElementType.Highlight,
    //    HighlightText = Windows.UI.ViewManagement.UIElementType.HighlightText,
    //    Hotlight = Windows.UI.ViewManagement.UIElementType.Hotlight,
    //    InactiveCaption = Windows.UI.ViewManagement.UIElementType.InactiveCaption,
    //    InactiveCaptionText = Windows.UI.ViewManagement.UIElementType.InactiveCaptionText,
    //    Window = Windows.UI.ViewManagement.UIElementType.Window,
    //    WindowText = Windows.UI.ViewManagement.UIElementType.WindowText,
    //    AccentColor = Windows.UI.ViewManagement.UIElementType.AccentColor,
    //    TextHigh = Windows.UI.ViewManagement.UIElementType.TextHigh,
    //    TextMedium = Windows.UI.ViewManagement.UIElementType.TextMedium,
    //    TextLow = Windows.UI.ViewManagement.UIElementType.TextLow,
    //    TextContrastWithHigh = Windows.UI.ViewManagement.UIElementType.TextContrastWithHigh,
    //    NonTextHigh = Windows.UI.ViewManagement.UIElementType.NonTextHigh,
    //    NonTextMediumHigh = Windows.UI.ViewManagement.UIElementType.NonTextMediumHigh,
    //    NonTextMedium = Windows.UI.ViewManagement.UIElementType.NonTextMedium,
    //    NonTextMediumLow = Windows.UI.ViewManagement.UIElementType.NonTextMediumLow,
    //    NonTextLow = Windows.UI.ViewManagement.UIElementType.NonTextLow,
    //    PageBackground = Windows.UI.ViewManagement.UIElementType.PageBackground,
    //    PopupBackground = Windows.UI.ViewManagement.UIElementType.PopupBackground,
    //    OverlayOutsidePopup = Windows.UI.ViewManagement.UIElementType.OverlayOutsidePopup,
    //};

    public static class SystemColor
    {
        private static bool _IsColorLight(global::Windows.UI.Color clr)
        {
            return (((5 * clr.G) + (2 * clr.R) + clr.B) > (8 * 128));
        }


        static System.Drawing.Color uiColorToClr(global::Windows.UI.Color color)
        {
            return System.Drawing.Color.FromArgb((int)color.A, (int)color.R, (int)color.G, (int)color.B);
        }

        public static bool IsDarkMode()
        {
            var settings = new UISettings();
            var foreground = settings.GetColorValue(UIColorType.Background);

            var clr = uiColorToClr(foreground);
            return _IsColorLight(foreground);
        }


        public static System.Drawing.Color BackGroundColor()
        {
            var settings = new UISettings();
            return uiColorToClr(settings.GetColorValue(UIColorType.Background));
        }

        public static System.Drawing.Color ForeGroundColor()
        {
            var settings = new UISettings();
            return uiColorToClr(settings.GetColorValue(UIColorType.Foreground));
        }

        public static System.Drawing.Color GetUIColor(UIColorType type)
        {
            var settings = new UISettings();
            var color = settings.GetColorValue(type);
            return uiColorToClr(color);
        }

        public static System.Drawing.Color GetUIElementColor(UIElementType type)
        {
            var settings = new UISettings();
            var color = settings.UIElementColor(type);
            return uiColorToClr(color);
        }
    }
}
