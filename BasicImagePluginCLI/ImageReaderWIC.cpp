#include "ImageReaderWIC.h"

using namespace BasicImagePluginCLI;
using namespace System;
using namespace DirectXTexNet;
#include <memory>

/// <summary>
/// 当リーダークラスがサポートする画像ファイルの拡張子リストを取得します。
/// </summary>
/// <returns></returns>
Collections::Generic::List<String^>^ ImageReaderWIC::GetSupportedExtensions()
{
	auto extensions = gcnew System::Collections::Generic::List<System::String^>();
	extensions->Add("jpg");
	extensions->Add("jpeg");
	extensions->Add("bmp");
	extensions->Add("png");

	return extensions;
}

/// <summary>
/// このプラグインが指定したファイルをサポートするかどうかを返します。
/// </summary>
/// <returns></returns>
bool ImageReaderWIC::IsSupported()
{
	if (filePath_ == nullptr || filePath_->Length == 0 || System::IO::File::Exists(filePath_)==false)
	{
		metadata_ = nullptr;
		return false;
	}
	auto helper = DirectXTexNet::TexHelper::Instance;
	auto meta = helper->GetMetadataFromWICFile(filePath_, DirectXTexNet::WIC_FLAGS::NONE);
	metadata_ = meta;
	return (meta != nullptr);
}

/// <summary>
/// ファイルのパスを設定し、サポートされているかどうかを返します。
/// </summary>
/// <param name="filePath"></param>
/// <returns></returns>
bool ImageReaderWIC::SetFilePath(String^ filePath)
{
	filePath_ = filePath;
	return IsSupported();
}

/// <summary>
/// 画像イメージを取得します。
/// </summary>
/// <returns></returns>
System::Drawing::Image^ ImageReaderWIC::GetImage()
{
	if (metadata_ == nullptr || System::IO::File::Exists(filePath_) == false)
	{
		return nullptr;
	}

	auto helper = DirectXTexNet::TexHelper::Instance;
	auto scratchImage = helper->LoadFromWICFile(filePath_, DirectXTexNet::WIC_FLAGS::NONE);

	if (scratchImage == nullptr)
	{
		return nullptr;
	}

	auto imageData = scratchImage->GetImage(0, 0, 0);
	if (imageData == nullptr)
	{
		return nullptr;
	}

	// Create a Bitmap from the image data
	auto bitmap = gcnew System::Drawing::Bitmap(imageData->Width, imageData->Height, (int)imageData->RowPitch, System::Drawing::Imaging::PixelFormat::Format32bppArgb, IntPtr(imageData->Pixels));

	return bitmap;
}

/// <summary>
/// 画像のサムネイルイメージを取得、または作成します。
/// </summary>
/// <param name="size"></param>
/// <returns></returns>
System::Drawing::Image^ ImageReaderWIC::GetThumbnailImage(Drawing::Size size)
{
    if (metadata_ == nullptr || System::IO::File::Exists(filePath_) == false)
    {
        return nullptr;
    }

    auto helper = DirectXTexNet::TexHelper::Instance;
    DirectXTexNet::ScratchImage^ thumbnailImage = nullptr;

    //if (ThumbnailQuality == ThumbnailQualities::UseThumbnail)
    //{
	// 後で実装
    //}

    if (thumbnailImage == nullptr)
    {
        auto scratchImage = helper->LoadFromWICFile(filePath_, DirectXTexNet::WIC_FLAGS::NONE);
        if (scratchImage == nullptr)
        {
            return nullptr;
        }
        thumbnailImage = scratchImage;
    }
    auto imageData = thumbnailImage ? thumbnailImage->GetImage(0, 0, 0) : nullptr;
    if (imageData == nullptr)
    {
        return nullptr;
    }
    

    DirectXTexNet::TexMetadata^ metadata = thumbnailImage->GetMetadata();
	void* pixels = new char[size.Width * size.Height * 4];
	memset(pixels, 0, size.Width * size.Height * 4);
    auto finalImage = gcnew Image(size.Width, size.Height, DirectXTexNet::DXGI_FORMAT::B8G8R8A8_UNORM, size.Width * 4, size.Width * size.Height * 4, IntPtr(pixels), nullptr);

    // ここで finalImage のピクセルデータが正しく設定されているか確認します
    if (finalImage->Pixels == IntPtr(nullptr) || finalImage->Width == 0 || finalImage->Height == 0)
    {
        delete[] pixels;
        return nullptr;
    }
    if (ThumbnailType == ThumbnailTypes::KeepAspectRatio)
    {
        float aspectRatio = static_cast<float>(metadata->Width) / metadata->Height;
        int newWidth = size.Width;
        int newHeight = int(size.Width / aspectRatio);

        if (newHeight > size.Height)
        {
            newHeight = size.Height;
            newWidth = int(size.Height * aspectRatio);
        }
        auto resizedImage = thumbnailImage->Resize(0, newWidth, newHeight, DirectXTexNet::TEX_FILTER_FLAGS::DEFAULT);
        auto resizedData = resizedImage->GetImage(0, 0, 0);

        int x = (size.Width - newWidth) / 2;
        int y = (size.Height - newHeight) / 2;

		helper->CopyRectangle(resizedData, 0, 0, newWidth, newHeight, finalImage, DirectXTexNet::TEX_FILTER_FLAGS::DEFAULT, x, y);

        delete resizedImage;

    }
    else if (ThumbnailType == ThumbnailTypes::Centering)
    {
        auto dstRatio = size.Height / float(size.Width);
		auto srcRatio = metadata->Height / float(metadata->Width);
        int newWidth = size.Width;
        int newHeight = size.Height;
        if (dstRatio > srcRatio)
        {
			newWidth = int(size.Height / srcRatio);
		}
		else
		{
			newHeight = int(size.Width * srcRatio);
		}
        auto resizedImage = thumbnailImage->Resize(0, newWidth, newHeight, DirectXTexNet::TEX_FILTER_FLAGS::DEFAULT);
        auto resizedData = resizedImage->GetImage(0, 0, 0);

        int x = (newWidth - size.Width) / 2;
        int y = (newHeight - size.Height) / 2;

        helper->CopyRectangle(resizedData, x, y, size.Width, size.Height, finalImage, DirectXTexNet::TEX_FILTER_FLAGS::DEFAULT, 0, 0);

        delete resizedImage;

    }
    auto result = gcnew System::Drawing::Bitmap(finalImage->Width, finalImage->Height, System::Drawing::Imaging::PixelFormat::Format32bppArgb);

    System::Drawing::Imaging::BitmapData^ bitmapData = result->LockBits(System::Drawing::Rectangle(0, 0, result->Width, result->Height), System::Drawing::Imaging::ImageLockMode::WriteOnly, result->PixelFormat);
    memcpy(bitmapData->Scan0.ToPointer(), pixels, size.Width * size.Height * 4);
    result->UnlockBits(bitmapData);


    delete[]pixels;
    delete thumbnailImage;
    delete finalImage;

    return result;
}

/// <summary>
/// 画像のイメージサイズを取得します。
/// </summary>
/// <returns></returns>
System::Drawing::Size ImageReaderWIC::GetImageSize()
{
	if (metadata_ == nullptr || System::IO::File::Exists(filePath_) == false)
	{
		return System::Drawing::Size(0, 0);
	}
	return System::Drawing::Size(metadata_->Width, metadata_->Height);
}

