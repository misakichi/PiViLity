#pragma once

struct SusiePluginComImpl;
using namespace System;
using namespace System::Runtime::InteropServices;

namespace PiVilityNative
{
 public ref class SPIPictureInfo
	{
	public:
		// 画像の基本情報を保持するコンテナです。
		// Simple container for basic picture information. Comment: GitHub Copilot
		int left; // 左位置。
		int top; // 上位置。
		int width; // 幅。
		int height; // 高さ。
		WORD x_density; // 横方向の画素密度。
		WORD y_density; // 縦方向の画素密度。
		short colorDepth; // カラーデプス（ビット/ピクセル）。
		String^ info; // プラグインが返す任意の情報文字列（null可）。
	};

	/// <summary>
	/// Managed wrapper around the native Susie COM plugin interface.
	/// Responsible for marshalling strings/buffers and exposing plugin
	/// functionality to .NET callers.
	/// </summary>
	public ref class SusiePluginCom : System::IDisposable
	{
	public:
		// COMラッパーを作成し初期化します（CoCreateInstanceを内部で使用）。
		// Create and initialize the COM wrapper (uses CoCreateInstance). Comment: GitHub Copilot
		SusiePluginCom();
		// 明示的な解放（IDisposableパターン）。
		// Deterministic cleanup (IDisposable). Comment: GitHub Copilot
		~SusiePluginCom();

		// 指定したパスのプラグインまたはリソースを読み込みます。COMエラー時は例外を投げます。
		// Load a plugin or resource by the specified path. Throws on COM error. Comment: GitHub Copilot
		bool Load(String^ path);
		
		// プラグインの情報文字列を取得します（infonoは情報タイプのインデックス）。
		// Retrieves an information string from the plugin. 'infono' specifies the info index. Comment: GitHub Copilot
		bool GetPluginInfo(int infono, [Out] String^% buf);

		// 指定したストリームがサポート済みのフォーマットかを判定します。
		// Determines whether the provided stream contains a supported format. Comment: GitHub Copilot
		bool IsSupportedStream(String^ filename, System::IO::Stream^ stream);

		// 生のバッファがサポートされているフォーマットかを判定します。
		// Determines whether the provided raw buffer contains a supported format. Comment: GitHub Copilot
		bool IsSupportedBuffer(String^ filename, array<byte>^ buffer);

		// ファイルから画像の基本情報（幅・高さ・密度など）を取得します。
		// Retrieves basic picture information (dimensions, density, etc.) for a file. Comment: GitHub Copilot
		bool GetPictureInfoFile(String^ filename, SPIPictureInfo^% info);

		// 共有メモリ（メモリマップドファイル）を使って画像データと情報を取得します。
		// Returns picture and info as memory-mapped files accessible by name. Comment: GitHub Copilot
		bool GetPictureFile(String^ filename, [Out] System::IO::MemoryMappedFiles::MemoryMappedFile^% info, [Out] System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp);

		System::Drawing::Bitmap^ GetPictureFileToBmp(System::String^ filename);

		// プレビュー画像とその情報を共有メモリ経由で取得します。
		// Retrieves a preview image and its info via shared memory. Comment: GitHub Copilot
		bool GetPreviewFile(String^ filename, [Out] System::IO::MemoryMappedFiles::MemoryMappedFile^% info, [Out] System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp);
		System::Drawing::Bitmap^ GetPreviewFileToBmp(System::String^ filename);

		// 画像取得処理の完了をプラグインに通知します。
		// Signals to the plugin that the caller has finished retrieving picture data. Comment: GitHub Copilot
		bool FinishGetPicture(void);


		// ガベージコレクタによって呼ばれるファイナライザ。Disposeが呼ばれなかった場合の後始末を行います。
		// Finalizer called by the garbage collector if Dispose wasn't called. Comment: GitHub Copilot
		!SusiePluginCom();

	private:
		// Pointer to the unmanaged implementation that holds COM references.
		SusiePluginComImpl* impl_;
	};


}
