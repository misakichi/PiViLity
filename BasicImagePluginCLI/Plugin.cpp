#include "Plugin.h"
using namespace BasicImagePluginCLI;

/// <summary>
/// プラグインの説明
/// </summary>
System::String^ PluginInformation::Description::get()
{
	return "Default implementation plugin.";
}
/// <summary>
/// プラグイン名称
/// </summary>
System::String^ PluginInformation::Name::get()
{
	return "Default Plugin";
}

/// <summary>
/// プラグイン名称
/// </summary>
System::String^ PluginInformation::OptionItemName::get()
{
	return "Default Plugin";
}