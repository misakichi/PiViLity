#pragma once
using namespace PiViLityPlugin::Difinition;
using namespace System;
using namespace System::Collections::Generic;


namespace BasicImagePluginCLI
{
	struct TgaImage;
	public ref class ImageReaderTGA : public ImageReaderBase, public IPropertyReader
	{
	private:
		String^ filePath_ = "";

	public:
		ImageReaderTGA();
		~ImageReaderTGA() override;
		!ImageReaderTGA();


		IEnumerable<String^>^ GetSupportedExtensions() override;

		/// <summary>
		/// このプラグインが指定したファイルをサポートするかどうかを返します。
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		bool IsSupported() override;

		bool SetFilePath(String^ filePath) override;

		Drawing::Image^ GetImage() override;

		System::Drawing::Size GetImageSize() override;

		virtual List<PiViLityPlugin::Difinition::Property^>^ ReadProperties();


	private:
		void ReleaseTga();
		TgaImage* tga_ = nullptr;

	}; // class ImageReaderJpeg

} // namespace BasicImagePluginCLI