#include "pch.h"
#include "SystemColor.h"

static inline bool _IsColorLight(const winrt::Windows::UI::Color& clr)
{
    return (((5 * clr.G) + (2 * clr.R) + clr.B) > (8 * 128));
}

using namespace ShelAPIHelper;

static System::Drawing::Color uiColorToClr(const winrt::Windows::UI::Color& color)
{
    return System::Drawing::Color::FromArgb(color.A,color.R,color.G,color.B);
}

void BackGroundColor()
{
    auto settings = UISettings();
    auto foreground = settings.GetColorValue(winrt::Windows::UI::ViewManagement::UIColorType::Background);
}
bool SystemColor::IsDarkMode()
{
    auto settings = UISettings();
    auto foreground = settings.GetColorValue(winrt::Windows::UI::ViewManagement::UIColorType::Foreground);
    auto clr = uiColorToClr(foreground);
    return _IsColorLight(foreground);
}

System::Drawing::Color SystemColor::BackGroundColor()
{
    auto settings = UISettings();
    return uiColorToClr(settings.GetColorValue(winrt::Windows::UI::ViewManagement::UIColorType::Background));
}

System::Drawing::Color SystemColor::ForeGroundColor()
{
    auto settings = UISettings();
    return uiColorToClr(settings.GetColorValue(winrt::Windows::UI::ViewManagement::UIColorType::Foreground));
}
System::Drawing::Color SystemColor::GetUIColor(ShelAPIHelper::UIColorType type)
{
    auto settings = UISettings();
    auto uiColor = winrt::Windows::UI::ViewManagement::UIColorType(type);
    auto color = settings.GetColorValue(uiColor);
    return uiColorToClr(color);
}
System::Drawing::Color SystemColor::GetUIElementColor(ShelAPIHelper::UIElementType type)
{
    auto settings = UISettings();
    auto uiColor = winrt::Windows::UI::ViewManagement::UIElementType(type);
    auto color = settings.UIElementColor(uiColor);
    return uiColorToClr(color);
}
