#include "Plugin.h"
using namespace BasicImagePluginCLI;

/// <summary>
/// �v���O�C���̐���
/// </summary>
System::String^ PluginInformation::Description::get()
{
	return "Default implementation plugin.";
}
/// <summary>
/// �v���O�C������
/// </summary>
System::String^ PluginInformation::Name::get()
{
	return "Default Plugin";
}

/// <summary>
/// �v���O�C������
/// </summary>
System::String^ PluginInformation::OptionItemName::get()
{
	return "Default Plugin";
}