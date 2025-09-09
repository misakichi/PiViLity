namespace PiViLity.Dock
{
    partial class FilePropertyContent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilePropertyContent));
            treeProp = new TreeView();
            SuspendLayout();
            // 
            // treeProp
            // 
            resources.ApplyResources(treeProp, "treeProp");
            treeProp.Name = "treeProp";
            treeProp.ShowLines = false;
            // 
            // FilePropertyContent
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeProp);
            Name = "FilePropertyContent";
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeProp;
    }
}