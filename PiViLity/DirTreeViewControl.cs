using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PiViLityCore.Shell;

namespace PiViLity
{
    internal class DirTreeViewControl
    {
        TreeView? treeView;
        DirTree? dirTree;
        IconStore iconStore = new(false, true, false);

        public class DirTreeViewControlEventArgs : EventArgs
        {
            public TreeNode? treeNode;
            public DirTreeNode? dirTreeNode;
        }
        public delegate void DirTreeViewControlEventHandler(DirTreeViewControl sender, DirTreeViewControlEventArgs e);

        public event DirTreeViewControlEventHandler? AfterSelect;

        public void Bind(DirTree _dirTree, TreeView _treeView)
        {
            dirTree = _dirTree;
            treeView = _treeView;

            treeView.BeforeExpand += TreeView_BeforeExpand;
            treeView.AfterSelect += TreeView_AfterSelect;

            treeView.ImageList = iconStore.SmallIconList;

            RefreshTree();
        }

        private void TreeView_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            DirTreeViewControlEventArgs args = new();
            args.treeNode = e.Node;
            args.dirTreeNode = e.Node?.Tag as DirTreeNode;

            AfterSelect?.Invoke(this, args);
        }

        /// <summary>
        /// 指定されたパスのディレクトリノードを検索する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private DirTreeNode? SearchDirectory(string path)
        {
            if (!Directory.Exists(path))
                return null;

            //深さ優先でノードを探す
            DirTreeNode? SearchDepthNode(DirTreeNode parent, string path)
            {
                path = path.ToLower();
                if (parent.Tag is TreeNode tn)
                {
                    CheckAndAnalyzeUnknownDirectory(parent);
                    DirTreeNode? matchNode = null;
                    int maxMatchLength = 0;
                    parent.Children.ForEach(node =>
                    {
                        if (maxMatchLength > node.Path.Length)
                            return;

                        if (path.StartsWith(node.Path, StringComparison.OrdinalIgnoreCase))
                        {
                            maxMatchLength = node.Path.Length;
                            matchNode = node;
                        }
                    });
                    if (matchNode == null)
                        return null;
                    if (String.Compare(path, matchNode.Path,StringComparison.OrdinalIgnoreCase)==0)
                    {
                        return matchNode;
                    }
                    else
                    {
                        return SearchDepthNode(matchNode, path);
                    }
                }
                return null;
            }

            //まずは特殊ノードからルートになるものを探す
            var node = dirTree?.RootNode;
            while(node!=null)
            {
                if (node.IsSpecialFolder && node.Type==DirTreeNodeType.SpecialFolderMyComputer)
                {
                    break;
                }
            }
            //探す
            if(node != null)
                return SearchDepthNode(node, path);
            return null;
        }

        /// <summary lang="ja-JP">
        /// 
        /// </summary>
        public string SelectedPath
        {
            get
            {
                if (treeView?.SelectedNode?.Tag is DirTreeNode dirTreeNode)
                {
                    return dirTreeNode.Path;
                }
                return "";
            }
            set
            {
                if(treeView == null)
                {
                    return;
                }
                var node = SearchDirectory(value);
                if (node != null && node.Tag is TreeNode tn)
                {
                    treeView.SelectedNode = tn;
                    tn.EnsureVisible();
                }
            }
        }

        /// <summary lang="ja-JP">
        /// 現在選択されているノードの名称を返す
        /// </summary>
        public string SelectedName
        {
            get
            {
                if (treeView?.SelectedNode?.Tag is DirTreeNode dirTreeNode)
                {
                    return dirTreeNode.Name;
                }
                return "";
            }
        }

        TreeNode TreeNodeFromDirNode( DirTreeNode dirTreeNode)
        {
            var tn = new TreeNode(dirTreeNode.ToString());
            dirTreeNode.Tag = tn;
            tn.Tag = dirTreeNode;
            tn.Name = dirTreeNode.Name;
            iconStore.GetIcon(dirTreeNode.Path, index => tn.SelectedImageIndex = tn.ImageIndex = index);            
            return tn;
        }

        private bool CheckAndAnalyzeUnknownDirectory(DirTreeNode dirNode)
        {
            if (treeView == null)
                return false;
            if (dirNode.Tag is TreeNode tvn)
            {
                if (dirNode.Type == DirTreeNodeType.DirectoryUnknown)
                {
                    List<TreeNode> addList = new();
                    dirNode.SetType(DirTreeNodeType.Directory, dirNode.Path);
                    dirNode.CurrentFirstTraversalTreeNodes((node, depth) =>
                    {
                        if (dirNode == node)
                            return;
                        var tn = TreeNodeFromDirNode(node);
                        if (node.Parent?.Tag is TreeNode parentNode)
                        {
                            //追加先が展開自身ならあとで一括追加する
                            if (parentNode == tvn)
                            {
                                addList.Add(tn);
                            }
                            else
                            {
                                parentNode.Nodes.Add(tn);
                            }
                        }
                    });
                    treeView.Visible = false;
                    treeView.BeginUpdate();
                    tvn.Nodes.Clear();
                    tvn.Nodes.AddRange(addList.ToArray());
                    treeView.EndUpdate();
                    treeView.Visible = true;
                    return true;
                }
            }
            return false;
        }

        private void TreeView_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            if (sender is TreeView tvw)
            {
                if (e.Node?.Tag is DirTreeNode dirNode)
                {
                    CheckAndAnalyzeUnknownDirectory(dirNode);
                }
            }
        }

        /// <summary lang="ja">
        /// 現状の保持状態からTreeViewを初期化する
        /// </summary>
        void RefreshTree()
        {
            if(treeView == null || dirTree == null) 
                return;

            var curPath = SelectedPath;

            //JA 各ノードに対しTreeNodeを割り当てる
            dirTree.RootNode.CurrentFirstTraversalTreeNodes((node, depth) => 
            {
                var tn = TreeNodeFromDirNode(node);
                if (node.Parent?.Tag is TreeNode parentNode)
                {
                    parentNode.Nodes.Add(tn);
                }
            });

            //JA TreeViewのノードを現状のものにする
            treeView.Nodes.Clear();
            if (dirTree.RootNode.Tag is TreeNode tvNode)
            {
                treeView.Nodes.Add(tvNode);
            }

            SelectedPath = curPath;
            if(treeView.SelectedNode==null)
            {
                if(treeView.Nodes.Count>0)
                {
                    treeView.TopNode = treeView.SelectedNode = treeView.Nodes[0];
                }
            }

        }

    }
}
