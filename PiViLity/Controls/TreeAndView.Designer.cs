using System.Windows.Forms;
using PiViLityCore.Controls;

namespace PiViLity.Controls
{
  #if false
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
            tvwDirMain = new DirectoryTreeView();
            splitDirView = new Splitter();
            panelViewInfo = new Panel();
            lsvFile = new FileListView();
            splitViewInfo = new Splitter();
            panelInfo = new Panel();
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
            tvwDirMain.HideSelection = false;
            tvwDirMain.HotTracking = true;
            tvwDirMain.ImageIndex = 0;
            tvwDirMain.Location = new Point(0, 25);
            tvwDirMain.Name = "tvwDirMain";
            tvwDirMain.SelectedImageIndex = 0;
            tvwDirMain.ShowLines = false;
            tvwDirMain.Size = new Size(226, 423);
            tvwDirMain.TabIndex = 4;
            // 
            // splitDirView
            // 
            splitDirView.Location = new Point(226, 25);
            splitDirView.Name = "splitDirView";
            splitDirView.Size = new Size(4, 423);
            splitDirView.TabIndex = 6;
            splitDirView.TabStop = false;
            // 
            // panelViewInfo
            // 
            panelViewInfo.Controls.Add(lsvFile);
            panelViewInfo.Controls.Add(splitViewInfo);
            panelViewInfo.Controls.Add(panelInfo);
            panelViewInfo.Dock = DockStyle.Fill;
            panelViewInfo.Location = new Point(230, 25);
            panelViewInfo.Name = "panelViewInfo";
            panelViewInfo.Size = new Size(457, 423);
            panelViewInfo.TabIndex = 13;
            // 
            // lsvFile
            // 
            lsvFile.Dock = DockStyle.Fill;
            lsvFile.Location = new Point(0, 0);
            lsvFile.Name = "lsvFile";
            lsvFile.Size = new Size(457, 252);
            lsvFile.Sorting = SortOrder.Ascending;
            lsvFile.TabIndex = 17;
            lsvFile.UseCompatibleStateImageBehavior = false;
            // 
            // splitViewInfo
            // 
            splitViewInfo.Dock = DockStyle.Bottom;
            splitViewInfo.Location = new Point(0, 252);
            splitViewInfo.Name = "splitViewInfo";
            splitViewInfo.Size = new Size(457, 4);
            splitViewInfo.TabIndex = 13;
            splitViewInfo.TabStop = false;
            // 
            // panelInfo
            // 
            panelInfo.BorderStyle = BorderStyle.Fixed3D;
            panelInfo.Dock = DockStyle.Bottom;
            panelInfo.Location = new Point(0, 256);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new Size(457, 167);
            panelInfo.TabIndex = 15;
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

        private PiViLityCore.Controls.DirectoryTreeView tvwDirMain;
        private Splitter splitDirView;
        private Panel panelViewInfo;
        private Panel panelInfo;
        private Splitter splitViewInfo;
        private ToolStrip toolStrip;
        private FileListView lsvFile;
    }
#endif
}