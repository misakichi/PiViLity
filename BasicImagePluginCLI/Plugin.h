#pragma once
using namespace PiViLityCore::Plugin;
using namespace System;
using namespace System::Runtime::CompilerServices;

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

	private ref class ResourceHolder
	{
	public:
		static System::Globalization::CultureInfo^ culture;
		static System::Resources::ResourceManager^ resourceManager;

		static ResourceHolder()
		{
			using namespace System::Globalization;
			using namespace System::Resources;

			String^ uiCultureName = CultureInfo::CurrentUICulture->Name;

			culture = gcnew CultureInfo(uiCultureName);
			resourceManager = gcnew ResourceManager("BasicImagePluginCLI.CliResource", Reflection::Assembly::GetExecutingAssembly());

			//String^ msg = rm->GetString("WelcomeMessage", culture);
		}

		static String ^ GetString(String^ name)
		{
			return resourceManager->GetString(name, culture);
		}

	};

} // namespace BasicImagePluginCLI
