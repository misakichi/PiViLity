using ShelAPIHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity
{
    public class IconStore
    {
        public ImageList smallIconList { get; private set; } = new();
        public ImageList largeIconList { get; private set; } = new();
        public ImageList jumboIconList { get; private set; } = new();

        public Dictionary<int, int> iconIndexToImageIndex = new();

        bool _useLarge;
        bool _useSmall;
        bool _useJumbo;

        public IconStore(bool useLarge = true, bool useSmall = true, bool useJumbo = false,
            Size? largeSize = null, Size? smallSize = null, Size? jumboSize = null)
        {
            if (!useLarge && !useSmall && !useSmall)
                throw new ArgumentException("all size unused.");

            _useLarge = useLarge;
            _useSmall = useSmall;
            _useJumbo = useJumbo;

            if (largeSize != null)
            {
                largeIconList.ImageSize = (Size)largeSize;
            }
            else
            {
                largeIconList.ImageSize = ShelAPIHelper.FileInfo.GetFileLargeIconFromIndex(0).Size;
            }
            if (smallSize != null)
            {
                smallIconList.ImageSize = (Size)smallSize;
            }
            else
            {
                smallIconList.ImageSize = ShelAPIHelper.FileInfo.GetFileSmallIconFromIndex(0).Size;
            }
            if (jumboSize != null)
            {
                jumboIconList.ImageSize = (Size)jumboSize;
            }
            else
            {
                jumboIconList.ImageSize = ShelAPIHelper.FileInfo.GetFileJumboIconFromIndex(0).Size;
            }
        }

        /// <summary>
        /// アイコン画像の取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetIconIndex(string path)
        {
            var isFile = File.Exists(path);
            if (!isFile && !Directory.Exists(path))
                return -1;

            if (isFile)
            {
                //大きさやキャッシュも考慮してない仮構造
                var imageReader = PluginManager.Instance.GetImageReader(path);
                if (imageReader != null)
                {
                    var image = imageReader.ReadImage(path);
                    if (image != null)
                    {
                        int imageIndex2 = -1;
                        if (_useLarge)
                        {
                            imageIndex2 = largeIconList.Images.Count;
                            largeIconList.Images.Add(image);
                        }
                        if (_useSmall)
                        {
                            imageIndex2 = smallIconList.Images.Count;
                            smallIconList.Images.Add(image);
                        }
                        if (_useJumbo)
                        {
                            imageIndex2 = jumboIconList.Images.Count;
                            jumboIconList.Images.Add(image);
                        }
                        if (imageIndex2 >= 0)
                            return imageIndex2;
                    }
                }
            }
            

            var sysIndex = ShelAPIHelper.FileInfo.GetFileIconIndex(path);
            if (iconIndexToImageIndex.TryGetValue(sysIndex, out int imageIndex))
            {
                Debug.WriteLine($"{imageIndex}");
                return imageIndex;
            }

            if (_useLarge)
            {
                var icon = ShelAPIHelper.FileInfo.GetFileLargeIconFromIndex(sysIndex);
                imageIndex = largeIconList.Images.Count;
                largeIconList.Images.Add(icon);
            }
            if (_useSmall)
            {
                var icon = ShelAPIHelper.FileInfo.GetFileSmallIconFromIndex(sysIndex);
                imageIndex = smallIconList.Images.Count;
                smallIconList.Images.Add(icon);
            }
            if (_useJumbo)
            {
                var icon = ShelAPIHelper.FileInfo.GetFileJumboIconFromIndex(sysIndex);
                imageIndex = jumboIconList.Images.Count;
                jumboIconList.Images.Add(icon);
            }
            iconIndexToImageIndex.TryAdd(sysIndex, imageIndex);


            return imageIndex;
        }

        public void Clear()
        {
            smallIconList.Images.Clear();
            largeIconList.Images.Clear();
            iconIndexToImageIndex.Clear();
        }

    }
}
