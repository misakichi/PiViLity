#include <windows.h>
#include <shlobj.h>
#include <shobjidl.h>
#include <shellapi.h>
#include <atlbase.h>
#include <atlcomcli.h>
#include <vector>
#include <string>

// ヘルパー: パスを IShellItem に変換
HRESULT CreateShellItemFromPath(const std::wstring& path, IShellItem** ppsi) {
    if (!ppsi) return E_INVALIDARG;
    return SHCreateItemFromParsingName(path.c_str(), nullptr, IID_PPV_ARGS(ppsi));
}

// ヘルパー: 複数の IShellItem からデータオブジェクトを作成
HRESULT CreateDataObjectFromMultipleShellItems(const std::vector<std::wstring>& paths, IDataObject** ppDataObj) {
    if (!ppDataObj) return E_INVALIDARG;

    // パスを IShellItemArray に変換
    std::vector<CComPtr<IShellItem>> items;
    items.reserve(paths.size());
    for (auto& path : paths) {
        CComPtr<IShellItem> spItem;
        HRESULT hr = CreateShellItemFromPath(path, &spItem);
        if (FAILED(hr)) {
            return hr;
        }
        items.push_back(spItem);
    }

    CComPtr<IObjectArray> spObjArray;
    HRESULT hr = CoCreateInstance(CLSID_EnumerableObjectCollection, nullptr, CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(&spObjArray));
    if (FAILED(hr)) {
        return hr;
    }

    CComQIPtr<IObjectCollection> spObjCollection(spObjArray);
    if (!spObjCollection) {
        return E_FAIL;
    }

    for (auto& item : items) {
        spObjCollection->AddObject(item);
    }

    CComPtr<IShellItemArray> spShellItemArray;
    hr = spObjCollection->QueryInterface(IID_PPV_ARGS(&spShellItemArray));
    if (FAILED(hr)) {
        return hr;
    }

    // IShellItemArray を IDataObject に変換
    hr = spShellItemArray->BindToHandler(nullptr, BHID_DataObject, IID_PPV_ARGS(ppDataObj));
    return hr;
}

// targetDir と dragFiles に対して、デフォルトの「ドラッグ＆ドロップ」スタイルのコンテキストメニューを表示
HRESULT ShowExplorerDragDropContextMenu(HWND hwndOwner, const std::wstring& targetDir, const std::vector<std::wstring>& dragFiles) {
    // 1) dragFiles からデータオブジェクトを作成
    CComPtr<IDataObject> spDataObj;
    HRESULT hr = CreateDataObjectFromMultipleShellItems(dragFiles, &spDataObj);
    if (FAILED(hr)) {
        return hr;
    }

    // 2) targetDir の IShellItem を作成
    CComPtr<IShellItem> spTargetShellItem;
    hr = CreateShellItemFromPath(targetDir, &spTargetShellItem);
    if (FAILED(hr)) {
        return hr;
    }

    // 3) 高度な使用のために targetDir の IShellItem を IShellFolder に変換
    //    これを DEFCONTEXTMENU::psf に渡し、シェルがターゲットディレクトリを「コンテキスト」または「ドロップターゲット」フォルダーとして扱えるようにします。
    CComPtr<IShellFolder> spTargetFolder;
    if (spTargetShellItem) {
        hr = spTargetShellItem->BindToHandler(nullptr, BHID_SFObject, IID_PPV_ARGS(&spTargetFolder));
        // これが失敗しても致命的ではないので、すぐには戻りません。
    }

    // 4) DEFCONTEXTMENU 構造体を準備
    DEFCONTEXTMENU dcm = {};
    dcm.hwnd = hwndOwner;
    dcm.pdtobj = spDataObj; // 「ドラッグ中」のオブジェクト
    // 利用可能な場合、ターゲットフォルダーを psf として設定:
    dcm.psf = spTargetFolder;

    // 追加制御が必要な場合、punkAssociation や pcmcb コールバックを使用できます
    dcm.punkAssociation = nullptr;
    dcm.pcmcb = nullptr;

    // 5) デフォルトのコンテキストメニューを作成
    CComPtr<IContextMenu> spContextMenu;
    hr = SHCreateDefaultContextMenu(&dcm, IID_PPV_ARGS(&spContextMenu));
    if (FAILED(hr)) {
        return hr;
    }

    // 6) メニューをビルド
    HMENU hMenu = CreatePopupMenu();
    if (!hMenu) {
        return E_OUTOFMEMORY;
    }

    UINT uFlags = CMF_NORMAL;
    hr = spContextMenu->QueryContextMenu(hMenu, 0, 1, 0x7FFF, uFlags);
    if (FAILED(hr)) {
        DestroyMenu(hMenu);
        return hr;
    }

    // 7) 現在のカーソル位置にコンテキストメニューを表示
    POINT pt = {};
    GetCursorPos(&pt);

    UINT nCmd = TrackPopupMenuEx(
        hMenu,
        TPM_RETURNCMD | TPM_RIGHTBUTTON | TPM_NONOTIFY,
        pt.x, pt.y,
        hwndOwner,
        nullptr
    );

    // 8) 何かが選択された場合、コマンドを実行
    if (nCmd >= 1) {
        CMINVOKECOMMANDINFOEX cmi = {};
        cmi.cbSize = sizeof(cmi);
        cmi.fMask = CMIC_MASK_UNICODE;
        cmi.hwnd = hwndOwner;
        cmi.lpVerb = MAKEINTRESOURCEA(nCmd - 1); // idCmdFirst によるオフセット
        cmi.nShow  = SW_SHOWNORMAL;

        hr = spContextMenu->InvokeCommand(reinterpret_cast<LPCMINVOKECOMMANDINFO>(&cmi));
    }

    DestroyMenu(hMenu);
    return hr;
}

// 簡単な WinMain の使用例:
int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE, LPWSTR, int) {
    // シェル操作のために COM を初期化
    HRESULT hr = CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);
    if (SUCCEEDED(hr)) {
        std::wstring targetDir = L"C:\\TargetFolder"; 
        std::vector<std::wstring> dragFiles = {
            L"C:\\SomeFile1.txt",
            L"C:\\SomeFile2.docx"
        };

        ShowExplorerDragDropContextMenu(nullptr, targetDir, dragFiles);

        CoUninitialize();
    }
    return 0;
}