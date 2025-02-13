using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PiViLity.FileListView;

namespace PiViLity
{

    public partial class TreeAndView
    {
        private void lsvFile_MouseClick(object sender, MouseEventArgs e)
        {
            // lsvFileの右クリックイベント処理
            if (e.Button == MouseButtons.Right)
            {
                List<string> list = new();
                foreach (ListViewItem? item in lsvFile.SelectedItems)
                {
                    if(item?.Tag is FileListItemData data)
                    {
                        list.Add(data.Path);
                    }
                }
                if (list.Count > 0)
                {
                    // ファイルの右クリックメニューを表示
                    var screen = lsvFile.PointToScreen(e.Location);
                    PiVilityNative.ShellAPI.ShowShellContextMenu(list.ToArray(), lsvFile.Handle, screen.X, screen.Y);
                }
            }
        }
    }
}
