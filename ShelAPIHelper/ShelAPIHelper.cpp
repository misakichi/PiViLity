#include "pch.h"

#include "ShelAPIHelper.h"
#include <shlobj.h>
#include <commoncontrols.h>
#include <vcclr.h>
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

using namespace ShelAPIHelper;

String^ SpecialFolder::GetMyCompute()
{
    GUID folderId = { 0x374DE290, 0x123F, 0x4565, { 0xBC, 0x9C, 0x2D, 0x7B, 0xC3, 0xB3, 0xD6, 0xAC } }; // This PC ‚Ì GUID
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

int FileInfo::GetFileIconIndex(String^ path)
{
    pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
    SHFILEINFO info = {};
    SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_ICON | SHGFI_SYSICONINDEX);
    return info.iIcon;
}

System::Drawing::Icon^ FileInfo::GetFileLargeIconFromIndex(int index)
{
    auto icon = ImageList_GetIcon(systemLargeImageList_, index, ILD_TRANSPARENT);
    return System::Drawing::Icon::FromHandle(IntPtr(icon));
}

System::Drawing::Icon^ FileInfo::GetFileSmallIconFromIndex(int index)
{
    auto icon = ImageList_GetIcon(systemSmallImageList_, index, ILD_TRANSPARENT);
    return System::Drawing::Icon::FromHandle(IntPtr(icon));
}

System::Drawing::Icon^ FileInfo::GetFileJumboIconFromIndex(int index)
{
    auto icon = ImageList_GetIcon(systemJumboImageList_, index, ILD_TRANSPARENT);
    return System::Drawing::Icon::FromHandle(IntPtr(icon));
}


System::Drawing::Icon^ FileInfo::GetFileLargeIcon(String^ path)
{
    pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
    SHFILEINFO info = {};
    SHGetFileInfo(wcharPath,0,&info, sizeof(info), SHGFI_ICON | SHGFI_LARGEICON);

    return System::Drawing::Icon::FromHandle(IntPtr(info.hIcon));
}

System::Drawing::Icon^ FileInfo::GetFileSmallIcon(String^ path)
{
    pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
    SHFILEINFO info = {};
    SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_ICON | SHGFI_SMALLICON);

    return System::Drawing::Icon::FromHandle(IntPtr(info.hIcon));
}

System::Drawing::Icon^ FileInfo::GetFileJumboIcon(String^ path)
{
    pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
    SHFILEINFO info = {};
    SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_ICON |  SHGFI_SYSICONINDEX);

    auto icon = ImageList_GetIcon(systemJumboImageList_, info.iIcon, ILD_TRANSPARENT);
    return System::Drawing::Icon::FromHandle(IntPtr(icon));
}
