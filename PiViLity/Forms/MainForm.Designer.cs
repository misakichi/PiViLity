namespace PiViLity.Forms
{
    partial class MainForm
    {
        /// <summary lang="ja">
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary lang="ja">
        ///  Clean up any resources being used.
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

        /// <summary lang="ja">
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolStrip = new ToolStrip();
            stsStrip = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            mnuForm = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            quitToolStripMenuItem = new ToolStripMenuItem();
            toolToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            tabPanel = new Panel();
            dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            windowToolStripMenuItem = new ToolStripMenuItem();
            mnuExplorer = new ToolStripMenuItem();
            mnuPreview = new ToolStripMenuItem();
            mnuProperty = new ToolStripMenuItem();
            stsStrip.SuspendLayout();
            mnuForm.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            resources.ApplyResources(toolStrip, "toolStrip");
            toolStrip.Name = "toolStrip";
            // 
            // stsStrip
            // 
            stsStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            resources.ApplyResources(stsStrip, "stsStrip");
            stsStrip.Name = "stsStrip";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // mnuForm
            // 
            mnuForm.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolToolStripMenuItem, windowToolStripMenuItem });
            resources.ApplyResources(mnuForm, "mnuForm");
            mnuForm.Name = "mnuForm";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { quitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // quitToolStripMenuItem
            // 
            quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            resources.ApplyResources(quitToolStripMenuItem, "quitToolStripMenuItem");
            // 
            // toolToolStripMenuItem
            // 
            toolToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { optionsToolStripMenuItem });
            toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            resources.ApplyResources(toolToolStripMenuItem, "toolToolStripMenuItem");
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
            // 
            // tabPanel
            // 
            resources.ApplyResources(tabPanel, "tabPanel");
            tabPanel.Name = "tabPanel";
            // 
            // dockPanel
            // 
            resources.ApplyResources(dockPanel, "dockPanel");
            dockPanel.Name = "dockPanel";
            dockPanel.ShowDocumentIcon = true;
            // 
            // windowToolStripMenuItem
            // 
            windowToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuExplorer, mnuPreview, mnuProperty });
            windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            resources.ApplyResources(windowToolStripMenuItem, "windowToolStripMenuItem");
            // 
            // mnuExplorer
            // 
            mnuExplorer.Name = "mnuExplorer";
            resources.ApplyResources(mnuExplorer, "mnuExplorer");
            mnuExplorer.Click += mnuExplorer_Click;
            // 
            // mnuPreview
            // 
            mnuPreview.Name = "mnuPreview";
            resources.ApplyResources(mnuPreview, "mnuPreview");
            mnuPreview.Click += mnuPreview_Click;
            // 
            // mnuProperty
            // 
            mnuProperty.Name = "mnuProperty";
            resources.ApplyResources(mnuProperty, "mnuProperty");
            mnuProperty.Click += mnuProperty_Click;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(dockPanel);
            Controls.Add(tabPanel);
            Controls.Add(stsStrip);
            Controls.Add(toolStrip);
            Controls.Add(mnuForm);
            MainMenuStrip = mnuForm;
            Name = "MainForm";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            stsStrip.ResumeLayout(false);
            stsStrip.PerformLayout();
            mnuForm.ResumeLayout(false);
            mnuForm.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip;
        private StatusStrip stsStrip;
        private MenuStrip mnuForm;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem quitToolStripMenuItem;
        private ToolStripMenuItem toolToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private Panel tabPanel;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private ToolStripMenuItem windowToolStripMenuItem;
        private ToolStripMenuItem mnuExplorer;
        private ToolStripMenuItem mnuPreview;
        private ToolStripMenuItem mnuProperty;
    }
}
