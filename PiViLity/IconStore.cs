using ShelAPIHelper;
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
    public class IconStore
    {
        public ImageList SmallIconList { get; private set; } = new();
        public ImageList LargeIconList { get; private set; } = new();
        public ImageList JumboIconList { get; private set; } = new();

        Dictionary<int, int> iconIndexToImageIndex = new();
        
        bool _useLarge;
        bool _useSmall;
        bool _useJumbo;
        Image dummyImage = new Bitmap(1, 1);

        bool _abortReadThumbnail = false;
        List<Task> _readThumbnailTasks = new();


        /// <summary>
        /// イメージ登録世代
        /// </summary>
        uint _registerGeneration = 0;

        /// <summary>
        /// イメージ登録タイマー
        /// </summary>
        System.Windows.Forms.Timer registerTimer = new();

        /// <summary>
        /// イメージ登録情報
        /// </summary>
        class RegisterInfo
        {
            public uint registerGeneration;
            public Image? small;
            public Image? large;
            public Image? jumbo;
            public Action<int>? postAction;
            public string path = "";
        }
        /// <summary>
        /// イメージ登録情報キュー
        /// </summary>
        ConcurrentQueue<RegisterInfo> _registerInfos = new();

        /// <summary>
        /// イメージのリサイズ（アスペクト比を保持する）
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Image ResizeImage(string path, Image image, Size size)
        {
            if (image.Size==size)
            {
                return image;
            }

            var currentTime = DateTime.Now;
            var ratioX = (double)size.Width / image.Width;
            var ratioY = (double)size.Height / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

#if false
            var newImage = PiViLityCore.Util.WICImage.ResizeImageUsingWIC(path, new Size(newWidth, newHeight));

#else
            var newImage = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.DrawImage(image, (size.Width - newWidth) / 2, (size.Height - newHeight) / 2, newWidth, newHeight);
            }
            var processTime = DateTime.Now - currentTime;
#endif

            return newImage;
        }

        void EnqueueRegisterImage(string path, Image? smallImage, Image? largeImage, Image? jumboImage, Action<int>? postAction)
        {
            var info = new RegisterInfo()
            {
                registerGeneration = _registerGeneration,
                small = smallImage,
                large = largeImage,
                jumbo = jumboImage,
                postAction = postAction,
                path = path
            };
            //_registerInfos.Enqueue(info);
            RegisterIcon(info);


        }
        /// <summary>
        /// イメージの登録(サムネイル取得後の処理を指定する)
        /// </summary>
        /// <param name="baseImage"></param>
        /// <param name="postAction"></param>
        void EnqueueRegisterImage(string path, Image baseImage, Action<int>? postAction)
        {
            var small = _useSmall ? ResizeImage(path, baseImage, SmallIconList.ImageSize) : null;
            var large = _useLarge ? ResizeImage(path, baseImage, LargeIconList.ImageSize) : null;
            var jumbo = _useJumbo ? ResizeImage(path, baseImage, JumboIconList.ImageSize) : null;
            if (small != baseImage && large != baseImage && jumbo != baseImage)
            {
                baseImage.Dispose();
            }
            EnqueueRegisterImage(path, small, large, jumbo, postAction);
        }

        /// <summary>
        /// イメージの登録(システムアイコンインデックス取得後の処理を指定する)
        /// </summary>
        /// <param name="sysIndex"></param>
        /// <param name="postAction"></param>
        void EnqueueRegisterImageFromSys(string path, int sysIndex, Action<int>? postAction)
        {
            var small = _useSmall ? ShelAPIHelper.FileInfo.GetFileLargeIconFromIndex(sysIndex).ToBitmap() : null;
            var large = _useLarge ? ShelAPIHelper.FileInfo.GetFileLargeIconFromIndex(sysIndex).ToBitmap() : null;
            var jumbo = _useJumbo ? ShelAPIHelper.FileInfo.GetFileLargeIconFromIndex(sysIndex).ToBitmap() : null;
            EnqueueRegisterImage(path, small, large, jumbo, postAction);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="useLarge"></param>
        /// <param name="useSmall"></param>
        /// <param name="useJumbo"></param>
        /// <param name="largeSize"></param>
        /// <param name="smallSize"></param>
        /// <param name="jumboSize"></param>
        /// <exception cref="ArgumentException"></exception>
        public IconStore(bool useLarge = true, bool useSmall = true, bool useJumbo = false,
            Size? largeSize = null, Size? smallSize = null, Size? jumboSize = null)
        {
            if (!useLarge && !useSmall && !useJumbo)
                throw new ArgumentException("all size unused.");

            registerTimer.Tick += RegisterTimer_Tick;
            registerTimer.Interval = 200;
            registerTimer.Enabled = true;

            _useLarge = useLarge;
            _useSmall = useSmall;
            _useJumbo = useJumbo;

            if (largeSize != null)
            {
                LargeIconList.ImageSize = (Size)largeSize;
            }
            else
            {
                LargeIconList.ImageSize = ShelAPIHelper.FileInfo.GetFileLargeIconFromIndex(0).Size;
            }
            if (smallSize != null)
            {
                SmallIconList.ImageSize = (Size)smallSize;
            }
            else
            {
                SmallIconList.ImageSize = ShelAPIHelper.FileInfo.GetFileSmallIconFromIndex(0).Size;
            }
            if (jumboSize != null)
            {
                JumboIconList.ImageSize = (Size)jumboSize;
            }
            else
            {
                JumboIconList.ImageSize = ShelAPIHelper.FileInfo.GetFileJumboIconFromIndex(0).Size;
            }
        }

        private void RegisterIcon(string path, Image? small, Image? large, Image? jumbo, Action<int>? postAction)
        {
            int index = -1;
            if (small != null)
            {
                SmallIconList.Images.Add(small);
                index = SmallIconList.Images.Count - 1;
            }
            if (large != null)
            {
                LargeIconList.Images.Add(large);
                index = LargeIconList.Images.Count - 1;
            }
            if (jumbo != null)
            {
                JumboIconList.Images.Add(jumbo);
                index = JumboIconList.Images.Count - 1;
            }
            postAction?.Invoke(index);
        }

        private bool RegisterIcon(RegisterInfo info)
        {
            if (info.registerGeneration != _registerGeneration)
                return false;
            PiViLityCore.Global.InvokeMainThread(() =>
            {
                RegisterIcon(info.path, info.small, info.large, info.jumbo, info.postAction);
            });
            return true;
        }
        /// <summary>
        /// イメージ登録タイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterTimer_Tick(object? sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            while (_registerInfos.TryDequeue(out var info)) 
            {
                if(!RegisterIcon(info))
                    continue;

                if((DateTime.Now - startTime).TotalMilliseconds > 50)
                {
                    return;
                }
            }
            registerTimer.Enabled = false;
        }

        object _lockObj = new object();
        private void GetThumbnailAsync(string path, Action<int> postAction, bool nouseSys)
        {
            if (_abortReadThumbnail)
                return;

            try
            {
                var imageReader = PluginManager.Instance.GetImageReader(path);
                if (imageReader != null)
                {
                    if (imageReader.SetFilePath(path))
                    {
                        var small = _useSmall ? imageReader.GetThumbnailImage(SmallIconList.ImageSize) : null;
                        var large = _useLarge ? imageReader.GetThumbnailImage(LargeIconList.ImageSize) : null;
                        var jumbo = _useJumbo ? imageReader.GetThumbnailImage(JumboIconList.ImageSize) : null;
                        if ((small != null)==_useSmall &&
                            (large != null)==_useLarge && 
                            (jumbo != null)==_useJumbo)
                        {
                            EnqueueRegisterImage(path, small, large, jumbo, postAction);
                            return;
                        }
                    }
                }
                if (!nouseSys)
                {
                    var sysIndex = ShelAPIHelper.FileInfo.GetFileIconIndex(path);
                    EnqueueRegisterImageFromSys(path, sysIndex, postAction);
                }

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
        public void GetIconIndex(string path, Action<int> returnAction)
        {
            var isFile = File.Exists(path);
            if (!isFile && !Directory.Exists(path))
                return;

            _GetIconIndexSys(path, returnAction);

            //ファイルの場合はプラグインからのサムネイルを取得を試みる
            if (isFile)
            {
                Task.Run(() => GetThumbnailAsync(path, returnAction, false));
                //registerTimer.Enabled = true;
            }
        }

        public void GetIconIndexNoSys(string path, Action<int> returnAction)
        {
            var isFile = File.Exists(path);
            if (!isFile && !Directory.Exists(path))
                return;

            //ファイルの場合はプラグインからのサムネイルを取得を試みる
            if (isFile)
            {
                Task.Run(() => GetThumbnailAsync(path, returnAction, true));
                //registerTimer.Enabled = true;
            }
        }

        public void GetIconIndexNoThumbnail(string path, Action<int> returnAction)
        {
            var isFile = File.Exists(path);
            if (!isFile && !Directory.Exists(path))
                return;

            _GetIconIndexSys(path, returnAction);
        }


        void _GetIconIndexSys(string path, Action<int> returnAction)
        {
            //システムアイコンの取得
            var sysIndex = ShelAPIHelper.FileInfo.GetFileIconIndex(path);
            if (iconIndexToImageIndex.TryGetValue(sysIndex, out int imageIndex))
            {
                returnAction(imageIndex);
            }
            else
            {
                var small = _useSmall ? ShelAPIHelper.FileInfo.GetFileSmallIconFromIndex(sysIndex).ToBitmap() : null;
                var large = _useLarge ? ShelAPIHelper.FileInfo.GetFileLargeIconFromIndex(sysIndex).ToBitmap() : null;
                var jumbo = _useJumbo ? ShelAPIHelper.FileInfo.GetFileJumboIconFromIndex(sysIndex).ToBitmap() : null;
                RegisterIcon(path, small, large, jumbo, imageIndex =>
                {
                    iconIndexToImageIndex[sysIndex] = imageIndex;
                    returnAction(imageIndex);
                });
            }

        }


        /// <summary>
        /// アイコン画像クリア
        /// </summary>
        public void Clear()
        {
            SmallIconList.Images.Clear();
            LargeIconList.Images.Clear();
            JumboIconList.Images.Clear();
            iconIndexToImageIndex.Clear();

            GC.Collect();

            _registerGeneration++;

            _abortReadThumbnail = true;
            Task.WaitAll(_readThumbnailTasks);
            _abortReadThumbnail = false;
        }

    }
}
