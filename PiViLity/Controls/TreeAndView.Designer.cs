using System.Windows.Forms;

namespace PiViLity.Controls
{
    partial class TreeAndView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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
            components = new System.ComponentModel.Container();
            tvwDirMain = new TreeView();
            splitDirView = new Splitter();
            panelViewInfo = new Panel();
            panelInfo = new Panel();
            splitViewInfo = new Splitter();
            lsvFile = new FileListView();
            toolStrip = new ToolStrip();
            panelViewInfo.SuspendLayout();
            SuspendLayout();
            // 
            // tvwDirMain
            // 
            tvwDirMain.AllowDrop = true;
            tvwDirMain.BorderStyle = BorderStyle.None;
            tvwDirMain.Dock = DockStyle.Left;
            tvwDirMain.FullRowSelect = true;
            tvwDirMain.HotTracking = true;
            tvwDirMain.Location = new Point(0, 25);
            tvwDirMain.Name = "tvwDirMain";
            tvwDirMain.ShowLines = false;
            tvwDirMain.Size = new Size(226, 423);
            tvwDirMain.TabIndex = 4;
            tvwDirMain.ItemDrag += tvwDirMain_ItemDrag;
            tvwDirMain.DragDrop += tvwDirMain_DragDrop;
            tvwDirMain.DragEnter += tvwDirMain_DragEnter;
            tvwDirMain.DragOver += tvwDirMain_DragOver;
            tvwDirMain.DragLeave += tvwDirMain_DragLeave;
            tvwDirMain.MouseClick += tvwDirMain_MouseClick;
            // 
            // splitDirView
            // 
            splitDirView.Location = new Point(226, 25);
            splitDirView.Name = "splitDirView";
            splitDirView.Size = new Size(3, 423);
            splitDirView.TabIndex = 6;
            splitDirView.TabStop = false;
            // 
            // panelViewInfo
            // 
            panelViewInfo.Controls.Add(panelInfo);
            panelViewInfo.Controls.Add(splitViewInfo);
            panelViewInfo.Controls.Add(lsvFile);
            panelViewInfo.Dock = DockStyle.Fill;
            panelViewInfo.Location = new Point(229, 25);
            panelViewInfo.Name = "panelViewInfo";
            panelViewInfo.Size = new Size(458, 423);
            panelViewInfo.TabIndex = 13;
            // 
            // panelInfo
            // 
            panelInfo.BorderStyle = BorderStyle.Fixed3D;
            panelInfo.Dock = DockStyle.Fill;
            panelInfo.Location = new Point(0, 372);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new Size(458, 51);
            panelInfo.TabIndex = 15;
            // 
            // splitViewInfo
            // 
            splitViewInfo.Dock = DockStyle.Top;
            splitViewInfo.Location = new Point(0, 369);
            splitViewInfo.Name = "splitViewInfo";
            splitViewInfo.Size = new Size(458, 3);
            splitViewInfo.TabIndex = 13;
            splitViewInfo.TabStop = false;
            // 
            // lsvFile
            // 
            lsvFile.Activation = ItemActivation.OneClick;
            lsvFile.AllowDrop = true;
            lsvFile.BorderStyle = BorderStyle.None;
            lsvFile.Dock = DockStyle.Top;
            lsvFile.HotTracking = true;
            lsvFile.HoverSelection = true;
            lsvFile.Location = new Point(0, 0);
            lsvFile.Name = "lsvFile";
            lsvFile.Size = new Size(458, 369);
            lsvFile.Sorting = SortOrder.Descending;
            lsvFile.TabIndex = 12;
            lsvFile.TileSize = new Size(320, 240);
            lsvFile.UseCompatibleStateImageBehavior = false;
            lsvFile.ColumnClick += lsvFile_ColumnClick;
            lsvFile.ItemDrag += lsvFile_ItemDrag;
            lsvFile.DragDrop += lsvFile_DragDrop;
            lsvFile.DragEnter += lsvFile_DragEnter;
            lsvFile.DragOver += lsvFile_DragOver;
            lsvFile.DragLeave += lsvFile_DragLeave;
            lsvFile.MouseClick += lsvFile_MouseClick;
            // 
            // toolStrip
            // 
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(687, 25);
            toolStrip.TabIndex = 0;
            toolStrip.Text = "toolStrip";
            // 
            // TreeAndView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panelViewInfo);
            Controls.Add(splitDirView);
            Controls.Add(tvwDirMain);
            Controls.Add(toolStrip);
            DoubleBuffered = true;
            Name = "TreeAndView";
            Size = new Size(687, 448);
            panelViewInfo.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView tvwDirMain;
        private Splitter splitDirView;
        private Panel panelViewInfo;
        private Panel panelInfo;
        private Splitter splitViewInfo;
        private FileListView lsvFile;
        private ToolStrip toolStrip;
    }
}