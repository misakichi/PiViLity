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
            tvwDirMain = new TreeView();
            splitDirView = new Splitter();
            lsvFile = new FileListView();
            panel1 = new Panel();
            splitter1 = new Splitter();
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
            tvwDirMain.Size = new Size(226, 363);
            tvwDirMain.TabIndex = 4;
            // 
            // splitDirView
            // 
            splitDirView.Location = new Point(226, 0);
            splitDirView.Name = "splitDirView";
            splitDirView.Size = new Size(3, 363);
            splitDirView.TabIndex = 6;
            splitDirView.TabStop = false;
            // 
            // lsvFile
            // 
            lsvFile.BorderStyle = BorderStyle.None;
            lsvFile.Dock = DockStyle.Fill;
            lsvFile.Location = new Point(229, 0);
            lsvFile.Name = "lsvFile";
            lsvFile.Size = new Size(351, 363);
            lsvFile.TabIndex = 7;
            lsvFile.TileSize = new Size(320, 240);
            lsvFile.UseCompatibleStateImageBehavior = false;
            lsvFile.View = View.Details;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(229, 263);
            panel1.Name = "panel1";
            panel1.Size = new Size(351, 100);
            panel1.TabIndex = 8;
            // 
            // splitter1
            // 
            splitter1.Dock = DockStyle.Bottom;
            splitter1.Location = new Point(229, 260);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(351, 3);
            splitter1.TabIndex = 9;
            splitter1.TabStop = false;
            // 
            // TreeAndView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(splitter1);
            Controls.Add(panel1);
            Controls.Add(lsvFile);
            Controls.Add(splitDirView);
            Controls.Add(tvwDirMain);
            DoubleBuffered = true;
            Name = "TreeAndView";
            Size = new Size(580, 363);
            ResumeLayout(false);
        }

        #endregion

        private TreeView tvwDirMain;
        private Splitter splitDirView;
        private FileListView lsvFile;
        private Panel panel1;
        private Splitter splitter1;
    }
}