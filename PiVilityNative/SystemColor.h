#pragma once

using namespace System;

#pragma unmanaged
#include <winrt/Windows.UI.ViewManagement.h>
using namespace winrt::Windows::UI::ViewManagement;
#pragma managed


namespace ShelAPIHelper {
    public enum class UIColorType
    {
        Background = winrt::Windows::UI::ViewManagement::UIColorType::Background,
        Foreground = winrt::Windows::UI::ViewManagement::UIColorType::Foreground,
        AccentDark3 = winrt::Windows::UI::ViewManagement::UIColorType::AccentDark3,
        AccentDark2 = winrt::Windows::UI::ViewManagement::UIColorType::AccentDark2,
        AccentDark1 = winrt::Windows::UI::ViewManagement::UIColorType::AccentDark1,
        Accent = winrt::Windows::UI::ViewManagement::UIColorType::Accent,
        AccentLight1 = winrt::Windows::UI::ViewManagement::UIColorType::AccentLight1,
        AccentLight2 = winrt::Windows::UI::ViewManagement::UIColorType::AccentLight2,
        AccentLight3 = winrt::Windows::UI::ViewManagement::UIColorType::AccentLight3,
        Complement = winrt::Windows::UI::ViewManagement::UIColorType::Complement,
    };

    public enum class UIElementType : uint32_t
    {
        ActiveCaption = winrt::Windows::UI::ViewManagement::UIElementType::ActiveCaption,
        Background = winrt::Windows::UI::ViewManagement::UIElementType::Background,
        ButtonFace = winrt::Windows::UI::ViewManagement::UIElementType::ButtonFace,
        ButtonText = winrt::Windows::UI::ViewManagement::UIElementType::ButtonText,
        CaptionText = winrt::Windows::UI::ViewManagement::UIElementType::CaptionText,
        GrayText = winrt::Windows::UI::ViewManagement::UIElementType::GrayText,
        Highlight = winrt::Windows::UI::ViewManagement::UIElementType::Highlight,
        HighlightText = winrt::Windows::UI::ViewManagement::UIElementType::HighlightText,
        Hotlight = winrt::Windows::UI::ViewManagement::UIElementType::Hotlight,
        InactiveCaption = winrt::Windows::UI::ViewManagement::UIElementType::InactiveCaption,
        InactiveCaptionText = winrt::Windows::UI::ViewManagement::UIElementType::InactiveCaptionText,
        Window = winrt::Windows::UI::ViewManagement::UIElementType::Window,
        WindowText = winrt::Windows::UI::ViewManagement::UIElementType::WindowText,
        AccentColor = winrt::Windows::UI::ViewManagement::UIElementType::AccentColor,
        TextHigh = winrt::Windows::UI::ViewManagement::UIElementType::TextHigh,
        TextMedium = winrt::Windows::UI::ViewManagement::UIElementType::TextMedium,
        TextLow = winrt::Windows::UI::ViewManagement::UIElementType::TextLow,
        TextContrastWithHigh = winrt::Windows::UI::ViewManagement::UIElementType::TextContrastWithHigh,
        NonTextHigh = winrt::Windows::UI::ViewManagement::UIElementType::NonTextHigh,
        NonTextMediumHigh = winrt::Windows::UI::ViewManagement::UIElementType::NonTextMediumHigh,
        NonTextMedium = winrt::Windows::UI::ViewManagement::UIElementType::NonTextMedium,
        NonTextMediumLow = winrt::Windows::UI::ViewManagement::UIElementType::NonTextMediumLow,
        NonTextLow = winrt::Windows::UI::ViewManagement::UIElementType::NonTextLow,
        PageBackground = winrt::Windows::UI::ViewManagement::UIElementType::PageBackground,
        PopupBackground = winrt::Windows::UI::ViewManagement::UIElementType::PopupBackground,
        OverlayOutsidePopup = winrt::Windows::UI::ViewManagement::UIElementType::OverlayOutsidePopup,
    };

	public ref class SystemColor
	{
	public:
		static bool IsDarkMode();
		static Drawing::Color BackGroundColor();
		static Drawing::Color ForeGroundColor();
		static Drawing::Color GetUIColor(ShelAPIHelper::UIColorType);
		static Drawing::Color GetUIElementColor(ShelAPIHelper::UIElementType);


	};

}