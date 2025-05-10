using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Media.Audio;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PiViLity.Viewer
{
    public partial class ImageViewer : UserControl, PiViLityCore.Plugin.ImageViewer
    {
        private float _drawScale = 1.0f;
        private Point _drawOffset = new(0, 0);
        private bool _requestCentering = false;
        private Image? _viewImage = new Bitmap(1, 1);

        [DefaultValue(ViewModeStyle.AutoScale)]
        public ViewModeStyle ViewMode { get; set; } = ViewModeStyle.AutoScale;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripStatusLabel ResolutionStatus { get; private set; } = new();
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripStatusLabel ScaleStatus { get; private set; } = new();

        public ImageViewer()
        {
            InitializeComponent();

            toolStrip1.Renderer = new ToolStripProfessionalRenderer();

            // 
            // lblResolution
            // 
            ResolutionStatus.AutoSize = false;
            ResolutionStatus.BorderSides = ToolStripStatusLabelBorderSides.Right;
            ResolutionStatus.BorderStyle = Border3DStyle.Etched;
            ResolutionStatus.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ResolutionStatus.Name = "lblResolution";
            ResolutionStatus.Size = new Size(120, 17);
            ResolutionStatus.Text = "999999 x 999999";
            // 
            // lblScale
            // 
            ScaleStatus.AutoSize = false;
            ScaleStatus.BorderSides = ToolStripStatusLabelBorderSides.Right;
            ScaleStatus.BorderStyle = Border3DStyle.Etched;
            ScaleStatus.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ScaleStatus.Name = "lblScale";
            ScaleStatus.Size = new Size(80, 17);
            ScaleStatus.Text = "100%";

            picImage.Dock = DockStyle.Fill;
            picImage.Paint += PnlImage_Paint;
            DoubleBuffered = true;
        }

        /// <summary>
        /// Paintイベントで画像を描画します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PnlImage_Paint(object? sender, PaintEventArgs e)
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
                //自動スケールの場合ウィンドウに合わせる
                if (ViewMode == ViewModeStyle.AutoScale)
                {
                    e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                    e.Graphics.DrawImage(_viewImage
                        , new Rectangle(_drawOffset, new Size((int)(_viewImage.Width * _drawScale), (int)(_viewImage.Height * _drawScale)))
                        , new Rectangle(0, 0, _viewImage.Width, _viewImage.Height)
                        , GraphicsUnit.Pixel
                    );
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
                    var dst = new Rectangle(x,y,w,h);
                    var src = new Rectangle((int)(x / _drawScale), (int)(y / _drawScale), (int)(w / _drawScale), (int)(h / _drawScale));

                    e.Graphics.DrawImage(_viewImage
                        , dst
                        , src
                        , GraphicsUnit.Pixel
                    );
                }
            }
        }

        public bool LoadImage(string filePath)
        {
            Image? image = null;
            using (var imageReader = PluginManager.Instance.GetImageReader(filePath))
            {
                if (imageReader?.SetFilePath(filePath) ?? false)
                {
                    image = imageReader.GetImage();
                }
            }
            SetImage(image);
            return image != null;
        }

        void adjustAutoScale()
        {
            if (_viewImage == null)
                return;

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

        private void SetImage(Image? image)
        {
            _viewImage = image;
            if (ViewMode == ViewModeStyle.Fixed)
            {
                _drawScale = 1.0f;
                _requestCentering = true;
            }
            adjustAutoScale();
        }

        public Control GetViewer()
        {
            // Return the control for displaying the image
            return this;
        }

        private void picImage_SizeChanged(object sender, EventArgs e)
        {
            adjustAutoScale();
        }

        Point _dragStartPosition = Point.Empty;
        Point _dragStartScrollPosition = Point.Empty;
        bool _dargScrolling = false;
        private void picImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ViewMode == ViewModeStyle.Fixed)
                {
                    _dragStartPosition = picImage.PointToScreen(e.Location);
                    _dragStartScrollPosition = pnlContainer.AutoScrollPosition;
                    picImage.Capture = true;
                    _dargScrolling = true;
                }
            }
        }

        private void picImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (_dargScrolling)
            {
                picImage.Capture = false;
                _dargScrolling = false;
                _dragStartPosition = Point.Empty;
                _dragStartScrollPosition = Point.Empty;
            }
        }
        private void picImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dargScrolling)
            {
                var nowPt = picImage.PointToScreen(e.Location);
                var offsetX = nowPt.X - _dragStartPosition.X;
                var offsetY = nowPt.Y - _dragStartPosition.Y;
                var dx = -_dragStartScrollPosition.X - offsetX;
                var dy = -_dragStartScrollPosition.Y - offsetY;
                pnlContainer.AutoScrollPosition = new Point(dx, dy);
            }
        }

        private void picImage_MouseHWheel(object sender, MouseEventArgs e)
        {
            var delta = e.Delta / 120;
            pnlContainer.AutoScrollPosition = new Point(
                -pnlContainer.AutoScrollPosition.X + delta * pnlContainer.Width / 10,
                -pnlContainer.AutoScrollPosition.Y
                );
        }

        private void tbtnFitSize_Click(object sender, EventArgs e)
        {
            ViewMode = ViewModeStyle.AutoScale;
            adjustAutoScale();
        }

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

        private void tbtnZoomOut_Click(object sender, EventArgs e)
        {
            ViewMode = ViewModeStyle.Fixed;
            zoomControl(-1);            
        }

        private void tbtnZoom100_Click(object sender, EventArgs e)
        {
            ViewMode = ViewModeStyle.Fixed;
            _drawScale = 1.0f;
            adjustAutoScale();
        }

        private void tbtnZoomIn_Click(object sender, EventArgs e)
        {
            ViewMode = ViewModeStyle.Fixed;
            zoomControl(1);
        }

        private void setStatus()
        {
            ResolutionStatus.Text = $"{_viewImage?.Width} x {_viewImage?.Height}";
            ScaleStatus.Text = $"{Math.Round(_drawScale * 100, 1)} %";
        }



    }
}
