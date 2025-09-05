#include "pch.h"
#include "BitmapUtil.h"

using namespace System::Drawing;
using namespace System::Drawing::Imaging;

#pragma warning(disable : 4642)
/// <summary>
/// sourceをrgbBitmap(24bppRgbとして)とalphaBitmap(Format32bppArgbとして)に分割します。
/// </summary>
/// <param name="source"></param>
/// <param name="rgbBitmap"></param>
/// <param name="alphaBitmap"></param>
void PiVilityNative::BitmapUtil::BitmapDivineRgbAndAlpha(Bitmap^ source, Bitmap^% rgbBitmap, Bitmap^% alphaBitmap)
{
    if (source == nullptr)
        throw gcnew ArgumentNullException("source");

    int width = source->Width;
    int height = source->Height;
    int alphaWidth = (width + 3) / 4;

    // RGB部分を格納するBitmapを作成
    rgbBitmap = gcnew Bitmap(width, height, PixelFormat::Format24bppRgb);
    // アルファチャンネルを格納するBitmapを作成
    alphaBitmap = gcnew Bitmap(alphaWidth, height, PixelFormat::Format32bppArgb);

    // Bitmapデータをロック
    System::Drawing::Rectangle rect = System::Drawing::Rectangle(0, 0, width, height);
    BitmapData^ sourceData = source->LockBits(rect, ImageLockMode::ReadOnly, source->PixelFormat);
    BitmapData^ rgbData = rgbBitmap->LockBits(rect, ImageLockMode::WriteOnly, PixelFormat::Format24bppRgb);
    System::Drawing::Rectangle alphaRect = System::Drawing::Rectangle(0, 0, alphaWidth, height);
    BitmapData^ alphaData = alphaBitmap->LockBits(alphaRect, ImageLockMode::WriteOnly, PixelFormat::Format32bppArgb);

    // ピクセルバッファへのポインタを取得
    Byte* sourcePtr = (Byte*)sourceData->Scan0.ToPointer();
    Byte* rgbPtr = (Byte*)rgbData->Scan0.ToPointer();
    Byte* alphaPtr = (Byte*)alphaData->Scan0.ToPointer();

    int sourceStride = sourceData->Stride;
    int rgbStride = rgbData->Stride;
    int alphaStride = alphaData->Stride;

    // ピクセルバッファにアクセスして操作
    for (int y = 0; y < height; y++)
    {
        int sourceIndex = y * sourceStride;
		auto pSrc = sourcePtr + sourceIndex;
        auto pDstRgb = rgbPtr + y * rgbStride;
        auto pDstAlpha = alphaPtr + y * alphaStride;
        for (int x = 0; x < width; x++)
        {
            int rgbIndex = y * rgbStride + x * 3; // 3はRGBのバイト数
            int alphaIndex = y * alphaStride + (x / 4) * 4; // 4はARGBのバイト数

            // RGB部分をコピー
            *pDstRgb++ = *pSrc++;
            *pDstRgb++ = *pSrc++;
            *pDstRgb++ = *pSrc++;

            // アルファチャンネルをコピー
            *pDstAlpha++ = *pSrc++;
        }
    }

    // ビットマップデータをアンロック
    source->UnlockBits(sourceData);
    rgbBitmap->UnlockBits(rgbData);
    alphaBitmap->UnlockBits(alphaData);
}
#pragma warning(default : 4642)


/// <summary>
/// rgbBitmap(24bppRgb)とalphaBitmap(Format32bppArgb)を組み合わせて、ArgbBitmap(32bppArgb)を生成します。
/// </summary>
/// <param name="rgbBitmap"></param>
/// <param name="alphaBitmap"></param>
/// <param name="argbBitmap"></param>
void PiVilityNative::BitmapUtil::RgbAndAlphaCombineToArgb(Bitmap^ rgbBitmap, Bitmap^ alphaBitmap, Bitmap^% argbBitmap)
{
    if (rgbBitmap == nullptr)
        throw gcnew ArgumentNullException("rgbBitmap");
    if (alphaBitmap == nullptr)
        throw gcnew ArgumentNullException("alphaBitmap");

    int width = rgbBitmap->Width;
    int height = rgbBitmap->Height;
    int alphaWidth = (width + 3) / 4;

    if (alphaBitmap->Width != alphaWidth || alphaBitmap->Height != height)
        throw gcnew ArgumentException("The dimensions of the alpha bitmap are incorrect.");

    // ARGB部分を格納するBitmapを作成
    argbBitmap = gcnew Bitmap(width, height, PixelFormat::Format32bppArgb);

    // Bitmapデータをロック
    System::Drawing::Rectangle rect = System::Drawing::Rectangle(0, 0, width, height);
    BitmapData^ rgbData = rgbBitmap->LockBits(rect, ImageLockMode::ReadOnly, PixelFormat::Format24bppRgb);
    System::Drawing::Rectangle alphaRect = System::Drawing::Rectangle(0, 0, alphaWidth, height);
    BitmapData^ alphaData = alphaBitmap->LockBits(alphaRect, ImageLockMode::ReadOnly, PixelFormat::Format32bppArgb);
    BitmapData^ argbData = argbBitmap->LockBits(rect, ImageLockMode::WriteOnly, PixelFormat::Format32bppArgb);

    // ピクセルバッファへのポインタを取得
    Byte* rgbPtr = (Byte*)rgbData->Scan0.ToPointer();
    Byte* alphaPtr = (Byte*)alphaData->Scan0.ToPointer();
    Byte* argbPtr = (Byte*)argbData->Scan0.ToPointer();

    int rgbStride = rgbData->Stride;
    int alphaStride = alphaData->Stride;
    int argbStride = argbData->Stride;

    // ピクセルバッファにアクセスして操作
    for (int y = 0; y < height; y++)
    {
        int rgbIndex = y * rgbStride;
        int alphaIndex = y * alphaStride;
        int argbIndex = y * argbStride;
        auto pSrcRgb = rgbPtr + rgbIndex;
        auto pSrcAlpha = alphaPtr + alphaIndex;
        auto pDstArgb = argbPtr + argbIndex;
        for (int x = 0; x < width; x++)
        {
            // RGB部分をコピー
            *pDstArgb++ = *pSrcRgb++;
            *pDstArgb++ = *pSrcRgb++;
            *pDstArgb++ = *pSrcRgb++;

            // アルファチャンネルをコピー
            *pDstArgb++ = *pSrcAlpha++;
        }
    }

    // ビットマップデータをアンロック
    rgbBitmap->UnlockBits(rgbData);
    alphaBitmap->UnlockBits(alphaData);
    argbBitmap->UnlockBits(argbData);
}
