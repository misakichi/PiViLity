#include "pch.h"
#include "SusiePluginCom.h"
#pragma unmanaged
#include <atlcomcli.h>
#include "COM/SusiePluginCom_i.h"
#include "COM/SusiePluginCom_i.c"

// マネージドラッパーが保持する実装構造体。COMインターフェイスのポインタを保持します。
struct SusiePluginComImpl
{
	ISusie* com;
};
#pragma managed

using namespace System::Runtime::InteropServices;
using namespace PiVilityNative;

// COMオブジェクト生成時にローカルサーバとインプロセスサーバの両方を許可します。
constexpr auto CLCTX_SERVER_TYPE = CLSCTX_LOCAL_SERVER | CLSCTX_INPROC_SERVER;

/// <summary>
/// COMラッパーを作成し初期化します。内部でCoCreateInstanceを呼び出します。
/// Create and initialize the COM wrapper (calls CoCreateInstance internally).
/// 
/// </summary>
SusiePluginCom::SusiePluginCom()
{
	impl_ = new SusiePluginComImpl();
    // COMラッパーのインスタンスを作成します。失敗時はマネージド例外を投げます。
	auto ret = CoCreateInstance(CLSID_SusieWrapper, nullptr, CLCTX_SERVER_TYPE, IID_PPV_ARGS(&impl_->com));
	if (FAILED(ret))
		throw Marshal::GetExceptionForHR(ret);
}
/// <summary>
/// 明示的な解放（IDisposableパターン）。ネイティブリソースを解放します。
/// Dispose pattern: deterministic cleanup of native resources.
/// 
/// </summary>
SusiePluginCom::~SusiePluginCom()
{
    // 共通のクリーンアップのためファイナライザを呼び出します。
	SusiePluginCom::!SusiePluginCom();
}

/// <summary>
/// ファイナライザ。ガベージコレクタから呼ばれた場合にネイティブ実装を解放します。
/// Finalizer: releases native implementation when called by the GC.
/// 
/// </summary>
SusiePluginCom::!SusiePluginCom()
{
    // ネイティブ実装とそのCOM参照を解放します。
	delete impl_;
	impl_ = nullptr;
}

/// <summary>
/// 指定したパスでプラグインを読み込みます。BSTRへマシュリングしてCOMへ渡します。
/// Loads a plugin by the specified path. Marshals System::String to BSTR and calls COM.
/// 
/// </summary>
bool SusiePluginCom::Load(System::String^ path)
{
    // マネージドの文字列をBSTRへマシュリングしてCOMへ渡します。
	auto intPtr = Marshal::StringToBSTR(path);
	BSTR bstrPath = (BSTR)(intPtr.ToPointer());
	pin_ptr<BSTR> pin = &bstrPath;

	auto ret = impl_->com->Load(bstrPath);

	Marshal::FreeBSTR(intPtr);

	if (FAILED(ret))
		throw Marshal::GetExceptionForHR(ret);

	return SUCCEEDED(ret);

}

/// <summary>
/// プラグインから情報文字列を取得します。COMが返すBSTRをSystem::Stringへ変換します。
/// Retrieves an information string from the plugin. Converts BSTR returned by COM to System::String.
/// 
/// </summary>
bool SusiePluginCom::GetPluginInfo(int infono, System::String^% buf)
{
    // COMプラグインからBSTRを受け取り、System::Stringへ変換します。
	BSTR str;
	HRESULT hr;
	if(FAILED(hr=impl_->com->GetPluginInfo(infono, &str)))
		throw Marshal::GetExceptionForHR(hr);

	buf = gcnew System::String(str);
    SysFreeString(str); // COMから提供されたBSTRを解放します。

	return SUCCEEDED(hr);

}


/// <summary>
/// 指定したストリームがプラグインでサポートされる形式かを問い合わせます。
/// Marshals filename to BSTR and obtains IStream interface for the managed Stream.
/// 
/// </summary>
bool SusiePluginCom::IsSupportedStream(System::String^ filename, System::IO::Stream^ stream)
{
    // ファイル名をBSTRへマシュリングし、マネージドStreamからIStream COMインターフェイスを取得します。
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

/// <summary>
/// 生のバッファをSAFEARRAYへ変換してプラグインに問い合わせます。
/// Converts managed byte[] into a SAFEARRAY (VT_UI1), copies data and calls COM.
/// 
/// </summary>
bool SusiePluginCom::IsSupportedBuffer(System::String^ filename, array<byte>^ buffer)
{
    // マネージドのbyte[]をSAFEARRAY(VT_UI1)へ変換します。
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
    // マネージドバッファの内容をSAFEARRAYのメモリへコピーします。
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

/// <summary>
/// ファイルからネイティブのPictureInfoを取得し、マネージドのSPIPictureInfoへマッピングします。
/// Retrieves native PictureInfo from COM and maps it to managed SPIPictureInfo.
/// 
/// </summary>
bool SusiePluginCom::GetPictureInfoFile(System::String^ filename, SPIPictureInfo^% info)
{
    // COMからネイティブのPictureInfo構造を取得し、マネージドのSPIPictureInfoへマッピングします。
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
        // 任意のBSTR情報をSystem::Stringへ変換し、BSTRを解放します。
		info->info = gcnew System::String(nativeInfo.info);
		SysFreeString(nativeInfo.info);
	}

	return true;
}

static bool s_GetPictureCommon(const CComPtr<ISharedMemory>& infoMem, const CComPtr<ISharedMemory>& bmpMem, System::IO::MemoryMappedFiles::MemoryMappedFile^% info, System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp)
{
    // ISharedMemoryから名前を取得して、対応するメモリマップドファイルを開くヘルパーです。
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
            // 名前で既存のメモリマップドファイルを開こうとします。存在しない場合はマネージドAPIが例外を投げます。
			info = MemoryMappedFile::OpenExisting(infoName);
			bmp = MemoryMappedFile::OpenExisting(bmpName);
		}
		catch (System::Exception^ e)
		{
            // 後で再スローするために例外を保持します。
			catchedEx = e;
		}


	} while (0);

	if (FAILED(hr) || catchedEx != nullptr)
	{
        // 失敗時に部分的に作成されたマネージドオブジェクトをクリーンアップします。
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
/// <summary>
/// ファイルから画像データを取得し、共有メモリの名前を元にMemoryMappedFileを開きます。
/// Calls COM to get shared memory handles and opens them via s_GetPictureCommon.
/// 
/// </summary>
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

/// <summary>
/// プレビュー画像を取得し、共有メモリを開きます。内部でCOMのGetPreviewFileを呼び出します。
/// Retrieves a preview image via COM and opens corresponding MemoryMappedFiles.
/// 
/// </summary>
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


/// <summary>
/// 画像取得処理の完了をプラグインに通知します。
/// Signals to the plugin that the caller has finished retrieving picture data.
/// 
/// </summary>
bool SusiePluginCom::FinishGetPicture(void)
{
    // COMメソッドへそのまま転送します。
	return SUCCEEDED(impl_->com->FinishGetPicture());
}


