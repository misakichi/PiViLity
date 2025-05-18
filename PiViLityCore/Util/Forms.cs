using PiViLityCore.Plugin;
using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    static public class Forms
    {
        /// <summary>
        /// ドロップされたとき、ListViewまたはTreeViewのドロップ先のファイルシステムアイテムを取得します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IFileSystemItem? GetDropTargetFs(object sender, DragEventArgs e)
        {
            var tree = sender as TreeView;
            var lsv = sender as ListView;
            Control? control = tree as Control ?? lsv;
            if (control == null)
                return null;

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

        public static void ChgeckProcessDragItem(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (e.Data == null)
                return;

            IFileSystemItem? fileSystemItem = PiViLityCore.Util.Forms.GetDropTargetFs(sender, e);

            if (fileSystemItem != null &&
                sender is Control control && 
                e.Data.GetData(DataFormats.FileDrop) is string[] pathList)
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
                        if (Control.ModifierKeys.HasFlag(Keys.Control))
                        {
                            e.Effect = DragDropEffects.Copy;
                            return;
                        }
                        else if (Control.ModifierKeys.HasFlag(Keys.Shift))
                        {
                            e.Effect = DragDropEffects.Move;
                            return;
                        }
                        else if (Control.ModifierKeys.HasFlag(Keys.Alt))
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
        /// <summary lang="ja-JP">
        /// 現在階層の処理を優先で子階層を含むコントロールに対して処理を行います。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void CurrentFirstTraversalControl(System.Windows.Forms.Control control, Action<System.Windows.Forms.Control, int> action)
        {
            Queue<Tuple<System.Windows.Forms.Control, int>> stack = new();
            stack.Enqueue(new Tuple<System.Windows.Forms.Control, int>(control, 0));

            Tuple<System.Windows.Forms.Control, int>? cur;
            while (stack.TryDequeue(out cur))
            {
                action(cur.Item1, cur.Item2);
                foreach (System.Windows.Forms.Control child in cur.Item1.Controls)
                    stack.Enqueue(new Tuple<System.Windows.Forms.Control, int>(child, cur.Item2 + 1));

            }
        }

        public static void FormInitializeSystemTheme(System.Windows.Forms.Control control)
        {
            CurrentFirstTraversalControl(control, (c, d) =>
            {
                if (c != null)
                {
                    control.Font = SystemFonts.MessageBoxFont;
                }
            });
        }

        public static bool ShowFileOnView(string path, ContainerControl? parent)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            bool ret = false;
            var fileInfo = new System.IO.FileInfo(path);
            if (fileInfo.Exists)
            {
                var ext = System.IO.Path.GetExtension(path).ToLower();
                if (PluginManager.Instance.SupportImageExtensions.Any(s => ext.CompareTo(s) == 0))
                {
                    var viewer = new PiViLityCore.Forms.ViewerForm();
                    if (viewer.LoadFile(path))
                    {
                        viewer.Owner = parent?.ParentForm;
                        viewer.StartPosition = FormStartPosition.CenterParent;
                        viewer.Show(parent?.ParentForm);
                        ret = true;
                    }
                    else
                    {
                        viewer.Dispose();

                    }
                }
            }
            return ret;
        }
    }
}
