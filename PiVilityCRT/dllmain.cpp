// dllmain.cpp : DLL アプリケーションのエントリ ポイントを定義します。
#include "pch.h"
#include "SysImageList.h"

#include <atomic>
static std::atomic<int> g_atachCount = 0;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
		if (g_atachCount.fetch_add(1) == 0)
		{
			InitializeSysImageList();
		}
		break;
		
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
		if (g_atachCount.fetch_sub(1) == 1)
		{
			TerminateSysImageList();
		}
        break;
    }
    return TRUE;
}

