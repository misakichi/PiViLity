namespace PiViLity.Dock
{
    partial class FileListViewContent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileListViewContent));
            _toolStrip = new ToolStrip();
            _parentDirectoryBtn = new ToolStripButton();
            _previousDirectoryBtn = new ToolStripButton();
            _nextDirectoryBtn = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            _btnSmallIconView = new ToolStripButton();
            _btnLargeIconView = new ToolStripButton();
            _btnListView = new ToolStripButton();
            _btnDetailView = new ToolStripButton();
            _btnTileView = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            _flvPanel = new Panel();
            _toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // _toolStrip
            // 
            _toolStrip.Items.AddRange(new ToolStripItem[] { _parentDirectoryBtn, _previousDirectoryBtn, _nextDirectoryBtn, toolStripSeparator1, _btnSmallIconView, _btnLargeIconView, _btnListView, _btnDetailView, _btnTileView, toolStripSeparator2 });
            _toolStrip.Location = new Point(0, 0);
            _toolStrip.Name = "_toolStrip";
            _toolStrip.Size = new Size(456, 25);
            _toolStrip.TabIndex = 0;
            _toolStrip.Text = "toolStrip1";
            // 
            // _parentDirectoryBtn
            // 
            _parentDirectoryBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _parentDirectoryBtn.ImageTransparentColor = Color.Magenta;
            _parentDirectoryBtn.Name = "_parentDirectoryBtn";
            _parentDirectoryBtn.Size = new Size(23, 22);
            _parentDirectoryBtn.Text = "↑";
            // 
            // _previousDirectoryBtn
            // 
            _previousDirectoryBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _previousDirectoryBtn.ImageTransparentColor = Color.Magenta;
            _previousDirectoryBtn.Name = "_previousDirectoryBtn";
            _previousDirectoryBtn.Size = new Size(23, 22);
            _previousDirectoryBtn.Text = "←";
            // 
            // _nextDirectoryBtn
            // 
            _nextDirectoryBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _nextDirectoryBtn.ImageTransparentColor = Color.Magenta;
            _nextDirectoryBtn.Name = "_nextDirectoryBtn";
            _nextDirectoryBtn.Size = new Size(23, 22);
            _nextDirectoryBtn.Text = "→";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // _btnSmallIconView
            // 
            _btnSmallIconView.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _btnSmallIconView.Image = (Image)resources.GetObject("_btnSmallIconView.Image");
            _btnSmallIconView.ImageTransparentColor = Color.Magenta;
            _btnSmallIconView.Name = "_btnSmallIconView";
            _btnSmallIconView.Size = new Size(23, 22);
            _btnSmallIconView.Text = "toolStripButton4";
            // 
            // _btnLargeIconView
            // 
            _btnLargeIconView.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _btnLargeIconView.Image = (Image)resources.GetObject("_btnLargeIconView.Image");
            _btnLargeIconView.ImageTransparentColor = Color.Magenta;
            _btnLargeIconView.Name = "_btnLargeIconView";
            _btnLargeIconView.Size = new Size(23, 22);
            _btnLargeIconView.Text = "toolStripButton5";
            // 
            // _btnListView
            // 
            _btnListView.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _btnListView.Image = (Image)resources.GetObject("_btnListView.Image");
            _btnListView.ImageTransparentColor = Color.Magenta;
            _btnListView.Name = "_btnListView";
            _btnListView.Size = new Size(23, 22);
            _btnListView.Text = "toolStripButton6";
            // 
            // _btnDetailView
            // 
            _btnDetailView.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _btnDetailView.Image = (Image)resources.GetObject("_btnDetailView.Image");
            _btnDetailView.ImageTransparentColor = Color.Magenta;
            _btnDetailView.Name = "_btnDetailView";
            _btnDetailView.Size = new Size(23, 22);
            _btnDetailView.Text = "toolStripButton7";
            // 
            // _btnTileView
            // 
            _btnTileView.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _btnTileView.Image = (Image)resources.GetObject("_btnTileView.Image");
            _btnTileView.ImageTransparentColor = Color.Magenta;
            _btnTileView.Name = "_btnTileView";
            _btnTileView.Size = new Size(23, 22);
            _btnTileView.Text = "toolStripButton1";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // _flvPanel
            // 
            _flvPanel.Dock = DockStyle.Fill;
            _flvPanel.Location = new Point(0, 25);
            _flvPanel.Name = "_flvPanel";
            _flvPanel.Size = new Size(456, 259);
            _flvPanel.TabIndex = 1;
            // 
            // FileListViewContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(456, 284);
            Controls.Add(_flvPanel);
            Controls.Add(_toolStrip);
            Name = "FileListViewContent";
            Text = "FileListViewContent";
            Load += FileListViewContent_Load;
            _toolStrip.ResumeLayout(false);
            _toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip _toolStrip;
        private ToolStripButton _parentDirectoryBtn;
        private ToolStripButton _previousDirectoryBtn;
        private ToolStripButton _nextDirectoryBtn;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton _btnSmallIconView;
        private ToolStripButton _btnLargeIconView;
        private ToolStripButton _btnListView;
        private ToolStripButton _btnDetailView;
        private ToolStripButton _btnTileView;
        private ToolStripSeparator toolStripSeparator2;
        private Panel _flvPanel;
    }
}