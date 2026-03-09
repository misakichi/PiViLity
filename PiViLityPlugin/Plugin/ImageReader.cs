using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    /// <summary>
    /// 画像ファイルのサムネイルの品質を指定します。
    /// </summary>
    public enum ThumbnailQualities
    {
        UseThumbnail,
        ResizeImage
    }

    /// <summary>
    /// サムネイルの種類を指定します。
    /// </summary>
    public enum ThumbnailTypes
    {
        Centering,      // 中央に表示（はみ出た分は切られます）
        KeepAspectRatio // アスペクト比を保持して表示（余白が生まれることがあります）
    }

    /// <summary>
    /// 画像ファイルを読み込むためのインターフェースです。
    /// </summary>
    public interface IImageReader : IReader
    {
        /// <summary>
        /// サムネイルの品質を指定します。
        /// </summary>
        public ThumbnailQualities ThumbnailQuality { get; set; }

        /// <summary>
        /// サムネイルの種類を指定します。
        /// </summary>
        public ThumbnailTypes ThumbnailType { get; set; }

        /// <summary>
        /// 画像イメージを取得します
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Image? GetImage();

        /// <summary>
        /// 画像イメージを取得します
        /// =GetImageという実装でもよいです。
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Image? GetPreviewImage() => GetImage();

        /// <summary>
        /// 画像ファイルのサムネイルを取得します。
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public System.Drawing.Image? GetThumbnailImage(Size size);

        /// <summary>
        /// 画像ファイルのサイズを取得します。
        /// </summary>
        /// <returns>サイズ</returns>
        public Size GetImageSize();

    }
}
