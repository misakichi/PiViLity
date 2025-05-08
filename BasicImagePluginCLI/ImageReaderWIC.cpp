#define NOMINMAX
#pragma unmanaged
#include <algorithm>
#include <wincodec.h>
#pragma comment(lib, "windowscodecs.lib")
#pragma comment(lib, "ole32.lib")
#pragma comment(lib, "gdi32.lib")
#pragma comment(lib, "User32.lib")
template<class T>
class ComPtr {
public:
	ComPtr() : ptr_(nullptr) {}
	ComPtr(T* ptr) : ptr_(ptr) { if (ptr_) ptr_->AddRef(); }
	ComPtr(const ComPtr& other) : ptr_(other.ptr_) { if (ptr_) ptr_->AddRef(); }
	~ComPtr() { if (ptr_) ptr_->Release(); }
	ComPtr& operator=(const ComPtr& other) {
		if (this != &other) {
			if (ptr_) 
                ptr_->Release();
			ptr_ = other.ptr_;
			if (ptr_)
                ptr_->AddRef();
		}
		return *this;
	}
	T* Get() { return ptr_; }
	const T* Get() const { return ptr_; }
	void Reset(T* ptr = nullptr) {
		if (ptr_) 
            ptr_->Release();
		ptr_ = ptr;
		if (ptr_) 
            ptr_->AddRef();
	}
	T** GetAddressOf() { return &ptr_; }
	operator T* () { return ptr_; }
	operator const T* () const { return ptr_; }
	T* operator->() { return ptr_; }
    const T* operator->() const { return ptr_; }
private:
	T* ptr_;
};
#pragma managed

#include "ImageReaderWIC.h"
#include <vcclr.h>
using namespace BasicImagePluginCLI;
using namespace System;

class WicFactory
{
public:
    WicFactory()
    {
        HRESULT hr = CoCreateInstance(
            CLSID_WICImagingFactory,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_IWICImagingFactory,
			(LPVOID*)wicFactory_.GetAddressOf()
        );
    }
    IWICImagingFactory* operator->()
	{
		return wicFactory_;
	}
private:
    ComPtr<IWICImagingFactory> wicFactory_ = nullptr;
};
static WicFactory s_WicFactory;

#pragma unmanaged
#include <string>
namespace BasicImagePluginCLI
{
    class ImageReaderWICNativeImpl
    {
    public:
        ImageReaderWICNativeImpl()
        {
        }
        ~ImageReaderWICNativeImpl()
        {
            Terminate();
        }
        bool  Initialize(const wchar_t* filePath)
        {
			ComPtr<IWICBitmapDecoder> decoder = nullptr;
            ComPtr<IWICBitmapFrameDecode> frameDecode_ = nullptr;
            HRESULT hr = s_WicFactory->CreateDecoderFromFilename(
                filePath,
                nullptr,
                GENERIC_READ,
                WICDecodeMetadataCacheOnDemand,
                decoder.GetAddressOf()
            );
            if (FAILED(hr)) {
                return false;
            }
            
			ComPtr<IWICBitmapFrameDecode> frameDecoder = nullptr;
            hr = decoder->GetFrame(0, frameDecoder.GetAddressOf());
            if (FAILED(hr)) {
                return false;
            }
			decoder_ = decoder;
			frameDecoder_ = frameDecoder;
            frameDecoder_->GetSize(&width_, &height_);

            path_ = filePath;

            return true;
        }
        void Terminate()
        {
            frameDecoder_ = nullptr;
            decoder_ = nullptr;
        }
		bool isValidate() const
		{
			return (decoder_ != nullptr && frameDecoder_ != nullptr);
		}
		bool isInitialized(const wchar_t* filePath) const
		{
			return (decoder_ != nullptr && frameDecoder_ != nullptr && _wcsicmp( path_.c_str(), filePath) == 0);
		}
        HBITMAP GetImage()
        {
            ComPtr<IWICFormatConverter> converter;
            if (SUCCEEDED(s_WicFactory->CreateFormatConverter(converter.GetAddressOf())))
            {
                if (SUCCEEDED(converter->Initialize(frameDecoder_.Get(), GUID_WICPixelFormat32bppBGRA, WICBitmapDitherTypeNone, NULL, 0.0f, WICBitmapPaletteTypeCustom)))
                {
                    ComPtr<IWICBitmap> bmp;
					if (SUCCEEDED(s_WicFactory->CreateBitmapFromSource(converter.Get(), WICBitmapCacheOnDemand, bmp.GetAddressOf())))
					{
						return wicToGdi(bmp.Get(), width_, height_, 0, 0);
					}
                }
            }
			return nullptr;
        }
        HBITMAP GetThumbnailImage(int rqWidth, int rqHeight)
        {
            auto bmp = getThumbanailBmp(rqWidth, rqHeight);
			return wicToGdi(bmp.Get(), rqWidth, rqHeight, 0, 0);
        }
        HBITMAP GetThumbnailImage(int rqWidth, int rqHeight, int destWidth, int destHeight, int destOffsetX, int destOffsetY)
        {
            auto bmp = getThumbanailBmp(rqWidth, rqHeight);
			return wicToGdi(bmp.Get(), destWidth, destHeight, destOffsetX, destOffsetY);
        }
        uint32_t width() const { return width_; }
        uint32_t height() const { return height_; }
    private:
        ComPtr<IWICBitmapSource> getThumbanailBmp(int rqWidth, int rqHeight)
        {
            ComPtr<IWICBitmapSource> bmp;
            if (SUCCEEDED(frameDecoder_->GetThumbnail(bmp.GetAddressOf())))
            {
                ComPtr<IWICFormatConverter> converter;
                if (SUCCEEDED(s_WicFactory->CreateFormatConverter(converter.GetAddressOf())))
                {
                    if (SUCCEEDED(converter->Initialize(bmp.Get(), GUID_WICPixelFormat32bppBGRA, WICBitmapDitherTypeNone, NULL, 0.0f, WICBitmapPaletteTypeCustom)))
                    {
						bmp = converter.Get();
                    }
                }
                ComPtr<IWICBitmapScaler> scaler;
                s_WicFactory->CreateBitmapScaler(scaler.GetAddressOf());
                scaler->Initialize(bmp.Get(), rqWidth, rqHeight, WICBitmapInterpolationModeLinear);
                bmp = scaler.Get();
			}
            else
            {
                ComPtr<IWICBitmapScaler> scaler;
                s_WicFactory->CreateBitmapScaler(scaler.GetAddressOf());
                scaler->Initialize(frameDecoder_.Get(), rqWidth, rqHeight, WICBitmapInterpolationModeLinear);

                bmp = scaler.Get();
            }
			return bmp;
        }
        HBITMAP wicToGdi(IWICBitmapSource* bitmap, int destWidth, int destHeight, int destOffsetX, int destOffsetY, void** ppDib = nullptr)
		{
            if (!bitmap) 
                return nullptr;

            UINT width, height;
            bitmap->GetSize(&width, &height);

			WICRect rc = { 0, 0, static_cast<INT>(width), static_cast<INT>(height) };
            if (destOffsetX < 0)
            {
				rc.X = -destOffsetX;
                rc.Width += destOffsetX;
                destOffsetX = 0;
            }
            if (destOffsetY < 0)
            {
				rc.Y = -destOffsetY;
                rc.Height += destOffsetY;
                destOffsetY = 0;
            }
			rc.Width = std::min(rc.Width, destWidth - destOffsetX);
			rc.Height = std::min(rc.Height, destHeight - destOffsetY);

            BITMAPINFO bmi = {};
            bmi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
            bmi.bmiHeader.biWidth = destWidth;
            bmi.bmiHeader.biHeight = -static_cast<LONG>(destHeight);
            bmi.bmiHeader.biPlanes = 1;
            bmi.bmiHeader.biBitCount = 32;
            bmi.bmiHeader.biCompression = BI_RGB;
            void* pBits = nullptr;
            HDC hdc = GetDC(nullptr);
            HBITMAP hBitmap = CreateDIBSection(hdc, &bmi, DIB_RGB_COLORS, &pBits, NULL, 0);
            ReleaseDC(nullptr, hdc);

            if (!hBitmap) 
                return nullptr;
            
            
            auto offset = (destOffsetY * destWidth + destOffsetX) * 4;
            auto hr = bitmap->CopyPixels(&rc, destWidth * 4, destHeight * destWidth * 4 - offset, (BYTE*)pBits + offset);
			if (FAILED(hr)) {
				DeleteObject(hBitmap);
				return nullptr;
			}
            if(ppDib)
			    *ppDib = pBits;
#if 0
            WICRect rect = { 0, 0, static_cast<INT>(width), static_cast<INT>(height) };
            ComPtr<IWICBitmapLock> lock;
            if (SUCCEEDED(bitmap->l(&rect, WICBitmapLockRead, &lock))) {
                UINT bufferSize = 0;
                BYTE* buffer = nullptr;
				UINT stride = 0;
                lock->GetDataPointer(&bufferSize, &buffer);
                lock->GetStride(&stride);

                if (buffer) {
					if (destWidth == width && destHeight == height && destOffsetX==0 && destOffsetY==0)
					{
						memcpy(pBits, buffer, bufferSize);
					}
					else
					{
                        int cpWidth = width;
                        int cpHeight = height;
                        int srcOffsetX = 0;
                        int srcOffsetY = 0;
                        if (destOffsetX < 0)
                        {
                            srcOffsetX = -destOffsetX;
                            destOffsetX = 0;
                        }
                        if(destOffsetX<0)
						    cpWidth += destOffsetX;
                        else
							cpWidth = std::min(cpWidth, destWidth - destOffsetX);
						
                        if (destOffsetY < 0)
                        {
                            srcOffsetY = -destOffsetY;
                            destOffsetY = 0;
                        }
                        if (destOffsetY < 0)
                            cpHeight += destOffsetY;
                        else
                            cpHeight = std::min(cpHeight, destHeight - destOffsetY);


                        if (cpHeight > 0 && cpWidth > 0)
                        {
							BYTE* src = buffer + (srcOffsetY * stride) + (srcOffsetX * 4);
                            DWORD* dst = static_cast<DWORD*>(pBits) + destOffsetY + destOffsetX;
                            BYTE* destBits = static_cast<BYTE*>(pBits) + (destOffsetY * stride) + (destOffsetX * 4);
                            for (int y = 0; y < cpHeight; ++y)
                            {
                                memcpy(dst, src, cpWidth * 4);
                                src += stride;
                                dst += destWidth;
                            }
                        }
					}
                }
            }
#endif
            return hBitmap;
        }
    private:
        std::wstring path_;
        uint32_t width_ = 0;
        uint32_t height_ = 0;
        ComPtr<IWICBitmapDecoder> decoder_ = nullptr;
        ComPtr<IWICBitmapFrameDecode> frameDecoder_ = nullptr;
        //IWICMetadataQueryReader* metadataQueryReader_ = nullptr;
    };
}
#pragma managed

ImageReaderWIC::ImageReaderWIC()
{
}
ImageReaderWIC::~ImageReaderWIC()
{
	if (nativeImpl_ != nullptr)
	{
		delete nativeImpl_;
		nativeImpl_ = nullptr;
	}
}
ImageReaderWIC::!ImageReaderWIC()
{
	if (nativeImpl_ != nullptr)
	{
		delete nativeImpl_;
		nativeImpl_ = nullptr;
	}
}

#include <memory>
/// <summary>
/// 当リーダークラスがサポートする画像ファイルの拡張子リストを取得します。
/// </summary>
/// <returns></returns>
Collections::Generic::List<String^>^ ImageReaderWIC::GetSupportedExtensions()
{
	auto extensions = gcnew System::Collections::Generic::List<System::String^>();
	extensions->Add("jpg");
	extensions->Add("jpeg");
	extensions->Add("bmp");
	extensions->Add("png");

	return extensions;
}

/// <summary>
/// このプラグインが指定したファイルをサポートするかどうかを返します。
/// </summary>
/// <returns></returns>
bool ImageReaderWIC::IsSupported()
{
	if (filePath_ == nullptr || filePath_->Length == 0 || System::IO::File::Exists(filePath_)==false)
	{
		if (nativeImpl_ != nullptr)
		{
			delete nativeImpl_;
			nativeImpl_ = nullptr;
		}
		return false;
	}
    pin_ptr<const wchar_t> nativePath = PtrToStringChars(filePath_);
    if (nativeImpl_)
    {
		if (nativeImpl_->isInitialized(nativePath))
		{
			return true;
		}
		delete nativeImpl_;
		nativeImpl_ = nullptr;
    }
	nativeImpl_ = new ImageReaderWICNativeImpl();

    return (nativeImpl_->Initialize(nativePath));
}

/// <summary>
/// ファイルのパスを設定し、サポートされているかどうかを返します。
/// </summary>
/// <param name="filePath"></param>
/// <returns></returns>
bool ImageReaderWIC::SetFilePath(String^ filePath)
{
	filePath_ = filePath;
	return IsSupported();
}

/// <summary>
/// 画像イメージを取得します。
/// </summary>
/// <returns></returns>
System::Drawing::Image^ ImageReaderWIC::GetImage()
{
	if (!nativeImpl_ || !nativeImpl_->isValidate())
		return nullptr;

    if(auto handle = nativeImpl_->GetImage())
        return System::Drawing::Bitmap::FromHbitmap((IntPtr)handle);
	return nullptr;
}

/// <summary>
/// 画像のサムネイルイメージを取得、または作成します。
/// </summary>
/// <param name="size"></param>
/// <returns></returns>
System::Drawing::Image^ ImageReaderWIC::GetThumbnailImage(Drawing::Size size)
{
    if (!nativeImpl_ || !nativeImpl_->isValidate())
        return nullptr;

#if 1
	auto thumbnailDrawRect = GetThumbnailDrawRect(System::Drawing::Size(nativeImpl_->width(), nativeImpl_->height()), size);
    auto thumbnailImage = nativeImpl_->GetThumbnailImage(thumbnailDrawRect.Width, thumbnailDrawRect.Height, size.Width, size.Height, thumbnailDrawRect.X, thumbnailDrawRect.Y);
    return thumbnailImage ? System::Drawing::Bitmap::FromHbitmap((IntPtr)thumbnailImage) : nullptr;
#else
    if (ThumbnailType == ThumbnailTypes::KeepAspectRatio)
    {
        float aspectRatio = static_cast<float>(nativeImpl_->width()) / nativeImpl_->height();
        int newWidth = size.Width;
        int newHeight = int(size.Width / aspectRatio);

        if (newHeight > size.Height)
        {
            newHeight = size.Height;
            newWidth = int(size.Height * aspectRatio);
        }
        int x = (size.Width - newWidth) / 2;
        int y = (size.Height - newHeight) / 2;
        auto thumbnailImage = nativeImpl_->GetThumbnailImage(newWidth, newHeight, size.Width, size.Height, x, y);
        return thumbnailImage ? System::Drawing::Bitmap::FromHbitmap((IntPtr)thumbnailImage) : nullptr;

    }
    else if (ThumbnailType == ThumbnailTypes::Centering)
    {
        auto dstRatio = size.Height / float(size.Width);
		auto srcRatio = nativeImpl_ ->height() / float(nativeImpl_->width());
        int newWidth = size.Width;
        int newHeight = size.Height;
        if (dstRatio > srcRatio)
        {
			newWidth = int(size.Height / srcRatio);
		}
		else
		{
			newHeight = int(size.Width * srcRatio);
		}

        int x = (size.Width - newWidth) / 2;
        int y = (size.Height- newHeight) / 2;
        auto thumbnailImage = nativeImpl_->GetThumbnailImage(newWidth, newHeight, size.Width, size.Height, x, y);
        return thumbnailImage ? System::Drawing::Bitmap::FromHbitmap((IntPtr)thumbnailImage) : nullptr;


    }
#endif
    return nullptr;
}

/// <summary>
/// 画像のイメージサイズを取得します。
/// </summary>
/// <returns></returns>
System::Drawing::Size ImageReaderWIC::GetImageSize()
{
    if (!nativeImpl_ || !nativeImpl_->isValidate())
    	return System::Drawing::Size(0, 0);
	return System::Drawing::Size(nativeImpl_->width(), nativeImpl_->height());
}

