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
            Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Option));
            TableLayoutPanel tableLayoutPanel1;
            pnlContent = new Panel();
            tvOptions = new TreeView();
            panel1 = new Panel();
            btnOk = new Button();
            btnCancel = new Button();
            label1 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.Controls.Add(pnlContent, 1, 0);
            tableLayoutPanel1.Controls.Add(tvOptions, 0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pnlContent
            // 
            resources.ApplyResources(pnlContent, "pnlContent");
            pnlContent.Name = "pnlContent";
            // 
            // tvOptions
            // 
            resources.ApplyResources(tvOptions, "tvOptions");
            tvOptions.HideSelection = false;
            tvOptions.Name = "tvOptions";
            // 
            // panel1
            // 
            panel1.Controls.Add(btnOk);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnCancel);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            resources.ApplyResources(btnOk, "btnOk");
            btnOk.Name = "btnOk";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.Name = "btnCancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // Option
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(panel1);
            Name = "Option";
            FormClosed += Option_FormClosed;
            Load += Option_Load;
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnOk;
        private Button btnCancel;
        private Panel pnlContent;
        private TreeView tvOptions;
    }
}