#pragma once

using namespace System;

namespace ShelAPIHelper
{

	public ref class SpecialFolder
	{
	public:
		static String^ GetMyCompute();
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
