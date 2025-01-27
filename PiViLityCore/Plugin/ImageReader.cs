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
        ResizeImage,
    }

    /// <summary>
    /// 画像ファイルを読み込むためのインターフェースです。
    /// </summary>
    public interface IImageReader
    {
        /// <summary>
        /// このプラグインがサポートする拡張子を返します。
        /// </summary>
        /// <returns></returns>
        public List<string> GetSupportExtensions();

        /// <summary>
        /// サムネイルの品質を指定します。
        /// </summary>
        public ThumbnailQualities ThumbnailQuality { get; set; }

        /// <summary>
        /// このプラグインが指定したファイルをサポートするかどうかを返します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsSupport(string filePath);

        public bool SetFilePath(string filePath);

        public System.Drawing.Image? GetImage();

        public System.Drawing.Image? GetThumbnailImage(Size size);

    }
}
