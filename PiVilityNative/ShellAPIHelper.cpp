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
/// シェルコンテキストメニューを表示します。
/// </summary>
/// <param name="paths"></param>
/// <param name="hwnd"></param>
/// <param name="x"></param>
/// <param name="y"></param>
void ShellAPI::ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd, int x, int y)
{
    // パスをWCHAR*の配列に変換
    std::vector<std::wstring> wPaths;
    for each(String ^ path in paths)
    {
        pin_ptr<const wchar_t> wchPath = PtrToStringChars(path);
        wPaths.push_back(std::wstring(wchPath));
    }

    auto shellItems = filepathsToShellItemArray(wPaths);
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
