#include "Plugin.h"

/// <summary>
/// プラグインの説明
/// </summary>
System::String^ BasicImagePluginCLI::PluginInformation::Description::get()
{
	return "Default implementation plugin.";
}
/// <summary>
/// プラグイン名称
/// </summary>
System::String^ BasicImagePluginCLI::PluginInformation::Name::get()
{
	return "Default Plugin";
}

/// <summary>
/// プラグイン名称
/// </summary>
System::String^ BasicImagePluginCLI::PluginInformation::OptionItemName::get()
{
	return "Default Plugin";
}

