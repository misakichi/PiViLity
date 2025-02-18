using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PiViLity
{
    public partial class TreeAndView
    {

        private int _dragEnterKeyState = 0;

        private IFileSystemItem? GetDropTargetFs(object sender, DragEventArgs e)
        {
            var tree = sender as TreeView;
            var lsv = sender as ListView;
            Control? control = tree as Control ?? lsv;
            if (control == null)
            {
                return null;
            }
            Point pt = control.PointToClient(new Point(e.X, e.Y));
            IFileSystemItem? fileSystemItem = null;
            if (tree != null)
            {
                TreeNode? targetNode = tree.GetNodeAt(pt);
                fileSystemItem = targetNode?.Tag as IFileSystemItem;
            }
            else if (lsv != null)
            {
                ListViewItem? targetItem = lsv.GetItemAt(pt.X, pt.Y);
                fileSystemItem = targetItem?.Tag as IFileSystemItem;
            }
            return fileSystemItem;
        }

        private void ChgeckProcessDragItem(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (e.Data == null)
            {
                return;
            }

            IFileSystemItem? fileSystemItem = GetDropTargetFs(sender, e);
            if (fileSystemItem == null)
            {
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is string[] pathList)
            {
                if (fileSystemItem.HasPath)
                {
                    var isDir = PiViLityCore.Util.Shell.IsDirectory(fileSystemItem.Path);
                    var isExe = PiViLityCore.Util.Shell.IsExecute(fileSystemItem.Path);
                    if (isExe)
                    {
                        return;
                    }
                    else if (isDir)
                    {
                        //ディレクトリなら状況に応じて（でもディレクトリなら結局メニュー出るけど）
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
                        var destRoot = Path.GetPathRoot(fileSystemItem.Path);
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
        }
        private void ProcessDropItem(object sender, DragEventArgs e)
        {
            ChgeckProcessDragItem(sender, e);
            if (e.Effect != DragDropEffects.None)
            {
                IFileSystemItem? fileSystemItem = GetDropTargetFs(sender, e);
                if (fileSystemItem == null)
                {
                    return;
                }

                var pt = new Point(e.X, e.Y);
                //var clientPt = tvwDirMain.PointToClient(pt);
                //var test = tvwDirMain.HitTest(clientPt.X, clientPt.Y);
                //var node = test?.Node;
                if (fileSystemItem.HasPath)
                {
                    var isDir = PiViLityCore.Util.Shell.IsDirectory(fileSystemItem.Path);
                    var isExe = PiViLityCore.Util.Shell.IsExecute(fileSystemItem.Path);
                    if ((_dragEnterKeyState & 2) != 0)
                    {
                        if (e.Data?.GetData(DataFormats.FileDrop) is string[] pathList)
                        {
                            var drop = Marshal.GetIUnknownForObject(this);
                            var dragData = Marshal.GetIUnknownForObject(e.Data);
                            var screen = (pt);
                            //PiVilityNative.ShellAPI.ShowShellDropContextMenu(dirTreeNode.Path, drop, dragData, tvwDirMain.Handle, screen.X, screen.Y);
                        }
                    }
                    else
                    {
                        if (e.Data?.GetData(DataFormats.FileDrop) is string[] pathList)
                        {
                            if (isDir)
                            {
                                if (ModifierKeys.HasFlag(Keys.Control))
                                {
                                    PiViLityCore.Util.Shell.Copy(pathList, fileSystemItem.Path);
                                }
                                else if (ModifierKeys.HasFlag(Keys.Shift))
                                {
                                    PiViLityCore.Util.Shell.Move(pathList, fileSystemItem.Path);
                                }
                                else if (ModifierKeys.HasFlag(Keys.Alt))
                                {
                                    foreach (var srcPath in pathList)
                                    {
                                        PiViLityCore.Util.Shell.CreateShortCut(srcPath, fileSystemItem.Path, "new shortcut");
                                    }
                                }
                                else
                                {
                                    PiViLityCore.Util.Shell.Move(pathList, fileSystemItem.Path);
                                }
                            }
                            else
                            {
                                //実行
                                System.Diagnostics.Process.Start(fileSystemItem.Path, pathList);
                            }
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
                        if (dirTreeNode.HasPath)
                        {
                            // フォルダの右クリックメニューを表示
                            var screen = tvwDirMain.PointToScreen(e.Location);
                            PiVilityNative.ShellAPI.ShowShellContextMenu([dirTreeNode.Path], tvwDirMain.Handle, screen.X, screen.Y);
                        }
                    }
                }
            }
        }

        private void tvwDirMain_DragDrop(object sender, DragEventArgs e)
        {
            ChgeckProcessDragItem(sender, e);
            if(e.Effect != DragDropEffects.None)
            {
                
                var pt = new Point(e.X, e.Y);
                var clientPt = tvwDirMain.PointToClient(pt);
                var test = tvwDirMain.HitTest(clientPt.X, clientPt.Y);
                var node = test?.Node;
                if (node?.Tag is DirTreeNode dirTreeNode && dirTreeNode.HasPath)
                {
                    var isDir = PiViLityCore.Util.Shell.IsDirectory(dirTreeNode.Path);
                    var isExe = PiViLityCore.Util.Shell.IsExecute(dirTreeNode.Path);
                    if ((_dragEnterKeyState & 2) != 0)
                    {
                        if (e.Data?.GetData(DataFormats.FileDrop) is string[] pathList)
                        {
                            var drop = Marshal.GetIUnknownForObject(this);
                            var dragData = Marshal.GetIUnknownForObject(e.Data);
                            var screen = (pt);
                            //PiVilityNative.ShellAPI.ShowShellDropContextMenu(dirTreeNode.Path, drop, dragData, tvwDirMain.Handle, screen.X, screen.Y);
                        }
                    }
                    else
                    {
                        if (e.Data?.GetData(DataFormats.FileDrop) is string[] pathList)
                        {
                            foreach (var srcPath in pathList)
                            {
                                if (isDir)
                                {
                                    if (ModifierKeys.HasFlag(Keys.Control))
                                    {
                                        PiViLityCore.Util.Shell.Copy(srcPath, dirTreeNode.Path);
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Shift))
                                    {
                                        PiViLityCore.Util.Shell.Move(srcPath, dirTreeNode.Path);
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Alt))
                                    {
                                        PiViLityCore.Util.Shell.CreateShortCut(srcPath, dirTreeNode.Path, "new shortcut");
                                    }
                                    else
                                    {
                                        PiViLityCore.Util.Shell.Move(srcPath, dirTreeNode.Path);
                                    }
                                }
                                else
                                {
                                   //実行
                                }
                            }
                        }
                    }
                }
            }
        }

        private void tvwDirMain_DragEnter(object sender, DragEventArgs e)
        {
            _dragEnterKeyState = e.KeyState;
            ChgeckProcessDragItem(sender, e);
        }

        private void tvwDirMain_DragLeave(object sender, EventArgs e)
        {
            // 必要に応じて処理を追加
        }

        private void tvwDirMain_DragOver(object sender, DragEventArgs e)
        {
            ChgeckProcessDragItem(sender, e);
        }

        private void tvwDirMain_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode node)
            {
                if (node.Tag is DirTreeNode dirTreeNode)
                {
                    if (dirTreeNode.HasPath)
                    {
                        var paths = new string[1];
                        var path = dirTreeNode.Path;
                        if(Directory.Exists(path))
                        {
                            paths[0] = path;
                            tvwDirMain.DoDragDrop(new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
                        }
                    }
                }
            }
        }
    }
}
