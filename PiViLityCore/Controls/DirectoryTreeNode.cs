using PiViLityCore.Option;
using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PiViLityCore.Controls
{
    public enum DirTreeNodeType
    {
        None,
        Unknown,
        Drive,
        Directory,
        DirectoryUnknown,
        Dummy,

        SpecialFolderMyComputer,  

    }

    public class DirectoryTreeNode : TreeNode, IFileSystemItem
    {
        Environment.SpecialFolder _spericalFolder = Environment.SpecialFolder.Desktop;

        public DirectoryTreeView DependencyDirectoryTree;

        public DirTreeNodeType Type { get; set; } = DirTreeNodeType.None;

        public DirectoryTreeNode(DirectoryTreeView dependencyDirectoryTree) 
            : base() 
        {
            DependencyDirectoryTree = dependencyDirectoryTree;
        }

        /// <summary lang="ja">
        /// 子ノード
        /// サブディレクトリ、所属ドライブなど
        /// </summary>
        public List<DirectoryTreeNode> Children { get; private set; } = new();

        public void RefreshChildren()
        {
            if (IsSpecialFolder)
            {
                DependencyDirectoryTree.IconStore.GetIcon(SpecialFolder,
                idx =>
                {
                    SelectedImageIndex = idx;
                    ImageIndex = idx;
                }
            );
            }
            else
            {
                DependencyDirectoryTree.IconStore.GetIcon(Path,
                    idx =>
                    {
                        SelectedImageIndex = idx;
                        ImageIndex = idx;
                    }
                );
            }
            if (DriveInfo != null)
            {
                var name = FileName == "" ? Util.Shell.DriveTypeName(DriveInfo.DriveType) : FileName;
                var later = Path.Replace("\\", "");
                Text = $"{name}[{later}]";
            }
            else
            {
                Text = FileName;
            }
            if (TreeView!=null)
            {
                TreeView.Visible = false;
                TreeView.BeginUpdate();
            }
            Nodes.Clear();
            Nodes.AddRange(Children.ToArray());
            if (TreeView != null)
            {
                TreeView.EndUpdate();
                TreeView.Visible = true;
            }
        }

        public bool IsCannotAccess = false;

        /// <summary lang="ja">
        /// フルパス
        /// </summary>
        public string Path
        {
            get; private set;
        } = "";

        public string FileName { get; private set; } = "";

        public string FileType { get; private set; } = "";

        public long Length => 0;

        /// <summary lang="ja">
        /// 文字列化して返す
        /// </summary>
        /// <returns></returns>
        //public override string ToString()
        //{
        //    if (DriveInfo != null)
        //    {
        //        var name = Name == "" ? Util.Shell.DriveTypeName(DriveInfo.DriveType) : FileName;
        //        var path = Path.Replace("\\", "");
        //        return $"{name}[{path}]";
        //    }
        //    return FileName;
        //}


        /// <summary lang="ja">
        /// ドライブの場合、ドライブ情報。それ以外はnull。
        /// </summary>
        public DriveInfo? DriveInfo
        {
            get; private set;
        } = null;

        /// <summary>
        /// 特殊フォルダかどうか
        /// </summary>
        public bool IsSpecialFolder => Type >= DirTreeNodeType.SpecialFolderMyComputer;

        /// <summary>
        /// 特殊フォルダの場合、その種類
        /// </summary>
        public Environment.SpecialFolder SpecialFolder => _spericalFolder;

        /// <summary>
        /// パスを持っているか
        /// </summary>
        public bool HasPath => Type == DirTreeNodeType.Drive || Type == DirTreeNodeType.Directory || Type == DirTreeNodeType.DirectoryUnknown || IsSpecialFolder;

        /// <summary>
        /// ファイル情報を取得する
        /// </summary>
        /// <returns></returns>
        public FileSystemInfo? GetFileSystemInfo()
        {
            if (HasPath)
            {
                try
                {
                    var fi = new DirectoryInfo(Path);
                    return fi;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }


        /// <summary lang="ja">
        /// タイプの変更
        /// 変更によって各種メンバや子ノードは再設定される
        /// </summary>
        /// <param name="SetType"></param>
        /// <param name="Path"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void SetType(DirTreeNodeType setType, string? path)
        {
            void AddUnknownChildren(DirectoryInfo dirInfo)
            {
                Children.Clear();
                foreach (var childDir in dirInfo.EnumerateDirectories())
                {
                    if (ShellSettings.Instance.IsVisibleDirectory(childDir) == false)
                        continue;
                    var child = new DirectoryTreeNode(DependencyDirectoryTree);
                    child.SetType(DirTreeNodeType.DirectoryUnknown, childDir.FullName);
                    Children.Add(child);
                }
            }

            Type = setType;
            switch (setType)
            {
                //使えないノード
                case DirTreeNodeType.None:
                case DirTreeNodeType.Unknown:
                    Path = "";
                    Children.Clear();
                    break;

                //ドライブの場合、Pathを元に情報を入れる
                //子があるかは調べるが、未調査状態とする
                case DirTreeNodeType.Drive:
                    if(path == null)
                        throw new ArgumentNullException(nameof(path));
                    if(path.Length==0)
                        throw new ArgumentOutOfRangeException(nameof(path));

                    Children.Clear();
                    DriveInfo? drive = null;
                    try
                    {
                        drive = new DriveInfo(path);
                    }
                    catch (Exception)
                    {
                        
                    }

                    foreach (var drv in DriveInfo.GetDrives())
                    {
                        if (drv.Name[0] == path[0])
                        {
                            drive = drv;
                        }
                    }
                    if (drive != null)
                    {
                        Path = drive.Name;
                        if (drive.DriveType == DriveType.Network)
                        {
                            Task.Run(() =>
                            {
                                if (Util.Shell.IsNetworkDriveAvailable(Path))
                                {
                                    FileName = drive.VolumeLabel;
                                    Global.InvokeMainThread(() => AddUnknownChildren(new DirectoryInfo(path)));
                                }
                            });
                            //PiViLityCore.Util.Shell.IsNetworkDriveAvailable(Path))
                        }
                        else
                        {
                            try
                            {
                                FileName = drive.VolumeLabel;
                                AddUnknownChildren(new DirectoryInfo(path));
                            }
                            catch (IOException)
                            {

                            }
                        }
                    }
                    DriveInfo = drive;
                    if (DriveInfo==null)
                        throw new DirectoryNotFoundException(nameof(Path));

                    break;

                //ディレクトリの場合、Pathをもとに情報をいれる
                //子があるかは調べるが、未調査状態とする
                case DirTreeNodeType.Directory:
                    {
                        if (path == null)
                            throw new ArgumentNullException(nameof(path));

                        try
                        {
                            var dirInfo = new DirectoryInfo(path);
                            Path = dirInfo.FullName;
                            FileName = dirInfo.Name;

                            AddUnknownChildren(dirInfo);
                        }
                        catch (UnauthorizedAccessException )
                        {
                            //アクセス不可
                            IsCannotAccess = true;
                        }

                    }
                    break;

                //不明ディレクトリの場合、Pathをもとに情報をいれる
                //が、子については調べてない
                case DirTreeNodeType.DirectoryUnknown:
                    {
                        if (path == null)
                            throw new ArgumentNullException(nameof(path));
                        Children.Clear();
                        try
                        {
                            var dirInfo = new DirectoryInfo(path);
                            Path = dirInfo.FullName;
                            FileName = dirInfo.Name;

                            foreach (var childDir in dirInfo.EnumerateDirectories())
                            {
                                var dummyChild = new DirectoryTreeNode(DependencyDirectoryTree);
                                dummyChild.SetType(DirTreeNodeType.Dummy, null);
                                Children.Add(dummyChild);
                                break;
                            }
                        }
                        catch(UnauthorizedAccessException /*unauthorizedAccessEx*/)
                        {
                            //アクセス不可
                            IsCannotAccess = true;
                        }

                    }
                    break;

                case DirTreeNodeType.Dummy:
                    FileName = "Dummy";
                    Path = "dummy";
                    break;

                //PCの場合、PCであることさえわかればいい。
                //子にDriveを用意する
                case DirTreeNodeType.SpecialFolderMyComputer:
                    _spericalFolder = Environment.SpecialFolder.MyComputer;
                    FileName = "PC";
                    Path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                    Children.Clear();
                    foreach (var drv in DriveInfo.GetDrives())
                    {
                        var child = new DirectoryTreeNode(DependencyDirectoryTree);
                        child.SetType(DirTreeNodeType.Drive, drv.Name);
                        Children.Add(child);
                    }
                    break;
            }

            RefreshChildren();
            FileType = PiVilityNative.FileInfo.GetFileTypeName(Path);
        }

        //子を含むこのノード以降に対して同一の処理を行う
        public void CurrentFirstTraversalTreeNodes(Action<DirectoryTreeNode,int> action)
        {
            Queue<Tuple<DirectoryTreeNode, int>> stack = new();
            stack.Enqueue(new Tuple<DirectoryTreeNode, int>(this, 0));
            
            Tuple<DirectoryTreeNode, int>? cur;
            while (stack.TryDequeue(out cur))
            {
                action(cur.Item1, cur.Item2);
                foreach (var child in cur.Item1.Children)
                    stack.Enqueue(new Tuple<DirectoryTreeNode, int>(child, cur.Item2 + 1));
                
            }
        }
        public void DepthFirstTraversalTreeNodes(Action<DirectoryTreeNode,int> action)
        {
            Queue<Tuple<DirectoryTreeNode, int>> stack = new();
            stack.Enqueue(new Tuple<DirectoryTreeNode, int>(this, 0));

            Tuple<DirectoryTreeNode, int>? cur;
            while (stack.TryDequeue(out cur))
            {
                foreach (var child in cur.Item1.Children)
                    stack.Enqueue(new Tuple<DirectoryTreeNode, int>(child, cur.Item2 + 1));
                
                action(cur.Item1, cur.Item2);
            }
        }

        public bool CheckAndAnalyzeUnknownDirectory()
        {
            if (base.TreeView == null)
                return false;

            if (Type == DirTreeNodeType.DirectoryUnknown)
            {
                List<TreeNode> addList = new();
                SetType(DirTreeNodeType.Directory, Path);
                RefreshChildren();
                return true;
            }
            return false;
        }

        //深さ優先でノードを探す
        public DirectoryTreeNode? SearchDepthNode(string path)
        {
            path = path.ToLower();
            CheckAndAnalyzeUnknownDirectory();
            DirectoryTreeNode? matchNode = null;
            int maxMatchLength = 0;
            Children.ForEach(node =>
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
            if (string.Compare(path, matchNode.Path, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return matchNode;
            }
            else
            {
                return matchNode.SearchDepthNode(path);
            }
        }

    }

}
