#pragma once

using namespace System;
using namespace System::Drawing;


namespace ShelAPIHelper
{
	public ref class BitmapUtil
	{
	public:
		/// <summary>
		/// sourceをrgbBitmap(24bppRgbとして)とalphaBitmap(Format32bppArgbとして)に分割します。
		/// alphaBitmapは1ピクセルに4ピクセルのアルファを格納します。
		/// sourceは1555Argb, 32bppArgb, 64bppArgbのいずれかである必要があります。
		/// </summary>
		/// <param name="source"></param>
		/// <param name="rgbBitmap"></param>
		/// <param name="alphaBitmap"></param>
		static void BitmapDivineRgbAndAlpha(Bitmap^ source, Bitmap^% rgbBitmap, Bitmap^% alphaBitmap);
		static void RgbAndAlphaCombineToArgb(Bitmap^ rgbBitmap, Bitmap^ alphaBitmap, Bitmap^% argbBitmap);
	};

}