#pragma once
using namespace PiViLityCore::Plugin;
using namespace System;

namespace BasicImagePluginCLI
{
    /// <summary>
    /// プラグイン情報
    /// </summary>
    public ref class Plugin : public IModuleInformation
    {
	public:
		/// <summary>
		/// プラグインの説明
		/// </summary>
		virtual property String^ Description
		{
			virtual String^ get() override
			{ 
				return "Default implementation plugin."; 
			}
		}
		/// <summary>
		/// プラグイン名称
		/// </summary>
		virtual property String^ Name
		{
			String^ get() override
			{
				return "";
			}
		}
    };
} // namespace BasicImagePluginCLI
