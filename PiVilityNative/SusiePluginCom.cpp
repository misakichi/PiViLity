#include "pch.h"
#include "SusiePluginCom.h"
#pragma unmanaged
#include <atlcomcli.h>
#include "COM/SusiePluginCom_i.h"
#include "COM/SusiePluginCom_i.c"
struct SusiePluginComImpl
{
	ISusie* com;
};
#pragma managed

using namespace System::Runtime::InteropServices;
using namespace PiVilityNative;

constexpr auto CLCTX_SERVER_TYPE = CLSCTX_LOCAL_SERVER | CLSCTX_INPROC_SERVER;

SusiePluginCom::SusiePluginCom()
{
	impl_ = new SusiePluginComImpl();
	auto ret = CoCreateInstance(CLSID_SusieWrapper, nullptr, CLCTX_SERVER_TYPE, IID_PPV_ARGS(&impl_->com));
	if (FAILED(ret))
		throw Marshal::GetExceptionForHR(ret);
}
SusiePluginCom::~SusiePluginCom()
{
	SusiePluginCom::!SusiePluginCom();
}

SusiePluginCom::!SusiePluginCom()
{
	delete impl_;
	impl_ = nullptr;
}

bool SusiePluginCom::Load(System::String^ path)
{
	auto intPtr = Marshal::StringToBSTR(path);
	BSTR bstrPath = (BSTR)(intPtr.ToPointer());
	pin_ptr<BSTR> pin = &bstrPath;

	auto ret = impl_->com->Load(bstrPath);

	Marshal::FreeBSTR(intPtr);

	if (FAILED(ret))
		throw Marshal::GetExceptionForHR(ret);

	return SUCCEEDED(ret);

}

bool SusiePluginCom::GetPluginInfo(int infono, System::String^% buf)
{
	BSTR str;
	HRESULT hr;
	if(FAILED(hr=impl_->com->GetPluginInfo(infono, &str)))
		throw Marshal::GetExceptionForHR(hr);

	buf = gcnew System::String(str);
	SysFreeString(str);

	return SUCCEEDED(hr);

}


bool SusiePluginCom::IsSupportedStream(System::String^ filename, System::IO::Stream^ stream)
{
	auto intPtr = Marshal::StringToBSTR(filename);
	BSTR bstrPath = (BSTR)(intPtr.ToPointer());
	pin_ptr<BSTR> pin = &bstrPath;


	ATL::CComPtr<IStream> comStream = (IStream*)Marshal::GetComInterfaceForObject(stream, IStream::typeid).ToPointer();
	HRESULT hr;
	hr = impl_->com->IsSupportedStream(bstrPath, comStream);
	Marshal::FreeBSTR(intPtr);
	if (FAILED(hr))
		throw Marshal::GetExceptionForHR(hr);

	return SUCCEEDED(hr);

}

bool SusiePluginCom::IsSupportedBuffer(System::String^ filename, array<byte>^ buffer)
{
	LONG len = buffer->Length;
	SAFEARRAY* safeArray = SafeArrayCreateVector(VT_UI1, 0, len);
	if (safeArray == nullptr)
		throw gcnew System::OutOfMemoryException();

	void* pDest = nullptr;
	SafeArrayAccessData(safeArray, &pDest);
	if (pDest == nullptr)
	{
		SafeArrayDestroy(safeArray);
		throw gcnew System::OutOfMemoryException();
	}
	pin_ptr<BYTE> pSrc = &buffer[0];
	memcpy(pDest, pSrc, len);
	SafeArrayUnaccessData(safeArray);

	auto intPtr = Marshal::StringToBSTR(filename);
	BSTR bstrPath = (BSTR)(intPtr.ToPointer());
	pin_ptr<BSTR> pin = &bstrPath;

	HRESULT hr;
	hr = impl_->com->IsSupportedBuffer(bstrPath, safeArray);
	SafeArrayDestroy(safeArray);
	Marshal::FreeBSTR(intPtr);

	if(FAILED(hr))
		throw Marshal::GetExceptionForHR(hr);

	return SUCCEEDED(hr);

}

bool SusiePluginCom::GetPictureInfoFile(System::String^ filename, SPIPictureInfo^% info)
{
	info = nullptr;
	auto intPtr = Marshal::StringToBSTR(filename);
	BSTR bstrPath = (BSTR)(intPtr.ToPointer());
	pin_ptr<BSTR> pin = &bstrPath;

	HRESULT hr;
	PictureInfo nativeInfo;
	hr = impl_->com->GetPictureInfoFile(bstrPath, &nativeInfo);
	if (FAILED(hr))
		throw Marshal::GetExceptionForHR(hr);

	info = gcnew SPIPictureInfo();
	info->left = nativeInfo.left;
	info->top = nativeInfo.top;
	info->width = nativeInfo.width;
	info->height = nativeInfo.height;
	info->x_density = nativeInfo.x_density;
	info->y_density = nativeInfo.y_density;
	info->colorDepth = nativeInfo.colorDepth;
	if (nativeInfo.info)
	{
		info->info = gcnew System::String(nativeInfo.info);
		SysFreeString(nativeInfo.info);
	}

	return true;
}

static bool s_GetPictureCommon(const CComPtr<ISharedMemory>& infoMem, const CComPtr<ISharedMemory>& bmpMem, System::IO::MemoryMappedFiles::MemoryMappedFile^% info, System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp)
{
	System::Exception^ catchedEx = nullptr;
	HRESULT hr = S_OK;
	do
	{
		BSTR infoNameBstr = NULL;
		BSTR bmpNameBstr = NULL;
		if (FAILED(hr = infoMem->GetPathName(&infoNameBstr)))
			break;
		if (FAILED(hr = bmpMem->GetPathName(&bmpNameBstr)))
		{
			SysFreeString(infoNameBstr);
			break;
		}

		auto infoName = gcnew System::String(infoNameBstr);
		auto bmpName = gcnew System::String(bmpNameBstr);
		SysFreeString(infoNameBstr);
		SysFreeString(bmpNameBstr);

		using namespace System::IO::MemoryMappedFiles;
		try {
			info = MemoryMappedFile::OpenExisting(infoName);
			bmp = MemoryMappedFile::OpenExisting(bmpName);
		}
		catch (System::Exception^ e)
		{
			catchedEx = e;
		}


	} while (0);

	if (FAILED(hr) || catchedEx != nullptr)
	{
		if (bmp)
			delete bmp;
		if (info)
			delete info;
		bmp = info = nullptr;

		if (catchedEx)
			throw catchedEx;
		throw Marshal::GetExceptionForHR(hr);
	}

	return true;
}
bool SusiePluginCom::GetPictureFile(System::String^ filename, System::IO::MemoryMappedFiles::MemoryMappedFile^% info, System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp)
{
	auto intPtr = Marshal::StringToBSTR(filename);
	BSTR bstrPath = (BSTR)(intPtr.ToPointer());
	pin_ptr<BSTR> pin = &bstrPath;
	CComPtr<ISharedMemory> infoMem;
	CComPtr<ISharedMemory> bmpMem;
	HRESULT hr = impl_->com->GetPictureFile(bstrPath, &infoMem, &bmpMem);
	Marshal::FreeBSTR(intPtr);
	if(FAILED(hr))
		throw Marshal::GetExceptionForHR(hr);

	return s_GetPictureCommon(infoMem, bmpMem, info, bmp);
}

bool SusiePluginCom::GetPreviewFile(System::String^ filename, System::IO::MemoryMappedFiles::MemoryMappedFile^% info, System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp)
{
	auto intPtr = Marshal::StringToBSTR(filename);
	BSTR bstrPath = (BSTR)(intPtr.ToPointer());
	pin_ptr<BSTR> pin = &bstrPath;
	CComPtr<ISharedMemory> infoMem;
	CComPtr<ISharedMemory> bmpMem;
	HRESULT hr = impl_->com->GetPreviewFile(bstrPath, &infoMem, &bmpMem);
	Marshal::FreeBSTR(intPtr);
	if (FAILED(hr))
		throw Marshal::GetExceptionForHR(hr);

	return s_GetPictureCommon(infoMem, bmpMem, info, bmp);
}

bool SusiePluginCom::FinishGetPicture(void)
{
	return SUCCEEDED(impl_->com->FinishGetPicture());
}


