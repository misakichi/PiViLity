#include "pch.h"

#include "ShellAPIHelper.h"
#include <shlobj.h>
#include <commoncontrols.h>
#include <vcclr.h>
#include <string>
#include <vector>
#include <atlbase.h>
using namespace System;

static wchar_t* GetKnownFolderPath(const GUID& folderId)
{
	wchar_t* path = nullptr;
	HRESULT hr = SHGetKnownFolderPath(folderId, 0, nullptr, reinterpret_cast<LPWSTR*>(&path));
	if (SUCCEEDED(hr))
	{
		return path;
	}
	return nullptr;
}

using namespace PiVilityNative;


String^ ShellAPI::GetMyCompute()
{
	GUID folderId = { 0x374DE290, 0x123F, 0x4565, { 0xBC, 0x9C, 0x2D, 0x7B, 0xC3, 0xB3, 0xD6, 0xAC } }; // This PC の GUID
	if (auto path = GetKnownFolderPath(folderId))
	{
		auto ret = gcnew String(path);
		CoTaskMemFree(path);
		return ret;
	}
	return "";
}

#if 0
IntPtr FileInfo::GetLargeImageList()
{
	HIMAGELIST hImageList;
	if (SHGetImageList(SHIL_SMALL, IID_IImageList, (PVOID*)&hImageList) == S_OK)
	{
		return IntPtr(hImageList);
	}
	else
	{
		return IntPtr(0);
	}
}

IntPtr FileInfo::GetSmallImageList()
{
	HIMAGELIST hImageList;
	if (SHGetImageList(SHIL_LARGE, IID_IImageList, (PVOID*)&hImageList) == S_OK)
	{
		return IntPtr(hImageList);
	}
	else
	{
		return IntPtr(0);
	}
}
#endif

/// <summary>
/// 静的コンストラクタから呼ばれる初期化処理。
/// システムイメージリストのハンドルを取得しておく。
/// </summary>
void FileInfo::StaticConstruct()
{
	HIMAGELIST hImageList;
	SHGetImageList(SHIL_SYSSMALL, IID_IImageList, (PVOID*)&hImageList);
	systemSmallImageList_ = hImageList;
	SHGetImageList(SHIL_EXTRALARGE, IID_IImageList, (PVOID*)&hImageList);
	systemLargeImageList_ = hImageList;
	SHGetImageList(SHIL_JUMBO, IID_IImageList, (PVOID*)&hImageList);
	systemJumboImageList_ = hImageList;
}


/// <summary>
/// ファイルまたは拡張子のファイル種別名を取得します
/// </summary>
/// <param name="pathOrExtension"></param>
/// <returns></returns>
System::String^ FileInfo::GetFileTypeName(String^ pathOrExtension)
{
	pin_ptr<const wchar_t> wcharPath = PtrToStringChars(pathOrExtension);
	SHFILEINFO info = {};
	SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_TYPENAME);
	return gcnew System::String(info.szTypeName);
}
/// <summary>
/// ファイルパスからファイルのアイコンインデックスを取得します。
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
int FileInfo::GetFileIconIndex(String^ path)
{
	pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
	SHFILEINFO info = {};
	SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_SYSICONINDEX);
	return info.iIcon;
}

/// <summary>
/// システムイメージリストのインデックスからアイコン（大）を取得します。
/// </summary>
/// <param name="index"></param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileLargeIconFromIndex(int index)
{
	auto icon = ImageList_GetIcon(systemLargeImageList_, index, ILD_TRANSPARENT);
	if (icon == nullptr)
	{
		auto  err = GetLastError();
		Diagnostics::Debug::Print("ImageList_GetIcon: " + err);
		return nullptr;
	}
	return System::Drawing::Icon::FromHandle(IntPtr(icon));
}

/// <summary>
/// システムイメージリストのインデックスからアイコン（小）を取得します。
/// </summary>
/// <param name="index"></param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileSmallIconFromIndex(int index)
{
	auto icon = ImageList_GetIcon(systemSmallImageList_, index, ILD_TRANSPARENT);
	if (icon == nullptr)
	{
		auto  err = GetLastError();
		Diagnostics::Debug::Print("ImageList_GetIcon: " + err);
		return nullptr;
	}
	return System::Drawing::Icon::FromHandle(IntPtr(icon));
}

/// <summary>
/// システムイメージリストのインデックスからアイコン（Jumbo）を取得します。
/// </summary>
/// <param name="index"></param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileJumboIconFromIndex(int index)
{
	auto icon = ImageList_GetIcon(systemJumboImageList_, index, ILD_TRANSPARENT);
	if (icon == nullptr)
	{
		auto  err = GetLastError();
		Diagnostics::Debug::Print("ImageList_GetIcon: " + err);
		return nullptr;
	}
	return System::Drawing::Icon::FromHandle(IntPtr(icon));
}

/// <summary>
/// ファイルパスからファイルのアイコン（大）を取得します。
/// <param name="path">ファイルパス</param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileLargeIcon(String^ path)
{
	pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
	SHFILEINFO info = {};
	SHGetFileInfo(wcharPath,0,&info, sizeof(info), SHGFI_ICON | SHGFI_LARGEICON);

	return System::Drawing::Icon::FromHandle(IntPtr(info.hIcon));
}

/// <summary>
/// ファイルパスからファイルのアイコン（小）を取得します。
/// </summary>
/// <param name="path">ファイルパス</param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileSmallIcon(String^ path)
{
	pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
	SHFILEINFO info = {};
	SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_ICON | SHGFI_SMALLICON);

	return System::Drawing::Icon::FromHandle(IntPtr(info.hIcon));
}

/// <summary>
/// ファイルパスからファイルのアイコン（Jumbo）を取得します。
/// </summary>
/// <param name="path">ファイルパス</param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileJumboIcon(String^ path)
{
	pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
	SHFILEINFO info = {};
	SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_ICON |  SHGFI_SYSICONINDEX);

	auto icon = ImageList_GetIcon(systemJumboImageList_, info.iIcon, ILD_TRANSPARENT);
	return System::Drawing::Icon::FromHandle(IntPtr(icon));
}


void ShellAPI::ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd)
{
	ShowShellContextMenu(paths, hwnd, INT_MIN, INT_MIN);
}

/// <summary>
/// OK
/// </summary>
/// <param name="paths"></param>
/// <returns></returns>
CComPtr<IShellItemArray> filepathsToShellItemArray(std::vector<std::wstring>& paths)
{
	CComPtr<IShellItemArray> shellItemArray;
	std::vector<LPITEMIDLIST> pidls;
	for (const auto& path : paths)
	{
		LPITEMIDLIST pidl = nullptr;
		auto hr = SHParseDisplayName(path.c_str(), nullptr, &pidl, 0, nullptr);
		if (SUCCEEDED(hr) && pidl)
		{
			pidls.push_back(pidl);
		}
	}
	if (pidls.empty())
		return nullptr;
	auto list = (LPCITEMIDLIST*)pidls.data();
	SHCreateShellItemArrayFromIDLists(pidls.size(), list, &shellItemArray);
	for (auto pidl : pidls)
	{
		CoTaskMemFree(pidl);
	}
	return shellItemArray;
}

/// <summary>
/// パス（複数）のシェルアイテムリストを作成する
/// </summary>
/// <param name="paths">ファイルパス(s)</param>
/// <returns>IShellItemArray</returns>
CComPtr<IShellItemArray> filepathsToShellItemArray(array<String^>^ paths)
{
#if 1
	//OK 一度IDリストの配列に変換してからSHCreateShellItemArrayFromIDListsでIShellItemArrayを作成する
	CComPtr<IShellItemArray> shellItemArray;
	std::vector<LPITEMIDLIST> pidls;
	for each(auto path in paths)
	{
		LPITEMIDLIST pidl = nullptr;
		pin_ptr<const wchar_t> wchPath = PtrToStringChars(path);
		auto hr = SHParseDisplayName(wchPath, nullptr, &pidl, 0, nullptr);
		if (SUCCEEDED(hr) && pidl)
		{
			pidls.push_back(pidl);
		}
	}
	if (pidls.empty())
		return nullptr;
	auto list = (LPCITEMIDLIST*)pidls.data();
	SHCreateShellItemArrayFromIDLists(pidls.size(), list, &shellItemArray);
	for (auto pidl : pidls)
	{
		CoTaskMemFree(pidl);
	}
	return shellItemArray;


#else
	//NG これだとエラーになる
	//IObjectCollectionを作成して、そこにIShellItemを追加していき、最終的にそこからIShellItemArrayを取得する
	CComPtr<IObjectArray> objArray;
	if (FAILED(CoCreateInstance(CLSID_EnumerableObjectCollection, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&objArray))))
	{
		return nullptr;
	}
	CComQIPtr<IObjectCollection> collection(objArray);
	if (!collection)
	{
		return nullptr;
	}

	for each(auto path in paths)
	{
		pin_ptr<const wchar_t> wchPath = PtrToStringChars(path);
		CComPtr<IShellItem> item;
		if (SUCCEEDED(SHCreateItemFromParsingName(wchPath, nullptr, IID_PPV_ARGS (&item))))
		{
			collection->AddObject(item);
		}
	}

	return CComQIPtr<IShellItemArray>(collection);
#endif
}


/// <summary>
/// シェルコンテキストメニューを表示します。
/// </summary>
/// <param name="paths"></param>
/// <param name="hwnd"></param>
/// <param name="x"></param>
/// <param name="y"></param>
void ShellAPI::ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd, int x, int y)
{
	auto shellItems = filepathsToShellItemArray(paths);

	if (shellItems)
	{
		CComPtr<IContextMenu> contextMenu;
		HRESULT hr = shellItems->BindToHandler(nullptr, BHID_SFUIObject, IID_PPV_ARGS(&contextMenu));
		if (SUCCEEDED(hr))
		{
			HMENU hMenu = CreatePopupMenu();
			if (hMenu)
			{
				hr = contextMenu->QueryContextMenu(hMenu, 0, 1, 0x7FFF, CMF_EXPLORE | CMF_EXTENDEDVERBS);
				if (SUCCEEDED(hr))
				{
					if (x == INT_MIN || y == INT_MIN)
					{
						POINT point;
						GetCursorPos(&point);
						x = point.x;
						y = point.y;
					}
					int cmd = TrackPopupMenu(hMenu, TPM_RETURNCMD | TPM_RIGHTBUTTON, x, y, 0, (HWND)hwnd.ToPointer(), nullptr);
					if (cmd > 0)
					{
						CMINVOKECOMMANDINFOEX info = { 0 };
						info.cbSize = sizeof(info);
						info.fMask = CMIC_MASK_UNICODE | CMIC_MASK_PTINVOKE;
						info.hwnd = (HWND)hwnd.ToPointer();
						info.lpVerb = MAKEINTRESOURCEA(cmd - 1);
						info.lpVerbW = MAKEINTRESOURCEW(cmd - 1);
						info.nShow = SW_SHOWNORMAL;
						info.ptInvoke.x = x;
						info.ptInvoke.y = y;
						contextMenu->InvokeCommand((LPCMINVOKECOMMANDINFO)&info);
					}
				}
				DestroyMenu(hMenu);
			}
		}
	}
}

static std::vector<wchar_t> pathListToDoubleNullTerminatedString(Collections::Generic::IEnumerable<String^>^ paths)
{
	std::vector<wchar_t> buffer;
	for each(auto path in paths)
	{
		pin_ptr<const wchar_t> wchPath = PtrToStringChars(path);
		const wchar_t* pSrc = wchPath;
		buffer.insert(buffer.end(), pSrc, pSrc + path->Length);
		buffer.push_back(L'\0');
	}
	buffer.push_back(L'\0');
	return buffer;
}
static std::vector<wchar_t> pathListToDoubleNullTerminatedString(String^ path)
{
	std::vector<wchar_t> buffer;
	pin_ptr<const wchar_t> wchPath = PtrToStringChars(path);
	const wchar_t* pSrc = wchPath;
	buffer.insert(buffer.end(), pSrc, pSrc + path->Length);
	buffer.push_back(L'\0');
	buffer.push_back(L'\0');
	return buffer;
}

void ShellAPI::FileOperationCopy(Collections::Generic::IEnumerable<String^>^ srcPath, String^ destPath)
{
	auto src = pathListToDoubleNullTerminatedString(srcPath);
	auto dest = pathListToDoubleNullTerminatedString(destPath);

	SHFILEOPSTRUCT fileOp = { 0 };
	fileOp.wFunc = FO_COPY;
	fileOp.pFrom = src.data();
	fileOp.pTo = dest.data();
	fileOp.fFlags = FOF_NOCONFIRMMKDIR;

	SHFileOperation(&fileOp);
}

void ShellAPI::FileOperationMove(Collections::Generic::IEnumerable<String^>^ srcPath, String^ destPath)
{
	auto src = pathListToDoubleNullTerminatedString(srcPath);
	auto dest = pathListToDoubleNullTerminatedString(destPath);

	SHFILEOPSTRUCT fileOp = { 0 };
	fileOp.wFunc = FO_MOVE;
	fileOp.pFrom = src.data();
	fileOp.pTo = dest.data();
	fileOp.fFlags = FOF_NOCONFIRMMKDIR;

	SHFileOperation(&fileOp);
}

void ShellAPI::FileOperationDelete(Collections::Generic::IEnumerable<String^>^ paths)
{
	auto src = pathListToDoubleNullTerminatedString(paths);
	SHFILEOPSTRUCT fileOp = { 0 };
	fileOp.wFunc = FO_DELETE;
	fileOp.pFrom = src.data();
	fileOp.fFlags = 0;
	SHFileOperation(&fileOp);
}

void ShellAPI::CreateShortCut(String^ srcPath, String^ destPath, String^ _description)
{
	pin_ptr<const wchar_t> src = PtrToStringChars(srcPath);
	pin_ptr<const wchar_t> dest = PtrToStringChars(destPath);
	pin_ptr<const wchar_t> description = PtrToStringChars(_description);

	IShellLink* psl;
	HRESULT hres = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, IID_IShellLink, (LPVOID*)&psl);

	if (SUCCEEDED(hres))
	{
		psl->SetPath(src);
		psl->SetDescription(description);

		IPersistFile* ppf;
		hres = psl->QueryInterface(IID_IPersistFile, (LPVOID*)&ppf);

		if (SUCCEEDED(hres))
		{
			hres = ppf->Save(dest, TRUE);
			ppf->Release();
		}
		psl->Release();
	}
}

#if 0
void ShellAPI::ShowShellDropContextMenu(array<String^>^ paths, String^ targetDir, IntPtr hwnd)
{
	ShowShellDropContextMenu(paths, targetDir, hwnd, INT_MIN, INT_MIN);
}

void ShellAPI::ShowShellDropContextMenu(String^ dropDir, IntPtr dropInterfacePtr, IntPtr dataInterfacePtr, IntPtr hwnd, int x, int y)
{
	CComQIPtr<IDropTarget> ctxmenu((IUnknown*)dropInterfacePtr.ToPointer());
	CComPtr<IShellItem> targetDirItem;
	CComPtr<IShellFolder> targetFolder;
	{
		pin_ptr<const wchar_t> targetDirPathPtr = PtrToStringChars(dropDir);
		SHCreateItemFromParsingName(targetDirPathPtr, nullptr, IID_PPV_ARGS(&targetDirItem));
	}
	if (targetDirItem)
	{
		targetDirItem->BindToHandler(nullptr, BHID_SFObject, IID_PPV_ARGS(&targetFolder));
	}
	CComQIPtr<IShellExtInit> shellExtInit(targetFolder);
	CComQIPtr<IDataObject> dataObject((IUnknown*)dataInterfacePtr.ToPointer());
	if (shellExtInit && dataObject)
	{
		pin_ptr<const wchar_t> wchPath = PtrToStringChars(dropDir);
		LPITEMIDLIST pidl = nullptr;
		HRESULT hr = SHParseDisplayName(wchPath, nullptr, &pidl, 0, nullptr);
		if (SUCCEEDED(hr) && pidl)
		{
			if (SUCCEEDED(shellExtInit->Initialize(pidl, dataObject, 0)))
			{
				;
				if (auto contextMenu = CComQIPtr<IContextMenu>(shellExtInit))
				{
					HMENU hMenu = CreatePopupMenu();
					if (hMenu)
					{
						auto hr = contextMenu->QueryContextMenu(hMenu, 0, 1, 0x7FFF, CMF_EXPLORE | CMF_EXTENDEDVERBS);
						if (SUCCEEDED(hr))
						{
							if (x == INT_MIN || y == INT_MIN)
							{
								POINT point;
								GetCursorPos(&point);
								x = point.x;
								y = point.y;
							}
							int cmd = TrackPopupMenu(hMenu, TPM_RETURNCMD | TPM_RIGHTBUTTON, x, y, 0, (HWND)hwnd.ToPointer(), nullptr);
							if (cmd > 0)
							{
								CMINVOKECOMMANDINFOEX info = { 0 };
								info.cbSize = sizeof(info);
								info.fMask = CMIC_MASK_UNICODE | CMIC_MASK_PTINVOKE;
								info.hwnd = (HWND)hwnd.ToPointer();
								info.lpVerb = MAKEINTRESOURCEA(cmd - 1);
								info.lpVerbW = MAKEINTRESOURCEW(cmd - 1);
								info.nShow = SW_SHOWNORMAL;
								info.ptInvoke.x = x;
								info.ptInvoke.y = y;
								contextMenu->InvokeCommand((LPCMINVOKECOMMANDINFO)&info);
							}
						}
						DestroyMenu(hMenu);
					}
				}
			}
			CoTaskMemFree(pidl);
		}		
	}
}

void ShellAPI::ShowShellDropContextMenu(array<String^>^ paths, String^ targetDir, IntPtr hwnd, int x, int y)
{
	//CComPtr<IDataObject> srcDataObj;
	//if(auto srcItemArray = filepathsToShellItemArray(paths))
	//	srcItemArray->BindToHandler(nullptr, BHID_DataObject, IID_PPV_ARGS(&srcDataObj));
	// パスを ITEMIDLIST に変換
	std::vector<LPITEMIDLIST> pidls;
	for each (String ^ path in paths)
	{
		pin_ptr<const wchar_t> wchPath = PtrToStringChars(path);
		LPITEMIDLIST pidl = nullptr;
		HRESULT hr = SHParseDisplayName(wchPath, nullptr, &pidl, 0, nullptr);
		if (SUCCEEDED(hr) && pidl)
		{
			pidls.push_back(pidl);
		}
	}

	//IUnknown* unknown = (IUnknown*)unknownPtr.ToPointer();
	//CComQIPtr<IShellExtInit> shellExtInit(unknown);
	//if(shellExtInit)
	//{
	//	CComPtr<IDataObject> dataObject;
	//	shellExtInit->Initialize(pidls[0], nullptr, 0, nullptr);
	//	shellExtInit->QueryInterface(IID_IDataObject, (void**)&dataObject);
	//}

	CComPtr<IShellItem> targetDirItem;
	CComPtr<IShellFolder> targetFolder;
	{
		pin_ptr<const wchar_t> targetDirPathPtr = PtrToStringChars(targetDir);
		SHCreateItemFromParsingName(targetDirPathPtr, nullptr, IID_PPV_ARGS(&targetDirItem));
	}
	if (targetDirItem)
	{
		targetDirItem->BindToHandler(nullptr, BHID_SFObject, IID_PPV_ARGS(&targetFolder));
	}

	DEFCONTEXTMENU dcm = {};
	dcm.hwnd = (HWND)hwnd.ToPointer();
	dcm.cidl = pidls.size();	// ドラッグ中のオブジェクト数
	dcm.apidl = (LPCITEMIDLIST*)pidls.data();	// 「ドラッグ中」のオブジェクト	
	dcm.psf = targetFolder;		// ターゲットフォルダー

	CComPtr<IContextMenu> contextMenu;
	SHCreateDefaultContextMenu(&dcm, IID_PPV_ARGS(&contextMenu));


	// コンテキストメニューを表示
	HMENU hMenu = CreatePopupMenu();
	if (hMenu)
	{
		auto hr = contextMenu->QueryContextMenu(hMenu, 0, 1, 0x7FFF, CMF_EXPLORE | CMF_EXTENDEDVERBS);
		if (SUCCEEDED(hr))
		{
			if (x == INT_MIN || y == INT_MIN)
			{
				POINT point;
				GetCursorPos(&point);
				x = point.x;
				y = point.y;
			}
			int cmd = TrackPopupMenu(hMenu, TPM_RETURNCMD | TPM_RIGHTBUTTON, x, y, 0, (HWND)hwnd.ToPointer(), nullptr);
			if (cmd > 0)
			{
				CMINVOKECOMMANDINFOEX info = { 0 };
				info.cbSize = sizeof(info);
				info.fMask = CMIC_MASK_UNICODE | CMIC_MASK_PTINVOKE;
				info.hwnd = (HWND)hwnd.ToPointer();
				info.lpVerb = MAKEINTRESOURCEA(cmd - 1);
				info.lpVerbW = MAKEINTRESOURCEW(cmd - 1);
				info.nShow = SW_SHOWNORMAL;
				info.ptInvoke.x = x;
				info.ptInvoke.y = y;
				contextMenu->InvokeCommand((LPCMINVOKECOMMANDINFO)&info);
			}
		}
		DestroyMenu(hMenu);
	}

	// メモリを解放
	for (auto pidl : pidls)
	{
		CoTaskMemFree(pidl);
	}
}
#endif

