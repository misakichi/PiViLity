#pragma once
using namespace PiViLityCore::Plugin;
using namespace System;
using namespace System::Collections::Generic;

namespace BasicImagePluginCLI
{
	class ImageReaderWICNativeImpl;

	public ref class ImageReaderWIC : public PiViLityCore::Plugin::ImageReaderBase, public PiViLityCore::Plugin::IPropertyReader
	{
	private:
		String^ filePath_ = "";

	public:
		ImageReaderWIC();
		~ImageReaderWIC() override;
		!ImageReaderWIC();


		List<String^>^ GetSupportedExtensions() override;

		/// <summary>
		/// ���̃v���O�C�����w�肵���t�@�C�����T�|�[�g���邩�ǂ�����Ԃ��܂��B
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		bool IsSupported() override;

		bool SetFilePath(String^ filePath) override;

		Drawing::Image^ GetImage() override;

		Drawing::Image^ GetThumbnailImage(Drawing::Size size) override;

		System::Drawing::Size GetImageSize() override;

		ImageReaderWICNativeImpl* nativeImpl_ = nullptr;

		virtual List<PiViLityCore::Plugin::Property^>^ ReadProperties();

	}; // class ImageReaderJpeg

} // namespace BasicImagePluginCLI