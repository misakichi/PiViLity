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
    GUID folderId = { 0x374DE290, 0x123F, 0x4565, { 0xBC, 0x9C, 0x2D, 0x7B, 0xC3, 0xB3, 0xD6, 0xAC } }; // This PC �� GUID
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
/// �ÓI�R���X�g���N�^����Ă΂�鏉���������B
/// �V�X�e���C���[�W���X�g�̃n���h�����擾���Ă����B
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
/// �t�@�C���p�X����t�@�C���̃A�C�R���C���f�b�N�X���擾���܂��B
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
/// �V�X�e���C���[�W���X�g�̃C���f�b�N�X����A�C�R���i��j���擾���܂��B
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
/// �V�X�e���C���[�W���X�g�̃C���f�b�N�X����A�C�R���i���j���擾���܂��B
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
/// �V�X�e���C���[�W���X�g�̃C���f�b�N�X����A�C�R���iJumbo�j���擾���܂��B
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
/// �t�@�C���p�X����t�@�C���̃A�C�R���i��j���擾���܂��B
/// <param name="path">�t�@�C���p�X</param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileLargeIcon(String^ path)
{
    pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
    SHFILEINFO info = {};
    SHGetFileInfo(wcharPath,0,&info, sizeof(info), SHGFI_ICON | SHGFI_LARGEICON);

    return System::Drawing::Icon::FromHandle(IntPtr(info.hIcon));
}

/// <summary>
/// �t�@�C���p�X����t�@�C���̃A�C�R���i���j���擾���܂��B
/// </summary>
/// <param name="path">�t�@�C���p�X</param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileSmallIcon(String^ path)
{
    pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
    SHFILEINFO info = {};
    SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_ICON | SHGFI_SMALLICON);

    return System::Drawing::Icon::FromHandle(IntPtr(info.hIcon));
}

/// <summary>
/// �t�@�C���p�X����t�@�C���̃A�C�R���iJumbo�j���擾���܂��B
/// </summary>
/// <param name="path">�t�@�C���p�X</param>
/// <returns></returns>
System::Drawing::Icon^ FileInfo::GetFileJumboIcon(String^ path)
{
    pin_ptr<const wchar_t> wcharPath = PtrToStringChars(path);
    SHFILEINFO info = {};
    SHGetFileInfo(wcharPath, 0, &info, sizeof(info), SHGFI_ICON |  SHGFI_SYSICONINDEX);

    auto icon = ImageList_GetIcon(systemJumboImageList_, info.iIcon, ILD_TRANSPARENT);
    return System::Drawing::Icon::FromHandle(IntPtr(icon));
}
