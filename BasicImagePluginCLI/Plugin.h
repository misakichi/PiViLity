#pragma once
using namespace PiViLityCore::Plugin;
using namespace System;

namespace BasicImagePluginCLI
{
    /// <summary>
    /// �v���O�C�����
    /// </summary>
    public ref class PluginInformation : public IModuleInformation
    {
	public:
		/// <summary>
		/// �v���O�C���̐���
		/// </summary>
		virtual property String^ Description
		{
			String^ get();
		}
		/// <summary>
		/// �v���O�C������
		/// </summary>
		virtual property String^ Name
		{
			String^ get();
		}
		/// <summary>
		/// �v���O�C������
		/// </summary>
		virtual property String^ OptionItemName
		{
			String^ get();
		}
		/// <summary>
		/// �v���O�C������
		/// </summary>
		virtual property System::Resources::ResourceManager^ ResourceManager
		{
			System::Resources::ResourceManager^ get() { return nullptr; }
		}
	};
} // namespace BasicImagePluginCLI
