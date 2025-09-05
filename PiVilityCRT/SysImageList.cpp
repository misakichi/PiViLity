#include "pch.h"
#include "SysImageList.h"
#include <shlobj.h>
#include <shellapi.h>
#include <initguid.h> 
#include <commoncontrols.h>
#pragma comment(lib,"Comctl32.lib")

static HIMAGELIST systemSmallImageList_ = NULL;
static HIMAGELIST systemLargeImageList_ = NULL;
static HIMAGELIST systemJumboImageList_ = NULL;


DLL_DCECLSPEC BOOL _cdecl InitializeSysImageList()
{
	HIMAGELIST hImageList;
	SHGetImageList(SHIL_SYSSMALL, IID_IImageList, (PVOID*)&hImageList);
	systemSmallImageList_ = hImageList;
	SHGetImageList(SHIL_EXTRALARGE, IID_IImageList, (PVOID*)&hImageList);
	systemLargeImageList_ = hImageList;
	SHGetImageList(SHIL_JUMBO, IID_IImageList, (PVOID*)&hImageList);
	systemJumboImageList_ = hImageList;

	return systemJumboImageList_ != NULL && systemLargeImageList_ != NULL && systemSmallImageList_ != NULL;
}

DLL_DCECLSPEC void _cdecl TerminateSysImageList()
{
}

DLL_DCECLSPEC HICON _cdecl GetSysImageListSmallIcon(int index)
{
	return ImageList_GetIcon(systemSmallImageList_, index, ILD_TRANSPARENT);
}

DLL_DCECLSPEC HICON _cdecl GetSysImageListLargeIcon(int index)
{
	return ImageList_GetIcon(systemLargeImageList_, index, ILD_TRANSPARENT);
}

DLL_DCECLSPEC HICON _cdecl GetSysImageListJumboIcon(int index)
{
	return ImageList_GetIcon(systemJumboImageList_, index, ILD_TRANSPARENT);
}
