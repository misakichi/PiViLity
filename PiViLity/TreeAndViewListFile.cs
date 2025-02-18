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


        private void lsvFile_ItemDrag(object sender, ItemDragEventArgs e)
        {
            List<string> files = new();
            for (int i = 0; i < lsvFile.SelectedItems.Count; i++)
            {
                if (lsvFile.SelectedItems[i].Tag is FileListItemData data)
                {
                    files.Add(data.Path);
                }
            }
            if (files.Count > 0)
            {
                lsvFile.DoDragDrop(new DataObject(DataFormats.FileDrop, files.ToArray()), DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
            }

        }

        private void lsvFile_DragEnter(object sender, DragEventArgs e)
        {
            _dragEnterKeyState = e.KeyState;
            ChgeckProcessDragItem(sender, e);
        }

        private void lsvFile_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void lsvFile_DragLeave(object sender, EventArgs e)
        {

        }

        private void lsvFile_DragOver(object sender, DragEventArgs e)
        {
            ChgeckProcessDragItem(sender, e);
        }

    }
}
