#define NOMINMAX
#pragma unmanaged
#include <algorithm>
#include <wincodec.h>
#include <atlcomcli.h>
#pragma comment(lib, "windowscodecs.lib")
#pragma comment(lib, "ole32.lib")
#pragma comment(lib, "gdi32.lib")
#pragma comment(lib, "User32.lib")
#pragma comment(lib, "OleAut32.lib")

#pragma managed

#include "ImageReaderTGA.h"
#include <vcclr.h>
//#include <filesystem>
//#include <fstream>
#include "libtga/tga.h"
#include <string>

using namespace BasicImagePluginCLI;
using namespace System;
using namespace PiViLityPlugin::Difinition;

namespace BasicImagePluginCLI
{
	struct TgaImage : tga_image
	{
	};
}

ImageReaderTGA::ImageReaderTGA()
{
}
ImageReaderTGA::~ImageReaderTGA()
{
	ReleaseTga();
}
ImageReaderTGA::!ImageReaderTGA()
{
	ReleaseTga();
}

void ImageReaderTGA::ReleaseTga()
{
	if (tga_)
	{
		free_tga(tga_);
		delete tga_;
		tga_ = nullptr;
	}

}

#include <memory>
/// <summary>
/// 当リーダークラスがサポートする画像ファイルの拡張子リストを取得します。
/// </summary>
/// <returns></returns>
Collections::Generic::IEnumerable<String^>^ ImageReaderTGA::GetSupportedExtensions()
{
	return gcnew array<String^> {"tga"};
}

/// <summary>
/// このプラグインが指定したファイルをサポートするかどうかを返します。
/// </summary>
/// <returns></returns>
bool ImageReaderTGA::IsSupported()
{
	if (filePath_ == nullptr || filePath_->Length == 0 || System::IO::File::Exists(filePath_)==false)
	{
		return false;
	}

	ReleaseTga();
	tga_ = new TgaImage();

	pin_ptr<const wchar_t> path = PtrToStringChars(filePath_);
	if (wload_tga(path, tga_) == false)
	{
		delete tga_;
		tga_ = nullptr;
		return nullptr;
	}
	
	return true;
}

/// <summary>
/// ファイルのパスを設定し、サポートされているかどうかを返します。
/// </summary>
/// <param name="filePath"></param>
/// <returns></returns>
bool ImageReaderTGA::SetFilePath(String^ filePath)
{
	filePath_ = filePath;

	return IsSupported();
}

/// <summary>
/// 画像イメージを取得します。
/// </summary>
/// <returns></returns>
System::Drawing::Image^ ImageReaderTGA::GetImage()
{
	if (tga_ == nullptr)
		return nullptr;

	System::Drawing::Bitmap^ bmp;
#pragma warning(suppress : 4642) 
	bmp = gcnew System::Drawing::Bitmap(tga_->width, tga_->height, tga_->channels==4 ? System::Drawing::Imaging::PixelFormat::Format32bppArgb : System::Drawing::Imaging::PixelFormat::Format24bppRgb);
#pragma warning(default : 4642) 
	auto mem = bmp->LockBits(System::Drawing::Rectangle(0, 0, tga_->width, tga_->height), System::Drawing::Imaging::ImageLockMode::WriteOnly, bmp->PixelFormat);
	auto pBits = (BYTE*)mem->Scan0.ToPointer();
	auto src = tga_->data;
	auto srcBw = tga_->width * tga_->channels;
	for (unsigned y = 0; y < tga_->height; y++)
	{
		memcpy(pBits, src, srcBw);
		pBits += mem->Stride;
		src += srcBw;
	}
	bmp->UnlockBits(mem);

	return bmp;
}

/// <summary>
/// 画像のイメージサイズを取得します。
/// </summary>
/// <returns></returns>
System::Drawing::Size ImageReaderTGA::GetImageSize()
{
	if(tga_==nullptr)
		return System::Drawing::Size(0, 0);
	return System::Drawing::Size(tga_->width, tga_->height);
}

#include "Plugin.h"

List<PiViLityPlugin::Difinition::Property^>^ ImageReaderTGA::ReadProperties()
{
	auto propertoes = gcnew List<PiViLityPlugin::Difinition::Property^>();
	auto size = GetImageSize();
	auto propWidth = gcnew Property();
	propWidth->Group = PropertyGroup::Image;
	propWidth->Name = ResourceHolder::GetString("PropertyName.Width");// "Width";
	propWidth->Value = size.Width.ToString();
	propertoes->Add(propWidth);

	auto propHeight = gcnew Property();
	propHeight->Group = PropertyGroup::Image;
	propHeight->Name = ResourceHolder::GetString("PropertyName.Height");// "Height";
	propHeight->Value = size.Height.ToString();
	propertoes->Add(propHeight);

	auto propFormat = gcnew Property();
	propFormat->Group = PropertyGroup::Image;
	propFormat->Name = ResourceHolder::GetString("PropertyName.Format");// "Format";
	propFormat->Value = "Unknown";
	propertoes->Add(propFormat);


	return propertoes;
}
