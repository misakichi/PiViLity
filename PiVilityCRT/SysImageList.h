#if !defined(__PIVILITY_CRT_SYS_IMAGE_LIST_H__)
#define __PIVILITY_CRT_SYS_IMAGE_LIST_H__

#ifndef DLL_DCECLSPEC
#define DLL_DCECLSPEC __declspec(dllimport)
#endif

#ifndef DECLARE_HANDLE
typedef void* HICON;
#endif

DLL_DCECLSPEC BOOL _cdecl InitializeSysImageList();
DLL_DCECLSPEC void _cdecl TerminateSysImageList();
DLL_DCECLSPEC HICON _cdecl GetSysImageListSmallIcon(int index);
DLL_DCECLSPEC HICON _cdecl GetSysImageListLargeIcon(int index);
DLL_DCECLSPEC HICON _cdecl GetSysImageListJumboIcon(int index);


#endif