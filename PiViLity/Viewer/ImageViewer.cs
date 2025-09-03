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
    public partial class ImageViewer :
        Panel,
        IImageViewer,
        IShotcutCommandSupport
    {
        private float _drawScale = 1.0f;
        private Point _drawOffset = new(0, 0);
        private bool _requestCentering = false;
        private Image? _viewImage = new Bitmap(1, 1);
        private string _filePath = string.Empty;

        public event EventHandler? FileLoaded;


        [DefaultValue(ViewModeStyle.AutoScale)]
        public ViewModeStyle ViewMode { get; set; } = ViewModeStyle.AutoScale;

        public IViewer.ViewType SupportViewType =>IViewer.ViewType.Image;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path { get => _filePath; protected set => _filePath = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TargetName => "Standard Image Viewer";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ShorcutTrigger> ShortCutTriggers { get; set; } = new();


        private PiViLityCore.Shell.DirectoryFilesDualterator directoryFilesDualterator = new();

        Brush? _backGroundBrush = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageViewer()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewer));
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
            InitializeToolStripItems();


            directoryFilesDualterator.FilterExtensions = PluginManager.Instance.SupportImageExtensions.ToArray();
            directoryFilesDualterator.FileChanged += DirectoryFilesDualterator_FileChanged;

            picImage.Dock = DockStyle.Fill;
            picImage.Paint += picImage_Paint;

            //register shortcut keys
            ShortCutTriggers.Add(new ShorcutTrigger() { Key = Keys.Oemplus, MethodName = "ZoomIn" });
            ShortCutTriggers.Add(new ShorcutTrigger() { Key = Keys.OemMinus, MethodName = "ZoomOut" });
            ShortCutTriggers.Add(new ShorcutTrigger() { Key = Keys.Control | Keys.C, MethodName = "CopyImage" });
            //resolve methods
            (this as IShotcutCommandSupport)?.ResolveCommandMethods();

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

            //センタリング要求がある場合はスクロール量を中央に合わせる
            if (_requestCentering)
            {
                var pt = new Point(Math.Max(0, (picImage.Width - pnlContainer.Width) / 2), Math.Max(0, (picImage.Height - pnlContainer.Height) / 2));
                pnlContainer.AutoScrollPosition = pt;
                _requestCentering = false;
            }
           
            if (_viewImage != null)
            {
                Rectangle dst = Rectangle.Empty;
                Rectangle src = Rectangle.Empty;
                //自動スケールの場合ウィンドウに合わせる
                if (ViewMode == ViewModeStyle.AutoScale)
                {
                    e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                    dst = new Rectangle(_drawOffset, new Size((int)(_viewImage.Width * _drawScale), (int)(_viewImage.Height * _drawScale)));
                    src = new Rectangle(0, 0, _viewImage.Width, _viewImage.Height);
                }
                //固定サイズの場合はスクロール量を考慮して描画
                else if (ViewMode == ViewModeStyle.Fixed)
                {
                    e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    var x = e.ClipRectangle.Left - 8;
                    var y = e.ClipRectangle.Top - 8;
                    var w = e.ClipRectangle.Width + 16;
                    var h = e.ClipRectangle.Height + 16;
                    dst = new Rectangle(x,y,w,h);
                    src = new Rectangle((int)(x / _drawScale), (int)(y / _drawScale), (int)(w / _drawScale), (int)(h / _drawScale));

                }
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

            //選択範囲がある場合はその部分を描画
            if (_selectRect != Rectangle.Empty)
            {
                //選択範囲の描画
                var rc = new Rectangle(
                    (int)(_selectRect.X * _drawScale) + _drawOffset.X,
                    (int)(_selectRect.Y * _drawScale) + _drawOffset.Y,
                    (int)(_selectRect.Width * _drawScale),
                    (int)(_selectRect.Height * _drawScale)
                );

                //選択範囲以外を半透明で塗りつぶす
                using (var path = new GraphicsPath())
                {
                    path.AddRectangle(e.ClipRectangle);
                    path.AddRectangle(rc);
                    path.FillMode = FillMode.Alternate;
                    using(var brush = new SolidBrush(Color.FromArgb(128, Color.Black)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
                //選択範囲の枠を点線で描画
                using (var newPen = new Pen(Color.White, 2))
                {
                    newPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                    newPen.DashPattern = [6, 6];
                    e.Graphics.DrawRectangle(Pens.Black, rc);
                    e.Graphics.DrawRectangle(newPen, rc);
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
                    image = imageReader.GetImage();
                }
            }

            Path = filePath;
            directoryFilesDualterator.FilePath = filePath;
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
            if (ViewMode == ViewModeStyle.AutoScale)
            {
                tbtnFitSize.Checked = true;
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
            }
            //固定サイズの場合はスクロール可能にして画像サイズを設定
            else
            {
                tbtnFitSize.Checked = false;
                if (picImage.Dock != DockStyle.None)
                {
                    picImage.Dock = DockStyle.None;
                    picImage.Left = 0;
                    picImage.Top = 0;
                }
                pnlContainer.AutoScroll = true;
                picImage.Width = (int)(_drawScale * _viewImage.Width);
                picImage.Height = (int)(_drawScale * _viewImage.Height);
                _drawOffset.X = 0;
                _drawOffset.Y = 0;
            }
            setStatus();
            picImage.Refresh();
        }

        /// <summary>
        /// 画像を設定します。
        /// </summary>
        /// <param name="image"></param>
        private void SetImage(Image? image)
        {
            _viewImage = image;
            if (ViewMode == ViewModeStyle.Fixed)
            {
                _drawScale = 1.0f;
                _requestCentering = true;
            }
            tbtnCopy.Enabled = image != null;
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

        Point _dragStartPosition = Point.Empty;
        Point _dragStartScrollPosition = Point.Empty;
        Rectangle _selectRect = Rectangle.Empty;
        enum DragMode
        {
            None,
            Scroll,
            Select
        }
        DragMode _dragMode = DragMode.None;

        /// <summary>
        /// マウスダウンによる各種処理
        /// 右ボタン（ドラッグ）：スクロール
        /// 左ボタン（ドラッグ）：範囲選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picImage_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ViewMode == ViewModeStyle.Fixed)
                {
                    _dragStartPosition = e.Location;
                    _dragStartScrollPosition = pnlContainer.AutoScrollPosition;
                    picImage.Capture = true;
                    _dragMode = DragMode.Scroll;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                //範囲選択の処理を追加する場合はここに実装
                if (_selectRect != Rectangle.Empty)
                {
                    _selectRect = Rectangle.Empty;
                    picImage.Refresh();
                }

                _dragStartPosition = e.Location;
                picImage.Capture = true;
                _dragMode = DragMode.Select;
            }
        }

        private void picImage_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_dragMode!=DragMode.None)
            {
                picImage.Capture = false;
                _dragMode = DragMode.None;
                _dragStartPosition = Point.Empty;
                _dragStartScrollPosition = Point.Empty;
            }
        }
        private void picImage_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_dragMode != DragMode.None)
            {
                if(_dragMode == DragMode.Scroll)
                {
                    var startScreen = picImage.PointToScreen(_dragStartPosition);
                    var nowPt = picImage.PointToScreen(e.Location);
                    var offsetX = nowPt.X - startScreen.X;
                    var offsetY = nowPt.Y - startScreen.Y;
                    var dx = -_dragStartScrollPosition.X - offsetX;
                    var dy = -_dragStartScrollPosition.Y - offsetY;

                    pnlContainer.AutoScrollPosition = new Point(dx, dy);
                }
                else if(_dragMode == DragMode.Select)
                {
#if true
                    var sx = (int)((Math.Min(_dragStartPosition.X, e.Location.X) - _drawOffset.X) / _drawScale);
                    var sy = (int)((Math.Min(_dragStartPosition.Y, e.Location.Y) - _drawOffset.Y) / _drawScale);
                    var ex = (int)((Math.Max(_dragStartPosition.X, e.Location.X) - _drawOffset.X) / _drawScale);
                    var ey = (int)((Math.Max(_dragStartPosition.Y, e.Location.Y) - _drawOffset.Y) / _drawScale);
                    sx = Math.Clamp(sx, 0, _viewImage?.Width - 1 ?? 0);
                    ex = Math.Clamp(ex, 0, _viewImage?.Width - 1 ?? 0);
                    sy = Math.Clamp(sy, 0, _viewImage?.Height - 1 ?? 0);
                    ey = Math.Clamp(ey, 0, _viewImage?.Height - 1 ?? 0);
                    _selectRect = new Rectangle(sx, sy, ex-sx, ey-sy);
#else
                    var x = Math.Clamp((int)(Math.Min(_dragStartPosition.X, e.Location.X) / _drawScale) - _drawOffset.X, 0, _viewImage?.Width - 1 ?? 0);
                    var y = Math.Clamp((int)(Math.Min(_dragStartPosition.Y, e.Location.Y) / _drawScale) - _drawOffset.Y, 0, _viewImage?.Height - 1 ?? 0);
                    var w = Math.Clamp((int)(Math.Abs(_dragStartPosition.X - e.Location.X) / _drawScale), 0, Math.Max(0, (_viewImage?.Width ?? 1) - x));
                    var h = Math.Clamp((int)(Math.Abs(_dragStartPosition.Y - e.Location.Y) / _drawScale), 0, Math.Max(0, (_viewImage?.Height ?? 1) - y));
                    _selectRect = new Rectangle(x,y,w,h);
#endif
                    picImage.Refresh();
                }
            }

            //マウスが動いたことによる情報更新
            var px = (int)(e.X / _drawScale - _drawOffset.X);
            var py = (int)(e.Y / _drawScale - _drawOffset.Y);
            px = Math.Clamp(px, 0, _viewImage?.Width -1 ?? 0);
            py = Math.Clamp(py, 0, _viewImage?.Height - 1 ?? 0);
            string pixelInfo;
            if (_viewImage is Bitmap bmp)
            {
                var color = bmp.GetPixel(px, py);
                pixelInfo = $"({px}, {py}) ARGB:{color.A,3},{color.R,3},{color.G,3},{color.B,3} (#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2})";
                tlblPixelColor.ForeColor = Color.FromArgb(255, color.R, color.G, color.B);
            }
            else
            {
                pixelInfo = $"({px}, {py})";
                tlblPixelColor.ForeColor = Color.Black;
            }
            if(_selectRect != Rectangle.Empty)
            {
                pixelInfo += $" Select:{_selectRect.Width}x{_selectRect.Height} ({_selectRect.X}, {_selectRect.Y})-({_selectRect.Right}, {_selectRect.Bottom})";
            }
            tlblPixelInfo.Text = pixelInfo;
        }

        private void picImage_MouseHWheel(object? sender, MouseEventArgs e)
        {
            var delta = e.Delta / 120;
            pnlContainer.AutoScrollPosition = new Point(
                -pnlContainer.AutoScrollPosition.X + delta * pnlContainer.Width / 10,
                -pnlContainer.AutoScrollPosition.Y
                );
        }

        private void tbtnFitSize_Click(object? sender, EventArgs e)
        {
            ViewMode = ViewModeStyle.AutoScale;
            adjustAutoScale();
        }
        
        /// <summary>
        /// ズーム制御
        /// </summary>
        /// <param name="move"></param>
        private void zoomControl(int move)
        {
            float[] zoomTable  = [0.1f, 0.333f, 0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 4.0f, 8.0f];
            var scale = _drawScale;
            if (move < 0)
            {
                for (int i = 0; i < zoomTable.Length; i++)
                {
                    if (zoomTable[i] >= _drawScale)
                    {
                        scale = zoomTable[Math.Max(0,i+move)];
                        break;
                    }
                }
            }
            else
            {
                for (int i = zoomTable.Length - 1; i >= 0; i--)
                {
                    if (zoomTable[i] <= _drawScale)
                    {
                        scale = zoomTable[Math.Min(zoomTable.Length-1,i+move)];
                        break;
                    }
                }
            }
            if(scale != _drawScale)
            {
                _drawScale = scale;
                adjustAutoScale();
            }
        }

        [ShortCutCommand(NameText="ZoomIn)")]
        void ZoomIn()
        {
            ViewMode = ViewModeStyle.Fixed;
            zoomControl(1);
        }
        [ShortCutCommand(NameText = "ZoomOut")]
        void ZoomOut()
        {
            ViewMode = ViewModeStyle.Fixed;
            zoomControl(-1);
        }
        [ShortCutCommand(NameText = "Copy")]
        void CopyImage()
        {
            if (_viewImage == null)
                return;

            if (_selectRect != Rectangle.Empty)
            {
                using (var bmp = new Bitmap(_selectRect.Width, _selectRect.Height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(_viewImage, new Rectangle(0, 0, bmp.Width, bmp.Height), _selectRect, GraphicsUnit.Pixel);
                    }
                    Clipboard.SetImage(bmp);
                }
            }
            else
            {
                Clipboard.SetImage(_viewImage);
            }
        }

        private void tbtnZoomOut_Click(object? sender, EventArgs e)
        {
            ZoomOut();
        }

        private void tbtnZoom100_Click(object? sender, EventArgs e)
        {
            ViewMode = ViewModeStyle.Fixed;
            _drawScale = 1.0f;
            adjustAutoScale();
        }

        private void tbtnZoomIn_Click(object? sender, EventArgs e)
        {
            ZoomIn();
        }

        private void tbtnCopy_Click(object? sender, EventArgs e)
        {
            CopyImage();
        }

        /// <summary>
        /// ステータスバーの更新
        /// </summary>
        private void setStatus()
        {
            tlblResolutionStatus.Text = $"{_viewImage?.Width} x {_viewImage?.Height}";
            tlblScaleStatus.Text = $"{Math.Round(_drawScale * 100, 1)} %";
        }

        public Control GetViewer()
        {
            return this;
        }

        /// <summary>
        /// 次のファイルへ
        /// </summary>
        public bool NextFile() => directoryFilesDualterator.MoveNext();

        /// <summary>
        /// 前のファイルへ
        /// </summary>
        /// <returns></returns>
        public bool PreviousFile() => directoryFilesDualterator.MovePrevious();

        /// <summary>
        /// 最初ののファイルへ
        /// </summary>
        public bool FirstFile() => directoryFilesDualterator.MoveFirst();

        /// <summary>
        /// 最後のファイルへ
        /// </summary>
        /// <returns></returns>
        public bool LastFile() => directoryFilesDualterator.MoveLast();
    }
}
