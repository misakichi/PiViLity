#pragma once


using namespace System;

namespace PiVilityNative
{
	public ref class CustomMenuItem
	{
	public:
		System::Action^ action = nullptr;
		String^ name = "";
		bool isDefault = false;
	};

	/// <summary>
	/// .Netでは実現の難しそうな機能を提供する
	/// </summary>
	public ref class ShellAPI
	{
	public:
		/// <summary>
		/// PC名
		/// </summary>
		/// <returns></returns>
		static String^ GetMyCompute();

		/// <summary>
		/// シェルコンテキストメニューの表示
		/// </summary>
		/// <param name="paths"></param>
		/// <param name="hwnd"></param>
		static void ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd);

		/// <summary>
		/// シェルコンテキストメニューの表示
		/// （位置指定）
		/// </summary>
		/// <param name="paths"></param>
		/// <param name="hwnd"></param>
		static void ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd, int x, int y);

		/// <summary>
		/// シェルコンテキストメニューの表示
		/// （カスタムメニューあり）
		/// </summary>
		/// <param name="paths"></param>
		/// <param name="hwnd"></param>
		static void ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd, int x, int y, array<CustomMenuItem^>^ customMenus);

		/// <summary>
		/// シェルのファイルコピー機能
		/// </summary>
		/// <param name="srcPath"></param>
		/// <param name="destPath"></param>
		static void FileOperationCopy(Collections::Generic::IEnumerable<String^>^ srcPath, String^ destPath);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srcPath"></param>
		/// <param name="destPath"></param>
		static void FileOperationMove(Collections::Generic::IEnumerable<String^>^ srcPath, String^ destPath);
		static void FileOperationDelete(Collections::Generic::IEnumerable<String^>^ paths);
		static void CreateShortCut(String^ srcPath, String^ destPath, String^ description);
	};

	public ref class FileInfo
	{
	public:
		static FileInfo() {
			StaticConstruct();
		}

		static System::String^ GetFileTypeName(String^ pathOrExtension);
		static int GetFileIconIndex(Environment::SpecialFolder specialFolder);
		static int GetFileIconIndex(String^ path);
		static System::Drawing::Icon^ GetFileLargeIconFromIndex(int index);
		static System::Drawing::Icon^ GetFileSmallIconFromIndex(int index);
		static System::Drawing::Icon^ GetFileJumboIconFromIndex(int index);
		static System::Drawing::Icon^ GetFileLargeIcon(String^ path);
		static System::Drawing::Icon^ GetFileSmallIcon(String^ path);
		static System::Drawing::Icon^ GetFileJumboIcon(String^ path);

	private:
		static void StaticConstruct();
	};


}
