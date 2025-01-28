#pragma once
using namespace PiViLityCore::Plugin;
using namespace System;

namespace BasicImagePluginCLI
{
    /// <summary>
    /// プラグイン情報
    /// </summary>
    public ref class PluginInformation : public IModuleInformation
    {
	public:
		/// <summary>
		/// プラグインの説明
		/// </summary>
		virtual property String^ Description
		{
			String^ get();
		}
		/// <summary>
		/// プラグイン名称
		/// </summary>
		virtual property String^ Name
		{
			String^ get();
		}
    };
} // namespace BasicImagePluginCLI
