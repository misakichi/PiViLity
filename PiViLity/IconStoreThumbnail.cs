using PiVilityNative;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity
{
    public class IconStoreThumbnail : PiViLityCore.Shell.IIconStore
    {
        List<IDisposable> _needDestroyObjects = new();

        Image dummyImage = new Bitmap(1, 1);

        /// <summary>
        /// イメージ登録世代
        /// </summary>
        uint _registerGeneration = 0;

        /// <summary>
        /// イメージ登録情報
        /// </summary>
        private class RegisterInfo
        {
            public uint registerGeneration;
            public Image? image; 
            public Action<int>? postAction;
            public string path = "";
        }
        public ImageList SmallIconList { get; private set; } = new();

        public ImageList LargeIconList { get; private set; } = new();

        public ImageList TileIconList { get; private set; } = new();

        void EnqueueRegisterImage(string path, Image? _image, Action<int>? postAction)
        {
            var info = new RegisterInfo()
            {
                registerGeneration = _registerGeneration,
                image = _image,
                postAction = postAction,
                path = path
            };

            RegisterImage(info);


        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="thumbnailSize"></param>
        /// <exception cref="ArgumentException"></exception>
        public IconStoreThumbnail(Size? thumbnailSize = null)
        {
            try
            {
                if (thumbnailSize != null)
                {
                    TileIconList.ImageSize = (Size)thumbnailSize;
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

        private void RegisterImage(string path, Image? image, Action<int>? postAction)
        {
            int RegisterImageOne(ImageList list, Image image)
            {
                _needDestroyObjects.Add(image);
                list.Images.Add(image);
                return list.Images.Count - 1;
            }

            int index = -1;
            if (image != null)
            {
                index = RegisterImageOne(TileIconList, image);
            }
            postAction?.Invoke(index);
        }


        private bool RegisterImage(RegisterInfo info)
        {
            if (info.registerGeneration != _registerGeneration)
                return false;
            PiViLityCore.Global.InvokeMainThread(() =>
            {
                if (info.registerGeneration != _registerGeneration)
                {
                    (info.image as IDisposable)?.Dispose();
                    return;
                }
                RegisterImage(info.path, info.image, info.postAction);
            });
            return true;
        }

        object _lockObj = new object();

        private void GetThumbnailAsync(PiViLityCore.Plugin.IImageReader imageReader, string path, Action<int>? postAction, bool nouseSys)
        {
            try
            {
                //var thumbnail = ThumbnailCache.Instance.GetThumbnail(path);
                //if(thumbnail != null)
                //{
                //    EnqueueRegisterImage(path, thumbnail, postAction);
                //    return;
                //}
                if (imageReader != null)
                {
                    if (imageReader.SetFilePath(path))
                    {
                        Image? image = imageReader.GetThumbnailImage(TileIconList.ImageSize);
                        if (image!=null)
                        {
                            ThumbnailCache.Instance.SetThumbnail(path, image);
                            EnqueueRegisterImage(path, image,  postAction);
                            return;
                        }
                    }
                    imageReader.Dispose();
                }
                PiViLityCore.Global.InvokeMainThread(() => postAction?.Invoke(-1));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// アイコン画像の取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns>アイコン画像。returnActionは何度か呼ばれる可能性がある</returns>
        private void GetThumbnailImage(string path, Action<int>? returnAction)
        {
            var isFile = File.Exists(path);
            if (!isFile)
                return;

            //ファイルの場合はプラグインからのサムネイルを取得を試みる
            if (isFile)
            {
                var thumbnail = ThumbnailCache.Instance.GetThumbnail(path);
                if (thumbnail != null)
                {
                    EnqueueRegisterImage(path, thumbnail, returnAction);
                    return;
                }

                var imageReader = PluginManager.Instance.GetImageReader(path);
                if (imageReader != null)
                {
                    Task.Run(() => GetThumbnailAsync(imageReader, path, returnAction, false));
                    return;
                }
            }

            returnAction?.Invoke(-1);
        }

        /// <summary>
        /// アイコン画像クリア
        /// </summary>
        public void Clear(Size? largeSize = null, Size? smallSize = null, Size? tileSize = null)
        {
            foreach (var icon in _needDestroyObjects)
            {
                icon.Dispose();
            }
            _needDestroyObjects.Clear();
            TileIconList.Images.Clear();
            if(tileSize is Size size)
                TileIconList.ImageSize = size;


            _registerGeneration++;

        }

        /// <summary>
        /// 特殊フォルダのアイコン取得
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="returnAction"></param>
        public void GetIcon(Environment.SpecialFolder specialFolder, Action<int>? returnAction)
        {
            returnAction?.Invoke(-1);
        }

        /// <summary>
        /// ファイルのアイコン取得
        /// </summary>
        /// <param name="path"></param>
        /// <param name="returnAction"></param>
        public void GetIcon(string path, Action<int>? returnAction)
        {
            GetThumbnailImage(path, returnAction);
        }

    }
}
