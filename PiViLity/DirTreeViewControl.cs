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
        IconStore iconStore = new(false, true);

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

            treeView.ImageList = iconStore.smallIconList;

            RefreshTree();
        }

        private void TreeView_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            DirTreeViewControlEventArgs args = new();
            args.treeNode = e.Node;
            args.dirTreeNode = e.Node?.Tag as DirTreeNode;

            AfterSelect?.Invoke(this, args);
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
            tn.SelectedImageIndex = tn.ImageIndex = iconStore.GetIconIndex(dirTreeNode.Path);            
            return tn;
        }

        private void TreeView_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            if (sender is TreeView tvw)
            {
                if (e.Node?.Tag is DirTreeNode dirNode)
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
                                if (parentNode == e.Node)
                                {
                                    addList.Add(tn);
                                }
                                else
                                {
                                    parentNode.Nodes.Add(tn);
                                }
                            }
                        });
                        tvw.Visible = false;
                        tvw.BeginUpdate();
                        e.Node.Nodes.Clear();
                        e.Node.Nodes.AddRange(addList.ToArray());
                        tvw.EndUpdate();
                        tvw.Visible = true;

                    }
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
