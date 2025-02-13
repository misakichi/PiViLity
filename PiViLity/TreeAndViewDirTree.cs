using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity
{
    public partial class TreeAndView
    {

        private void ProcessDropItem(DragEventArgs e)
        {
            if (e?.Data == null)
            {
                return;
            }
            Point pt = tvwDirMain.PointToClient(new Point(e.X, e.Y));
            TreeNode? targetNode = tvwDirMain.GetNodeAt(pt);
            DirTreeNode? dirTreeNode = targetNode?.Tag as DirTreeNode;
            e.Effect = DragDropEffects.None;
            if (e.Data.GetData(DataFormats.FileDrop) is string[] pathList)
            {
                if (dirTreeNode == null)
                {
                    return;
                }

                if (dirTreeNode.IsPath)
                {
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                    else if (ModifierKeys.HasFlag(Keys.Shift))
                    {
                        e.Effect = DragDropEffects.Move;
                        return;
                    }
                    else if (ModifierKeys.HasFlag(Keys.Alt))
                    {
                        e.Effect = DragDropEffects.Link;
                        return;
                    }

                    var destRoot = Path.GetPathRoot(dirTreeNode.Path);
                    e.Effect = DragDropEffects.Move;
                    foreach (var srcPath in pathList)
                    {
                        if (string.Compare(Path.GetPathRoot(srcPath), destRoot, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            e.Effect = DragDropEffects.Copy;
                        }
                    }
                }
            }
        }

        private void tvwDirMain_MouseClick(object sender, MouseEventArgs e)
        {
            // tvwDirMainの右クリックイベント処理
            if (e.Button == MouseButtons.Right)
            {
                var node = tvwDirMain.SelectedNode;
                if (node != null)
                {
                    if(node.Tag is DirTreeNode dirTreeNode)
                    {
                        if (dirTreeNode.IsPath)
                        {
                            // フォルダの右クリックメニューを表示
                            var screen = tvwDirMain.PointToScreen(e.Location);
                            ShelAPIHelper.ShellAPI.ShowShellContextMenu([dirTreeNode.Path], tvwDirMain.Handle, screen.X, screen.Y);
                        }
                    }
                }
            }
        }

        private void tvwDirMain_DragDrop(object sender, DragEventArgs e)
        {
            ProcessDropItem(e);
        }

        private void tvwDirMain_DragEnter(object sender, DragEventArgs e)
        {
            ProcessDropItem(e);
        }

        private void tvwDirMain_DragLeave(object sender, EventArgs e)
        {
            // 必要に応じて処理を追加
        }

        private void tvwDirMain_DragOver(object sender, DragEventArgs e)
        {
            ProcessDropItem(e);
        }

        private void tvwDirMain_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // 必要に応じて処理を追加
        }
    }
}
