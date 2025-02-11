#pragma once

using namespace System;
using namespace System::Drawing;


namespace ShelAPIHelper
{
	public ref class BitmapUtil
	{
	public:
		/// <summary>
		/// source��rgbBitmap(24bppRgb�Ƃ���)��alphaBitmap(Format32bppArgb�Ƃ���)�ɕ������܂��B
		/// alphaBitmap��1�s�N�Z����4�s�N�Z���̃A���t�@���i�[���܂��B
		/// source��1555Argb, 32bppArgb, 64bppArgb�̂����ꂩ�ł���K�v������܂��B
		/// </summary>
		/// <param name="source"></param>
		/// <param name="rgbBitmap"></param>
		/// <param name="alphaBitmap"></param>
		static void BitmapDivineRgbAndAlpha(Bitmap^ source, Bitmap^% rgbBitmap, Bitmap^% alphaBitmap);
		static void RgbAndAlphaCombineToArgb(Bitmap^ rgbBitmap, Bitmap^ alphaBitmap, Bitmap^% argbBitmap);
	};

}