namespace PiViLity.Forms
{
    partial class Option
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Option));
            tvOptions = new TreeView();
            pnlContent = new Panel();
            SuspendLayout();
            // 
            // tvOptions
            // 
            tvOptions.HideSelection = false;
            resources.ApplyResources(tvOptions, "tvOptions");
            tvOptions.Name = "tvOptions";
            // 
            // pnlContent
            // 
            resources.ApplyResources(pnlContent, "pnlContent");
            pnlContent.Name = "pnlContent";
            // 
            // Option
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            Controls.Add(pnlContent);
            Controls.Add(tvOptions);
            Name = "Option";
            Load += Option_Load;
            ResumeLayout(false);
        }

        #endregion

        private TreeView tvOptions;
        private Panel pnlContent;
    }
}