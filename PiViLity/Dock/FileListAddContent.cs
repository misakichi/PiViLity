using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PiViLity.Dock
{
    internal class FileListAddContent : DockContent
    {
        public FileListAddContent()
        {
            CloseButton = false;           // 閉じる操作を無効化
            CloseButtonVisible = false;    // タブの×ボタンを非表示
            AllowEndUserDocking = false;
            Text = "+";
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (CloseButtonVisible)
                e.Cancel = true;

            base.OnFormClosing(e);
        }
        protected override string GetPersistString()
        {
            return "FileListAddContent";
        }
    }

}
