#pragma once
using namespace PiViLityCore::Plugin;
using namespace System;
using namespace System::Collections::Generic;

namespace BasicImagePluginCLI
{
	public ref class ImageReaderJpeg : public IImageReader
	{
	public:
		virtual List<String^>^ GetSupportExtensions();

		virtual property ThumbnailQualities ThumbnailQuality
        {
            ThumbnailQualities get() override { return thumbnailQuality_; }
            void set(ThumbnailQualities value) override { thumbnailQuality_ = value; }
        }

		/// <summary>
		/// このプラグインが指定したファイルをサポートするかどうかを返します。
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		bool IsSupport(String^ filePath) override;

		bool SetFilePath(String^ filePath) override;

		Drawing::Image^ GetImage() override;

		Drawing::Image^ GetThumbnailImage(Drawing::Size size) override;

	private:
		ThumbnailQualities thumbnailQuality_;
	}; // class ImageReaderJpeg

} // namespace BasicImagePluginCLI