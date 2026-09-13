namespace PiViLity.Viewer
{
    partial class ImageViewer
    {
        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                directoryFilesDualterator.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            _hscrollPanel = new Panel();
            _hscroll = new HScrollBar();
            tlblResolutionStatus = new ToolStripStatusLabel();
            tlblScaleStatus = new ToolStripStatusLabel();
            _picImage = new PiViLityCore.Controls.PictureBox();
            _vscrollPanel = new Panel();
            _vscroll = new VScrollBar();
            panel2 = new Panel();
            _hscrollPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_picImage).BeginInit();
            _vscrollPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _hscrollPanel
            // 
            _hscrollPanel.Controls.Add(_hscroll);
            _hscrollPanel.Dock = DockStyle.Bottom;
            _hscrollPanel.Location = new Point(0, 295);
            _hscrollPanel.Name = "_hscrollPanel";
            _hscrollPanel.Size = new Size(437, 16);
            _hscrollPanel.TabIndex = 12;
            // 
            // _hscroll
            // 
            _hscroll.Dock = DockStyle.Fill;
            _hscroll.Location = new Point(0, 0);
            _hscroll.Name = "_hscroll";
            _hscroll.Size = new Size(437, 16);
            _hscroll.TabIndex = 12;
            _hscroll.ValueChanged += _hscroll_ValueChanged;
            _hscroll.MouseEnter += _scroll_MouseEnter;
            _hscroll.MouseLeave += _scroll_MouseLeave;
            // 
            // tlblResolutionStatus
            // 
            tlblResolutionStatus.Name = "tlblResolutionStatus";
            tlblResolutionStatus.Size = new Size(23, 23);
            // 
            // tlblScaleStatus
            // 
            tlblScaleStatus.Name = "tlblScaleStatus";
            tlblScaleStatus.Size = new Size(23, 23);
            // 
            // _picImage
            // 
            _picImage.Dock = DockStyle.Fill;
            _picImage.Location = new Point(0, 0);
            _picImage.Name = "_picImage";
            _picImage.Size = new Size(453, 311);
            _picImage.TabIndex = 9;
            _picImage.TabStop = false;
            _picImage.MouseHWheel += _picImage_MouseHWheel;
            _picImage.SizeChanged += _picImage_SizeChanged;
            _picImage.MouseDown += _picImage_MouseDown;
            _picImage.MouseLeave += _picImage_MouseLeave;
            _picImage.MouseMove += _picImage_MouseMove;
            _picImage.MouseUp += _picImage_MouseUp;
            _picImage.MouseWheel += _picImage_MouseVWheel;
            // 
            // _vscrollPanel
            // 
            _vscrollPanel.Controls.Add(_vscroll);
            _vscrollPanel.Controls.Add(panel2);
            _vscrollPanel.Dock = DockStyle.Right;
            _vscrollPanel.Location = new Point(437, 0);
            _vscrollPanel.Name = "_vscrollPanel";
            _vscrollPanel.Size = new Size(16, 311);
            _vscrollPanel.TabIndex = 10;
            // 
            // _vscroll
            // 
            _vscroll.Dock = DockStyle.Fill;
            _vscroll.Location = new Point(0, 0);
            _vscroll.Name = "_vscroll";
            _vscroll.Size = new Size(16, 295);
            _vscroll.TabIndex = 3;
            _vscroll.ValueChanged += _vscroll_ValueChanged;
            _vscroll.MouseEnter += _scroll_MouseEnter;
            _vscroll.MouseLeave += _scroll_MouseLeave;
            // 
            // panel2
            // 
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 295);
            panel2.Name = "panel2";
            panel2.Size = new Size(16, 16);
            panel2.TabIndex = 0;
            // 
            // ImageViewer
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(_hscrollPanel);
            Controls.Add(_vscrollPanel);
            Controls.Add(_picImage);
            Name = "ImageViewer";
            Size = new Size(453, 311);
            _hscrollPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_picImage).EndInit();
            _vscrollPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PiViLityCore.Controls.PictureBox _picImage;
        private Panel _vscrollPanel;
        private VScrollBar _vscroll;
        private Panel panel2;
        private HScrollBar _hscroll;
        private Panel _hscrollPanel;
    }
}
