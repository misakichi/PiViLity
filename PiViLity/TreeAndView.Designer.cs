using System.Windows.Forms;

namespace PiViLity
{
    partial class TreeAndView
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
            components = new System.ComponentModel.Container();
            tvwDirMain = new TreeView();
            splitDirView = new Splitter();
            panelViewInfo = new Panel();
            panelInfo = new Panel();
            splitViewInfo = new Splitter();
            lsvFile = new FileListView();
            toolStrip = new ToolStrip();
            panelViewInfo.SuspendLayout();
            panelInfo.SuspendLayout();
            SuspendLayout();
            // 
            // tvwDirMain
            // 
            tvwDirMain.BorderStyle = BorderStyle.None;
            tvwDirMain.Dock = DockStyle.Left;
            tvwDirMain.FullRowSelect = true;
            tvwDirMain.Location = new Point(0, 0);
            tvwDirMain.Name = "tvwDirMain";
            tvwDirMain.ShowLines = false;
            tvwDirMain.Size = new Size(226, 679);
            tvwDirMain.TabIndex = 4;
            // 
            // splitDirView
            // 
            splitDirView.Location = new Point(226, 0);
            splitDirView.Name = "splitDirView";
            splitDirView.Size = new Size(3, 679);
            splitDirView.TabIndex = 6;
            splitDirView.TabStop = false;
            // 
            // panelViewInfo
            // 
            panelViewInfo.Controls.Add(panelInfo);
            panelViewInfo.Controls.Add(splitViewInfo);
            panelViewInfo.Controls.Add(lsvFile);
            panelViewInfo.Dock = DockStyle.Fill;
            panelViewInfo.Location = new Point(229, 0);
            panelViewInfo.Name = "panelViewInfo";
            panelViewInfo.Size = new Size(598, 679);
            panelViewInfo.TabIndex = 13;
            // 
            // panelInfo
            // 
            panelInfo.BorderStyle = BorderStyle.Fixed3D;
            panelInfo.Dock = DockStyle.Fill;
            panelInfo.Location = new Point(0, 197);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new Size(598, 482);
            panelInfo.TabIndex = 15;
            // 
            // splitViewInfo
            // 
            splitViewInfo.Dock = DockStyle.Top;
            splitViewInfo.Location = new Point(0, 194);
            splitViewInfo.Name = "splitViewInfo";
            splitViewInfo.Size = new Size(598, 3);
            splitViewInfo.TabIndex = 13;
            splitViewInfo.TabStop = false;
            // 
            // lsvFile
            // 
            lsvFile.BorderStyle = BorderStyle.None;
            lsvFile.Dock = DockStyle.Top;
            lsvFile.Location = new Point(0, 0);
            lsvFile.Name = "lsvFile";
            lsvFile.Size = new Size(598, 194);
            lsvFile.TabIndex = 12;
            lsvFile.TileSize = new Size(320, 240);
            lsvFile.UseCompatibleStateImageBehavior = false;
            // 
            // toolStrip
            // 
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(594, 25);
            toolStrip.TabIndex = 0;
            toolStrip.Text = "toolStrip";
            toolStrip.Dock = DockStyle.Top;
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
            Size = new Size(827, 679);
            panelViewInfo.ResumeLayout(false);
            panelInfo.ResumeLayout(false);
            panelInfo.PerformLayout();
            ResumeLayout(false);
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