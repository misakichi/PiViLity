#pragma once
using namespace PiViLityPlugin::Difinition;
using namespace System;
using namespace System::Collections::Generic;

namespace BasicImagePluginCLI
{
	class ImageReaderWICNativeImpl;

	public ref class ImageReaderWIC : public ImageReaderBase, public IPropertyReader
	{
	private:
		String^ filePath_ = "";

	public:
		ImageReaderWIC();
		~ImageReaderWIC() override;
		!ImageReaderWIC();


		List<String^>^ GetSupportedExtensions() override;

		/// <summary>
		/// このプラグインが指定したファイルをサポートするかどうかを返します。
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		bool IsSupported() override;

		bool SetFilePath(String^ filePath) override;

		Drawing::Image^ GetImage() override;

		Drawing::Image^ GetThumbnailImage(Drawing::Size size) override;

		System::Drawing::Size GetImageSize() override;

		ImageReaderWICNativeImpl* nativeImpl_ = nullptr;

		virtual List<PiViLityPlugin::Difinition::Property^>^ ReadProperties();

	}; // class ImageReaderJpeg

} // namespace BasicImagePluginCLI