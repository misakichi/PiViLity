namespace PiViLity.Forms
{
    partial class SusiePluginSetting
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            GroupBox groupBox1;
            Button btnDirRef;
            txtPluginDirectory = new TextBox();
            groupBox1 = new GroupBox();
            btnDirRef = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtPluginDirectory);
            groupBox1.Controls.Add(btnDirRef);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6);
            groupBox1.Size = new Size(906, 54);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Susie プラグインディレクトリ";
            // 
            // txtPluginDirectory
            // 
            txtPluginDirectory.Dock = DockStyle.Fill;
            txtPluginDirectory.Location = new Point(6, 22);
            txtPluginDirectory.Name = "txtPluginDirectory";
            txtPluginDirectory.Size = new Size(866, 23);
            txtPluginDirectory.TabIndex = 2;
            // 
            // btnDirRef
            // 
            btnDirRef.Dock = DockStyle.Right;
            btnDirRef.Location = new Point(872, 22);
            btnDirRef.Name = "btnDirRef";
            btnDirRef.Padding = new Padding(0, 0, 0, 6);
            btnDirRef.Size = new Size(28, 26);
            btnDirRef.TabIndex = 1;
            btnDirRef.Text = "...";
            btnDirRef.UseVisualStyleBackColor = true;
            // 
            // SusiePluginSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Name = "SusiePluginSetting";
            Padding = new Padding(3);
            Size = new Size(912, 586);
            Load += SusiePluginSetting_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtPluginDirectory;
    }
}
