#include "Plugin.h"

/// <summary>
/// �v���O�C���̐���
/// </summary>
System::String^ BasicImagePluginCLI::PluginInformation::Description::get()
{
	return "Default implementation plugin.";
}
/// <summary>
/// �v���O�C������
/// </summary>
System::String^ BasicImagePluginCLI::PluginInformation::Name::get()
{
	return "Default Plugin";
}

/// <summary>
/// �v���O�C������
/// </summary>
System::String^ BasicImagePluginCLI::PluginInformation::OptionItemName::get()
{
	return "Default Plugin";
}

