#pragma once
using namespace PiViLityCore::Plugin;
using namespace System;

namespace BasicImagePluginCLI
{
    /// <summary>
    /// �v���O�C�����
    /// </summary>
    public ref class Plugin : public IModuleInformation
    {
	public:
		/// <summary>
		/// �v���O�C���̐���
		/// </summary>
		virtual property String^ Description
		{
			virtual String^ get() override
			{ 
				return "Default implementation plugin."; 
			}
		}
		/// <summary>
		/// �v���O�C������
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
