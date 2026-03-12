#pragma once

struct SusiePluginComImpl;

namespace PiVilityNative
{
	public ref class SPIPictureInfo
	{
	public:
		 int left;
		 int top;
		 int width;
		 int height;
		 WORD x_density;
		 WORD y_density;
		 short colorDepth;
		 System::String^ info;
	};
	public ref class SusiePluginCom : System::IDisposable
	{
	public:
		SusiePluginCom();
		~SusiePluginCom();

		bool Load(System::String^ path);
		bool GetPluginInfo(int infono, System::String^% buf);

		bool IsSupportedStream(System::String^ filename, System::IO::Stream^ stream);

		bool IsSupportedBuffer(System::String^ filename, array<byte>^ buffer);

		bool GetPictureInfoFile(System::String^ filename, SPIPictureInfo^% info);

		bool GetPictureFile(System::String^ filename, System::IO::MemoryMappedFiles::MemoryMappedFile^% info, System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp);

		bool GetPreviewFile(System::String^ filename, System::IO::MemoryMappedFiles::MemoryMappedFile^% info, System::IO::MemoryMappedFiles::MemoryMappedFile^% bmp);

		bool FinishGetPicture(void);


		!SusiePluginCom();

	private:
		SusiePluginComImpl* impl_;
	};


}
