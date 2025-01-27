#include "ImageReaderJpeg.h"

using namespace BasicImagePluginCLI;
using namespace System;
using namespace DirectXTexNet;

Collections::Generic::List<String^>^ ImageReaderJpeg::GetSupportExtensions()
{
	auto extensions = gcnew System::Collections::Generic::List<System::String^>();
	extensions->Add(".jpg");
	extensions->Add(".jpeg");
	return extensions;
}

bool ImageReaderJpeg::IsSupport(String^ filePath)
{
	return true;
}

bool ImageReaderJpeg::SetFilePath(String^ filePath)
{
	return true;
}

System::Drawing::Image^ ImageReaderJpeg::GetImage()
{
	return nullptr;
}

System::Drawing::Image^ImageReaderJpeg::GetThumbnailImage(Drawing::Size size)
{
	return nullptr;
}

// Compare this snippet from BasicImagePluginCLI/ImageReaderJpeg.cpp: