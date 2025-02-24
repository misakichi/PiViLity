using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.ComponentModel;

namespace PiViLityCore.Controls
{
    public class DirectoryTreeView : TreeView
    {
        private int _dragEnterKeyState = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Shell.IIconStore IconStore { get; set; } = new Shell.IconStoreSystem(false, true, false);

        public class DirTreeViewControlEventArgs : EventArgs
        {
            public TreeNode? treeNode;
            public DirectoryTreeNode? DirectoryTreeNode;
        }


        public DirectoryTreeView() : base()
        {
            AllowDrop = true;

            //ツリービューセットアップ
            FullRowSelect = true;
            HideSelection = false;

            HotTracking = true;

            ImageList = IconStore.SmallIconList;

        }

        /// <summary lang="ja-JP">
        /// 選択されたパス
        /// </summary>
        /// <summary lang="en-US">
        /// Selected path
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedPath
        {
            get
            {
                if (SelectedNode is DirectoryTreeNode DirectoryTreeNode)
                {
                    return DirectoryTreeNode.Path;
                }
                return "";
            }
            set
            {
                var node = SearchDirectory(value);
                if (node != null)
                {
                    SelectedNode = node;
                    node.EnsureVisible();
                }
            }
        }


        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            if (e.Node is DirectoryTreeNode dirNode)
            {
                dirNode.CheckAndAnalyzeUnknownDirectory();
            }
        }

        /// <summary>
        /// 指定されたパスのディレクトリノードを検索する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public DirectoryTreeNode? SearchDirectory(string path)
        {
            if (!Directory.Exists(path))
                return null;

            DirectoryTreeNode? node = null;
            //まずは特殊ノードからルートになるものを探す
            foreach (DirectoryTreeNode _node in Nodes)
            {
                if (_node == null)
                    continue;
                if (_node.IsSpecialFolder && _node.Type == DirTreeNodeType.SpecialFolderMyComputer)
                {
                    node = _node;
                    break;
                }
            }

            //探す
            return node?.SearchDepthNode(path);
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if (e.Item is DirectoryTreeNode node)
            {
                if (node != null)
                {
                    if (node.HasPath)
                    {
                        var paths = new string[1];
                        var path = node.Path;
                        if (Directory.Exists(path))
                        {
                            paths[0] = path;
                            DoDragDrop(new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
                        }
                    }
                }
            }
            base.OnItemDrag(e);
        }
        protected override void OnDragEnter(DragEventArgs e)
        {
            _dragEnterKeyState = e.KeyState;
            PiViLityCore.Util.Forms.ChgeckProcessDragItem(this, e);
            base.OnDragEnter(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            PiViLityCore.Util.Forms.ChgeckProcessDragItem(this, e);
            base.OnDragOver(e);
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            PiViLityCore.Util.Forms.ChgeckProcessDragItem(this, e);
            if (e.Effect != DragDropEffects.None)
            {
                var pt = new Point(e.X, e.Y);
                var clientPt = PointToClient(pt);
                var test = HitTest(clientPt.X, clientPt.Y);
                var node = test?.Node;
                if (node is DirectoryTreeNode DirectoryTreeNode && DirectoryTreeNode.HasPath)
                {
                    var isDir = PiViLityCore.Util.Shell.IsDirectory(DirectoryTreeNode.Path);
                    var isExe = PiViLityCore.Util.Shell.IsExecute(DirectoryTreeNode.Path);
                    if ((_dragEnterKeyState & 2) != 0)
                    {
                        if (e.Data?.GetData(DataFormats.FileDrop) is string[] pathList)
                        {
                            var drop = Marshal.GetIUnknownForObject(this);
                            var dragData = Marshal.GetIUnknownForObject(e.Data);
                            var screen = (pt);
                            //PiVilityNative.ShellAPI.ShowShellDropContextMenu(DirectoryTreeNode.Path, drop, dragData, tvwDirMain.Handle, screen.X, screen.Y);
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
                                        PiViLityCore.Util.Shell.Copy(srcPath, DirectoryTreeNode.Path);
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Shift))
                                    {
                                        PiViLityCore.Util.Shell.Move(srcPath, DirectoryTreeNode.Path);
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Alt))
                                    {
                                        PiViLityCore.Util.Shell.CreateShortCut(srcPath, DirectoryTreeNode.Path, "new shortcut");
                                    }
                                    else
                                    {
                                        PiViLityCore.Util.Shell.Move(srcPath, DirectoryTreeNode.Path);
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
            base.OnDragDrop(e);
        }

        /// <summary>
        /// マウスクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // tvwDirMainの右クリックイベント処理
            if (e.Button == MouseButtons.Right)
            {
                if (SelectedNode is DirectoryTreeNode node)
                {
                    if (node.HasPath)
                    {
                        // フォルダの右クリックメニューを表示
                        var screen = PointToScreen(e.Location);
                        PiVilityNative.ShellAPI.ShowShellContextMenu([node.Path], Handle, screen.X, screen.Y);
                    }
                }
            }

            base.OnMouseClick(e);
        }

    }
}
