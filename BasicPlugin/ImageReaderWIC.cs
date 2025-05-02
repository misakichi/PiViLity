using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

#if false
#if true
using DirectXTexNet;

namespace BasicPlugin
{
    /// <summary>
    /// WICを使用して画像を読み込むクラス
    /// </summary>
    public class ImageReaderWIC : ImageReaderBase
    {
        private string? filePath;
        private TexMetadata? metadata;

        static ImageReaderWIC()
        {
        }

        /// <summary>
        /// 当リーダークラスがサポートする画像ファイルの拡張子リストを取得します。
        /// </summary>
        /// <returns></returns>
        public override List<string> GetSupportedExtensions()
        {
            return new List<string> { "jpg", "jpeg", "bmp", "png" };
        }

        /// <summary>
        /// このプラグインが指定したファイルをサポートするかどうかを返します。
        /// </summary>
        /// <returns></returns>
        public override bool IsSupported()
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                metadata = null;
                return false;
            }

            var helper = TexHelper.Instance;
            metadata = helper.GetMetadataFromWICFile(filePath, WIC_FLAGS.NONE);
            return metadata != null;
        }

        /// <summary>
        /// ファイルのパスを設定し、サポートされているかどうかを返します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public override bool SetFilePath(string filePath)
        {
            this.filePath = filePath;
            return IsSupported();
        }

        /// <summary>
        /// 画像イメージを取得します。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Image? GetImage()
        {
            if (metadata == null || string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            var helper = TexHelper.Instance;
            var scratchImage = helper.LoadFromWICFile(filePath, WIC_FLAGS.NONE);

            if (scratchImage == null)
            {
                return null;
            }

            var imageData = scratchImage.GetImage(0, 0, 0);
            if (imageData == null)
            {
                return null;
            }

            // Create a Bitmap from the image data
            return new System.Drawing.Bitmap(imageData.Width, imageData.Height, (int)imageData.RowPitch, System.Drawing.Imaging.PixelFormat.Format32bppArgb, imageData.Pixels);
        }

        /// <summary>
        /// 画像のサムネイルイメージを取得、または作成します。
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public override System.Drawing.Image? GetThumbnailImage(System.Drawing.Size size)
        {
            if (metadata == null || string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            var helper = TexHelper.Instance;
            ScratchImage? thumbnailImage = null;

            // サムネイルの品質に応じた処理（未実装部分をそのまま保持）
            // if (ThumbnailQuality == ThumbnailQualities.UseThumbnail)
            // {
            //     // 後で実装
            // }

            if (thumbnailImage == null)
            {
                var scratchImage = helper.LoadFromWICFile(filePath, WIC_FLAGS.NONE);
                if (scratchImage == null)
                {
                    return null;
                }
                thumbnailImage = scratchImage;
            }

            var imageData = thumbnailImage.GetImage(0, 0, 0);
            if (imageData == null)
            {
                return null;
            }

            //var finalImage = new DirectXTexNet.Image(size.Width, size.Height, DirectXTexNet.DXGI_FORMAT.B8G8R8A8_UNORM, size.Width * 4, size.Width * size.Height * 4, IntPtr(pixels), nullptr);
            var finalImage = new DirectXTexNet.Image(size.Width, size.Height, DirectXTexNet.DXGI_FORMAT.B8G8R8A8_UNORM, size.Width * 4, size.Width * size.Height * 4, 0, null);

            if (ThumbnailType == ThumbnailTypes.KeepAspectRatio)
            {
                // アスペクト比を維持してリサイズ
                float aspectRatio = (float)metadata.Width / metadata.Height;
                int newWidth = size.Width;
                int newHeight = (int)(size.Width / aspectRatio);

                if (newHeight > size.Height)
                {
                    newHeight = size.Height;
                    newWidth = (int)(size.Height * aspectRatio);
                }

                var resizedImage = thumbnailImage.Resize(0, newWidth, newHeight, TEX_FILTER_FLAGS.DEFAULT);
                helper.CopyRectangle(resizedImage.GetImage(0, 0, 0), 0, 0, newWidth, newHeight, finalImage, TEX_FILTER_FLAGS.DEFAULT, (size.Width - newWidth) / 2, (size.Height - newHeight) / 2);
            }
            else if (ThumbnailType == ThumbnailTypes.Centering)
            {
                // 中央に配置
                var dstRatio = (float)size.Height / size.Width;
                var srcRatio = (float)metadata.Height / metadata.Width;

                int newWidth = size.Width;
                int newHeight = size.Height;

                if (dstRatio > srcRatio)
                {
                    newWidth = (int)(size.Height / srcRatio);
                }
                else
                {
                    newHeight = (int)(size.Width * srcRatio);
                }

                var resizedImage = thumbnailImage.Resize(0, newWidth, newHeight, TEX_FILTER_FLAGS.DEFAULT);
                helper.CopyRectangle(resizedImage.GetImage(0, 0, 0), (newWidth - size.Width) / 2, (newHeight - size.Height) / 2, size.Width, size.Height, finalImage, TEX_FILTER_FLAGS.DEFAULT, 0, 0);
            }

            return new System.Drawing.Bitmap(finalImage.Width, finalImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// 画像のイメージサイズを取得します。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Size GetImageSize()
        {
            if (metadata == null)
            {
                return new System.Drawing.Size(0, 0);
            }
            return new System.Drawing.Size((int)metadata.Width, (int)metadata.Height);
        }
    }
}

#else
namespace BasicPlugin
{
    /// <summary>
    /// WICを使用して画像を読み込むクラス
    /// </summary>
    public class ImageReaderWIC : ImageReaderBase
    {
        private string? filePath;
        private Size imageSize;

        /// <summary>
        /// 当リーダークラスがサポートする画像ファイルの拡張子リストを取得します。
        /// </summary>
        /// <returns></returns>
        public override List<string> GetSupportedExtensions()
        {
            return new List<string> { "jpg", "jpeg", "bmp", "png" };
        }

        /// <summary>
        /// このプラグインが指定したファイルをサポートするかどうかを返します。
        /// </summary>
        /// <returns></returns>
        public override bool IsSupported()
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                imageSize = new Size(0, 0);
                return false;
            }

            try
            {
                using (var image = Image.FromFile(filePath))
                {
                    imageSize = new Size(image.Width, image.Height);
                }
                return true;
            }
            catch
            {
                imageSize = new Size(0, 0);
                return false;
            }
        }

        /// <summary>
        /// ファイルのパスを設定し、サポートされているかどうかを返します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public override bool SetFilePath(string filePath)
        {
            this.filePath = filePath;
            return IsSupported();
        }

        /// <summary>
        /// 画像イメージを取得します。
        /// </summary>
        /// <returns></returns>
        public override Image? GetImage()
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            try
            {
                return Image.FromFile(filePath);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 画像のサムネイルイメージを取得、または作成します。
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public override Image? GetThumbnailImage(Size size)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            try
            {
                using (var image = Image.FromFile(filePath))
                {
                    var thumbnail = new Bitmap(size.Width, size.Height);
                    using (var graphics = Graphics.FromImage(thumbnail))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        if (ThumbnailType == ThumbnailTypes.KeepAspectRatio)
                        {
                            // 元のアスペクト比を維持してサムネイルを作成
                            float aspectRatio = (float)image.Width / image.Height;
                            int newWidth = size.Width;
                            int newHeight = (int)(size.Width / aspectRatio);

                            if (newHeight > size.Height)
                            {
                                newHeight = size.Height;
                                newWidth = (int)(size.Height * aspectRatio);
                            }

                            int x = (size.Width - newWidth) / 2;
                            int y = (size.Height - newHeight) / 2;

                            graphics.Clear(Color.Transparent);
                            graphics.DrawImage(image, x, y, newWidth, newHeight);
                        }
                        else if (ThumbnailType == ThumbnailTypes.Centering)
                        {
                            // サムネイルを中央に配置
                            float srcRatio = (float)image.Width / image.Height;
                            float dstRatio = (float)size.Width / size.Height;

                            int newWidth = size.Width;
                            int newHeight = size.Height;

                            if (dstRatio > srcRatio)
                            {
                                newWidth = (int)(size.Height * srcRatio);
                            }
                            else
                            {
                                newHeight = (int)(size.Width / srcRatio);
                            }

                            int x = (size.Width - newWidth) / 2;
                            int y = (size.Height - newHeight) / 2;

                            graphics.Clear(Color.Transparent);
                            graphics.DrawImage(image, x, y, newWidth, newHeight);
                        }
                    }
                    return thumbnail;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 画像のイメージサイズを取得します。
        /// </summary>
        /// <returns></returns>
        public override Size GetImageSize()
        {
            return imageSize;
        }
    }
}
#endif
#endif