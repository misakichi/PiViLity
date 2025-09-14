using PiViLityCore.Plugin;
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
        protected override string GetPersistString()
        {
            return "FilePropertyContent";
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
            if (filePath == "")
            {
                treeProp.Nodes.Clear();
                return;
            }

            System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(FilePropertyContent));
            TreeNode? fileRoot = null;
            TreeNode? imgRoot = new TreeNode(res.GetString("Property.Type.Image"));
            TreeNode? othersRoot = new TreeNode(res.GetString("Property.Type.Others"));//とりあえずその他としてまとめる。あとでその他のカテゴリ名称をIPropertyから取れるようにする

            if (System.IO.File.Exists(filePath))
            {
                var fileInfo = new System.IO.FileInfo(filePath);
                fileRoot = new TreeNode(res.GetString("FileInfo.Head"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Name")}\v{fileInfo.Name}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Path")}\v{fileInfo.FullName}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Type")}\v{PiVilityNative.FileInfo.GetFileTypeName(fileInfo.FullName)}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Size")}\v{PiViLityCore.Util.String.GetEasyReadFileSize(fileInfo.Length, false)} ({fileInfo.Length})"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.CreatedDate")}\v{fileInfo.CreationTime}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.UpdatedDate")}\v{fileInfo.LastWriteTime}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.AccessedDate")}\v{fileInfo.LastAccessTime}"));
            }
            else if (System.IO.Directory.Exists(filePath))
            {
                var dirInfo = new System.IO.DirectoryInfo(filePath);
                fileRoot = new TreeNode(res.GetString("FileInfo.DirHead"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.DirName")}\v{dirInfo.Name}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.Path")}\v{dirInfo.FullName}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.CreatedDate")}\v{dirInfo.CreationTime}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.UpdatedDate")}\v{dirInfo.LastWriteTime}"));
                fileRoot.Nodes.Add(new TreeNode($"{res.GetString("FileInfo.AccessedDate")}\v{dirInfo.LastAccessTime}"));
                treeProp.Nodes.Clear();
            }
            else
            {
                treeProp.Nodes.Clear();
                return;
            }
            using (var propReader = PluginManager.Instance.GetPropertyReader(filePath))
            {
                if (propReader != null)
                {
                    propReader.SetFilePath(filePath);
                    var properties = propReader.ReadProperties();
                    foreach (var prop in properties)
                    {
                        if (prop.Group == PropertyGroup.File)
                        {
                            fileRoot.Nodes.Add(new TreeNode($"{prop.Name}\v{prop.Value}"));
                        }
                        else if (prop.Group == PropertyGroup.Image)
                        {
                            imgRoot.Nodes.Add(new TreeNode($"{prop.Name}\v{prop.Value}"));
                        }
                        else
                        {
                            othersRoot.Nodes.Add(new TreeNode($"{prop.Name}\v{prop.Value}"));
                        }
                    }
                }
            }
            treeProp.Nodes.Clear();
            treeProp.Nodes.Add(fileRoot);
            if(imgRoot.Nodes.Count > 0)
                treeProp.Nodes.Add(imgRoot);
            if(othersRoot.Nodes.Count > 0)
                treeProp.Nodes.Add(othersRoot);
            treeProp.ExpandAll();
        }
    }
}
