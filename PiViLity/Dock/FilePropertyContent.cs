using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace PiViLity.Dock
{
    public partial class FilePropertyContent : DockContent
    {
        public FilePropertyContent()
        {
            InitializeComponent();
            treeProp.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeProp.DrawNode += TreeProp_DrawNode;
        }

        private void TreeProp_DrawNode(object? sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null)
            {
                e.DrawDefault = true;
                return;
            }
            var split = e.Node.Text.Split('\v');
            if (split == null || split.Length == 0)
            {
                e.DrawDefault = true;
                return;
            }
            e.DrawDefault = false;

            TextRenderer.DrawText(e.Graphics, split[0], e.Node.NodeFont ?? e.Node.TreeView?.Font, e.Bounds, treeProp.ForeColor, TextFormatFlags.Left);
            if (split.Length > 1)
            {
                var rect = e.Bounds;
                rect.X += 200;
                TextRenderer.DrawText(e.Graphics, split[1], e.Node.NodeFont ?? e.Node.TreeView?.Font, rect, treeProp.ForeColor, TextFormatFlags.Left);
            }


        }
        public void SetFile(string filePath)
        {
            if (filePath == "" || System.IO.File.Exists(filePath) == false)
            {
                treeProp.Nodes.Clear();
                return;
            }

            System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(FilePropertyContent));
            var fileInfo = new System.IO.FileInfo(filePath);
            TreeNode root = new TreeNode(res.GetString("FileInfo.Head"));
            root.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Name")}\v{fileInfo.Name}"));
            root.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Path")}\v{fileInfo.FullName}"));
            root.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Type")}\v{PiVilityNative.FileInfo.GetFileTypeName(fileInfo.FullName)}"));
            root.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Size")}\v{PiViLityCore.Util.String.GetEasyReadFileSize(fileInfo.Length, false)} ({fileInfo.Length})"));
            root.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.CreatedDate")}\v{fileInfo.CreationTime}"));
            root.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.UpdatedDate")}\v{fileInfo.LastWriteTime}"));
            root.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.AccessedDate")}\v{fileInfo.LastAccessTime}"));
            treeProp.Nodes.Clear();
            treeProp.Nodes.Add(root);
            treeProp.ExpandAll();
        }
    }
}
