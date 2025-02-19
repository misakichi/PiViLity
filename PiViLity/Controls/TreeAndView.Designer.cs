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
            tvwDirMain = new TreeView();
            splitDirView = new Splitter();
            panelViewInfo = new Panel();
            panelInfo = new Panel();
            splitViewInfo = new Splitter();
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
            panelInfo.Location = new Point(0, 3);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new Size(458, 420);
            panelInfo.TabIndex = 15;
            // 
            // splitViewInfo
            // 
            splitViewInfo.Dock = DockStyle.Top;
            splitViewInfo.Location = new Point(0, 0);
            splitViewInfo.Name = "splitViewInfo";
            splitViewInfo.Size = new Size(458, 3);
            splitViewInfo.TabIndex = 13;
            splitViewInfo.TabStop = false;
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