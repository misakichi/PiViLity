using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Shell
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

    public class DirTreeNode : IFileSystemItem
    {
        Environment.SpecialFolder _spericalFolder = Environment.SpecialFolder.Desktop;

        DirTreeNodeType _type = DirTreeNodeType.None;
        public DirTreeNodeType Type
        { 
            get =>_type;
            set
            {
                if (_type != value)
                {

                    _type = value;
                }
            }
        }

        /// <summary lang="ja">
        /// 名前
        /// </summary>
        public string Name { get; private set; } = "";

        public object? Tag = null;

        /// <summary lang="ja">
        /// 子ノード
        /// サブディレクトリ、所属ドライブなど
        /// </summary>
        public List<DirTreeNode> Children { get; private set; } = new();

        public DirTreeNode? Parent { get; private set; } = null;

        public DirTree Tree;

        public bool IsCannotAccess = false;

        /// <summary lang="ja">
        /// フルパス
        /// </summary>
        public string Path
        {
            get; set;
        } = "";

        /// <summary lang="ja">
        /// 文字列化して返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (DriveInfo != null)
            {
                var name = Name == "" ? Util.Shell.DriveTypeName(DriveInfo.DriveType) : Name;
                var path = Path.Replace("\\", "");
                return $"{name}[{path}]";
            }

            return Name;
        }


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
        public bool IsSpecialFolder => _type >= DirTreeNodeType.SpecialFolderMyComputer;

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

        public DirTreeNode(DirTree dirTree)
        {
            Tree= dirTree;
        }
        public DirTreeNode(DirTreeNode parent, DirTree dirTree)
        {
            Parent = parent;
            Tree = dirTree;
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
                    if (Global.settings.IsVisibleDirectory(childDir) == false)
                        continue;
                    var child = new DirTreeNode(this,Tree);
                    child.SetType(DirTreeNodeType.DirectoryUnknown, childDir.FullName);
                    Children.Add(child);
                }
            }

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
                    DriveInfo = null;
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        if(drive.Name[0] == path[0])
                        {
                            DriveInfo = drive;
                            Path = drive.Name;
                            try
                            {
                                Name = drive.VolumeLabel;
                                AddUnknownChildren(new DirectoryInfo(path));
                            }
                            catch (IOException)
                            {

                            }
                        }
                    }
                    if(DriveInfo==null)
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
                            Name = dirInfo.Name;

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
                            Name = dirInfo.Name;

                            foreach (var childDir in dirInfo.EnumerateDirectories())
                            {
                                var dummyChild = new DirTreeNode(this, Tree);
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
                    Name = "Dummy";
                    Path = "dummy";
                    break;

                //PCの場合、PCであることさえわかればいい。
                //子にDriveを用意する
                case DirTreeNodeType.SpecialFolderMyComputer:
                    Name = "PC";
                    Path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                    Children.Clear();
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        var child = new DirTreeNode(this, Tree);
                        child.SetType(DirTreeNodeType.Drive, drive.Name);
                        Children.Add(child);
                    }

                    break;
            }
            Type = setType;
        }

        //子を含むこのノード以降に対して同一の処理を行う
        public void CurrentFirstTraversalTreeNodes(Action<DirTreeNode,int> action)
        {
            Queue<Tuple<DirTreeNode, int>> stack = new();
            stack.Enqueue(new Tuple<DirTreeNode, int>(this, 0));
            
            Tuple<DirTreeNode, int>? cur;
            while (stack.TryDequeue(out cur))
            {
                action(cur.Item1, cur.Item2);
                foreach (var child in cur.Item1.Children)
                    stack.Enqueue(new Tuple<DirTreeNode, int>(child, cur.Item2 + 1));
                
            }
        }
        public void DepthFirstTraversalTreeNodes(Action<DirTreeNode,int> action)
        {
            Queue<Tuple<DirTreeNode, int>> stack = new();
            stack.Enqueue(new Tuple<DirTreeNode, int>(this, 0));

            Tuple<DirTreeNode, int>? cur;
            while (stack.TryDequeue(out cur))
            {
                foreach (var child in cur.Item1.Children)
                    stack.Enqueue(new Tuple<DirTreeNode, int>(child, cur.Item2 + 1));
                
                action(cur.Item1, cur.Item2);
            }
        }

    }
    
}
