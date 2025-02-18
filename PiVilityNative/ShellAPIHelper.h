#pragma once


using namespace System;

namespace PiVilityNative
{
	public ref class ShellAPI
	{
	public:
		static String^ GetMyCompute();
		static void ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd);
		static void ShowShellContextMenu(array<String^>^ paths, IntPtr hwnd, int x, int);

		static void FileOperationCopy(Collections::Generic::IEnumerable<String^>^ srcPath, String^ destPath);
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
		//static IntPtr GetLargeImageList();
		//static IntPtr GetSmallImageList();
		//static int GetFileLargeIconIndex(String^ path);
		//static int GetFileSmallIconIndex(String^ path);
		static int GetFileIconIndex(String^ path);
		static System::Drawing::Icon^ GetFileLargeIconFromIndex(int index);
		static System::Drawing::Icon^ GetFileSmallIconFromIndex(int index);
		static System::Drawing::Icon^ GetFileJumboIconFromIndex(int index);
		static System::Drawing::Icon^ GetFileLargeIcon(String^ path);
		static System::Drawing::Icon^ GetFileSmallIcon(String^ path);
		static System::Drawing::Icon^ GetFileJumboIcon(String^ path);

	private:
		static void StaticConstruct();
		static HIMAGELIST systemSmallImageList_;
		static HIMAGELIST systemLargeImageList_;
		static HIMAGELIST systemJumboImageList_;
	};


}
