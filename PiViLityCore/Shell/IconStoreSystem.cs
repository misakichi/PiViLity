using PiVilityNative;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Shell
{
    public class IconStoreSystem : IIconStore
    {
        public ImageList SmallIconList { get; private set; } = new();
        public ImageList LargeIconList { get; private set; } = new();
        public ImageList TileIconList { get; private set; } = new();

        private Dictionary<int, int> iconIndexToImageIndex = new();
        private List<IDisposable> _needDestroyIcons = new();

        private bool _useLarge;
        private bool _useSmall;
        private bool _useTile;
        private Image dummyImage = new Bitmap(1, 1);

        /// <summary>
        /// イメージ登録世代
        /// </summary>
        private uint _registerGeneration = 0;

        private object _lockObj = new object();

        /// <summary>
        /// イメージ登録情報
        /// </summary>
        private class RegisterInfo
        {
            public uint registerGeneration = 0;
            public Icon? small = null;
            public Icon? large = null;
            public Icon? tile = null;
            public Action<int>? postAction = null;
            public string path = "";
        }
        /// <summary>
        /// イメージ登録情報キュー
        /// </summary>
        ConcurrentQueue<RegisterInfo> _registerInfos = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="useLarge"></param>
        /// <param name="useSmall"></param>
        /// <param name="useTile"></param>
        /// <param name="largeSize"></param>
        /// <param name="smallSize"></param>
        /// <param name="tileSize"></param>
        /// <exception cref="ArgumentException"></exception>
        public IconStoreSystem(bool useLarge = true, bool useSmall = true, bool useTile = false,
            Size? largeSize = null, Size? smallSize = null, Size? tileSize = null)
        {
            if (!useLarge && !useSmall && !useTile)
                throw new ArgumentException("all size unused.");

            _useLarge = useLarge;
            _useSmall = useSmall;
            _useTile = useTile;

            try
            {
                if (largeSize != null)
                {
                    LargeIconList.ImageSize = (Size)largeSize;
                }
                else
                {
                    using (var icon = PiVilityNative.FileInfo.GetFileLargeIconFromIndex(0))
                        LargeIconList.ImageSize = icon?.Size ?? new Size(32, 32);
                }
                if (smallSize != null)
                {
                    SmallIconList.ImageSize = (Size)smallSize;
                }
                else
                {
                    using(var icon = PiVilityNative.FileInfo.GetFileSmallIconFromIndex(0))
                        SmallIconList.ImageSize = icon?.Size ?? new Size(16, 16);
                }
                if (tileSize != null)
                {
                    TileIconList.ImageSize = (Size)tileSize;
                }
                else
                {
                    using (var icon = PiVilityNative.FileInfo.GetFileJumboIconFromIndex(0))
                        TileIconList.ImageSize = icon?.Size ?? new Size(32, 32);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Debug.WriteLine("");
        }

        private void RegisterIcon(Icon? small, Icon? large, Icon? tile, Action<int>? postAction)
        {
            int RegisterIconOne(ImageList list, Icon icon)
            {
                _needDestroyIcons.Add(icon);
                list.Images.Add(icon);
                return list.Images.Count - 1;
            }

            int index = -1;
            if (small != null)
            {
                index = RegisterIconOne(SmallIconList, small);
            }
            if (large != null)
            {
                index = RegisterIconOne(LargeIconList, large);
            }
            if (tile != null)
            {
                index = RegisterIconOne(TileIconList, tile);
            }
            postAction?.Invoke(index);
        }


        /// <summary>
        /// システムアイコンインデックスからアイコンを取得
        /// </summary>
        /// <param name="sysIndex"></param>
        /// <param name="returnAction"></param>
        protected void GetIcon(int sysIndex, Action<int>? returnAction)
        {
            if (iconIndexToImageIndex.TryGetValue(sysIndex, out int imageIndex))
            {
                returnAction?.Invoke(imageIndex);
            }
            else
            {
                ///システムアイコンの取得
                var small = _useSmall ? PiVilityNative.FileInfo.GetFileSmallIconFromIndex(sysIndex) : null;
                var large = _useLarge ? PiVilityNative.FileInfo.GetFileLargeIconFromIndex(sysIndex) : null;
                var tile = _useTile ? PiVilityNative.FileInfo.GetFileJumboIconFromIndex(sysIndex) : null;
                ///アイコン登録
                RegisterIcon(small, large, tile, imageIndex =>
                {
                    iconIndexToImageIndex[sysIndex] = imageIndex;
                    returnAction?.Invoke(imageIndex);
                });
            }
        }

        public void GetIcon(Environment.SpecialFolder specialFolder, Action<int>? returnAction)
        {
            var sysIndex = PiVilityNative.FileInfo.GetFileIconIndex(specialFolder);
            GetIcon(sysIndex, returnAction);
        }

        public void GetIcon(string path, Action<int>? returnAction)
        {
            //システムアイコンの取得
            var sysIndex = PiVilityNative.FileInfo.GetFileIconIndex(path);
            GetIcon(sysIndex, returnAction);

        }


        /// <summary>
        /// アイコン画像クリア
        /// </summary>
        public void Clear(Size? largeSize = null, Size? smallSize = null, Size? tileSize = null)
        {
            foreach (var icon in _needDestroyIcons)
            {
                icon.Dispose();
            }
            _needDestroyIcons.Clear();
            SmallIconList.Images.Clear();
            LargeIconList.Images.Clear();
            TileIconList.Images.Clear();
            iconIndexToImageIndex.Clear();

            if (largeSize != null)
            {
                LargeIconList.ImageSize = (Size)largeSize;
            }
            if (smallSize != null)
            {
                SmallIconList.ImageSize = (Size)smallSize;
            }
            if (tileSize != null)
            {
                TileIconList.ImageSize = (Size)tileSize;
            }


            _registerGeneration++;

        }

    }
}
