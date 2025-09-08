using PiViLityCore.Option;
using PiViLityCore.Plugin;
using PiViLityCore.Shell;
using Sharpen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Media.Audio;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PiViLity.Viewer
{
    /// <summary>
    /// 画像ビューワー
    /// </summary>
    public partial class ImagePreViewer :
        Panel,
        IPreViewer
    {
        private float _drawScale = 1.0f;
        private Point _drawOffset = new(0, 0);
        private Image? _viewImage = new Bitmap(1, 1);
        private string _filePath = string.Empty;

        public event EventHandler? FileLoaded;
        public IEnumerable<ToolStripItem> ToolBarItems { get => []; }
        public IEnumerable<ToolStripItem> StatusBarItems { get=>[]; }


        [DefaultValue(ViewModeStyle.AutoScale)]
        public ViewModeStyle ViewMode { get; set; } = ViewModeStyle.AutoScale;

        public ViewType SupportViewType => ViewType.Image;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path { get => _filePath; protected set => _filePath = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TargetName => "Standard Image Viewer";

        Brush? _backGroundBrush = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImagePreViewer()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImagePreViewer));
            byte[]? back = resources.GetObject("DefaultBackGround") as byte[];
            if (back != null)
            {
                using (MemoryStream ms = new MemoryStream(back))
                {
                    var backImg = Image.FromStream(ms);
                    if (backImg != null)
                    {
                        var tbrush = new TextureBrush(backImg);
                        tbrush.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                        _backGroundBrush = tbrush;
                    }
                }
            }

            InitializeComponent();
            
            picImage.Dock = DockStyle.Fill;
            picImage.Paint += picImage_Paint;

        }

        /// <summary>
        /// if directoryFilesDualterator detects file change, this event is called.
        /// reload file if it still exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirectoryFilesDualterator_FileChanged(object? sender, FileChangeEventArgs e)
        {
            if(File.Exists(e.FullPath))
            {
                LoadFile(e.FullPath);
            }
        }

        /// <summary>
        /// Paintイベントで画像を描画します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picImage_Paint(object? sender, PaintEventArgs e)
        {
            if (_viewImage != null)
            {
                Rectangle dst = Rectangle.Empty;
                Rectangle src = Rectangle.Empty;
                //自動スケールの場合ウィンドウに合わせる
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                dst = new Rectangle(_drawOffset, new Size((int)(_viewImage.Width * _drawScale), (int)(_viewImage.Height * _drawScale)));
                src = new Rectangle(0, 0, _viewImage.Width, _viewImage.Height);

                if (dst != Rectangle.Empty && src != Rectangle.Empty)
                {
                    //範囲に合わせて背景色と画像を描画
                    int y = dst.Y - _drawOffset.Y;
                    int x = dst.X - _drawOffset.X;
                    x = (x & (~31)) + _drawOffset.X; 
                    y = (y & (~31)) + _drawOffset.Y;

                    Rectangle baseRectangle = new Rectangle(x, y, dst.Width, dst.Height);
                    if (_backGroundBrush != null)
                    {
                        e.Graphics.FillRectangle(_backGroundBrush, baseRectangle);
                    }
                    e.Graphics.DrawImage(_viewImage
                        , dst
                        , src
                        , GraphicsUnit.Pixel
                    );
                }
            }
        }

        /// <summary>
        /// Loads an image file from the specified file path and sets it as the current image.
        /// </summary>
        /// <remarks>This method attempts to load the image using the appropriate image reader provided by
        /// the plugin manager. If the image is successfully loaded, the <see cref="FileLoaded"/> event is raised, and
        /// the image is set as the current image.</remarks>
        /// <param name="filePath">The full path to the image file to load. This cannot be null or empty.</param>
        /// <returns><see langword="true"/> if the image was successfully loaded; otherwise, <see langword="false"/>.</returns>
        public bool LoadFile(string filePath)
        {
            //load image using plugin.
            Image? image = null;
            using (var imageReader = PluginManager.Instance.GetImageReader(filePath))
            {
                if (imageReader?.SetFilePath(filePath) ?? false)
                {
                    image = imageReader.GetPreviewImage();
                }
            }

            Path = filePath;
            FileLoaded?.Invoke(this, new());
            SetImage(image);
            return image != null;
        }

        /// <summary>
        /// 自動スケールの調整を行います。
        /// </summary>
        void adjustAutoScale()
        {
            if (_viewImage == null)
                return;

            //自動スケールの場合はウィンドウに合わせてスケールを調整
            pnlContainer.AutoScroll = false;
            picImage.Dock = DockStyle.Fill;
            pnlContainer.PerformLayout();
            _drawScale = (float)picImage.Width / _viewImage.Width;
            if (_viewImage.Height * _drawScale <= picImage.Height)
            {
                _drawOffset.X = 0;
                _drawOffset.Y = (int)((picImage.Height - _viewImage.Height * _drawScale) / 2);
            }
            else
            {
                _drawScale = (float)picImage.Height / _viewImage.Height;
                _drawOffset.X = (int)((picImage.Width - _viewImage.Width * _drawScale) / 2);
                _drawOffset.Y = 0;
            }

            //setStatus();
            picImage.Refresh();
        }

        /// <summary>
        /// 画像を設定します。
        /// </summary>
        /// <param name="image"></param>
        private void SetImage(Image? image)
        {
            _viewImage = image;
            adjustAutoScale();
        }

        /// <summary>
        /// サイズ変更イベントで自動スケールを調整します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picImage_SizeChanged(object? sender, EventArgs e)
        {
            adjustAutoScale();
        }

        public Control GetPreViewer()
        {
            return this;
        }

    }
}
