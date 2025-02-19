using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Controls
{
    [Flags]
    public enum FileListViewSubItemTypes
    {
        None = 0,
        ModifiedDateTime = 1,
        Type = 2,
        Size = 4,

        All = ModifiedDateTime | Type | Size
    }

    /// <summary>
    /// ファイルリスト用ListView(FileListView)のアイテムデータ
    /// </summary>
    public class FileListViewItem : ListViewItem, IFileSystemItem
    {
        public FileListViewItem(string path) 
        {
            if(System.IO.File.Exists(path))
            {
                RefreshItems(new System.IO.FileInfo(path));
            }
            else if (System.IO.Directory.Exists(path))
            {
                RefreshItems(new System.IO.DirectoryInfo(path));
            }
        }
        public FileListViewItem(FileSystemInfo fsi) : this(fsi, FileListViewSubItemTypes.All)
        {
        }

        public FileListViewItem(FileSystemInfo fsi, FileListViewSubItemTypes subItemType)
        {
            _fileListViewSubItemType = subItemType;
            RefreshItems(fsi);
        }

        private void RefreshItems(FileSystemInfo fsi)
        {
            _path = fsi.FullName;
            Text = fsi.Name;
            var fi = fsi as FileInfo;
            IsFile = fi != null;

            var length = fi?.Length ?? 0;
            var lengthStr = IsFile ? PiViLityCore.Util.String.GetEasyReadFileSizeF(length) : "";
            ToolTipText = $"{Name}{fsi.LastWriteTime}{lengthStr}";

            ModifiedDateTime = fsi.LastWriteTime;
            Length = length;
            FileType = fi?.Extension ?? "";

            if (fi!=null)
            { 
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.ModifiedDateTime))
                {
                    SubItems.Add(fi.LastWriteTime.ToString());
                }
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.Type))
                {
                    SubItems.Add(fi.Extension);
                }
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.Size))
                { 
                    if (fi.Length < 1024)
                        SubItems.Add(new ListViewItem.ListViewSubItem(this, $"{fi.Length} B"));
                    else
                        SubItems.Add(new ListViewItem.ListViewSubItem(this, $"{fi.Length / 1024:N0} KB"));
                }
            }
            else
            {
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.ModifiedDateTime))
                    SubItems.Add(fsi.LastWriteTime.ToString());
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.Type))
                    SubItems.Add("");
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.Size))
                    SubItems.Add("");
            }
        }

        private void RefreshItems()
        {
            if (System.IO.File.Exists(_path))
            {
                RefreshItems(new System.IO.FileInfo(_path));
            }
            else if (System.IO.Directory.Exists(_path))
            {
                RefreshItems(new System.IO.DirectoryInfo(_path));
            }
        }

        private FileListViewSubItemTypes _fileListViewSubItemType;
        public FileListViewSubItemTypes FileListViewSubItemType
        {
            get => _fileListViewSubItemType;
            set
            {
                _fileListViewSubItemType = value;
            }
        }



        private string _path = "";
        public string Path
        {
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RefreshItems();
                }
            }
            get => _path;
        }

        public DateTime ModifiedDateTime { get; private set; } = DateTime.MinValue;
        public long Length { get; private set; } = 0;
        public string FileType { get; private set; } = "";

        /// <summary>
        /// タイルアイコンのインデックス
        /// </summary>
        public int LoadTileIconIndex = -1;

        /// <summary>
        /// ファイルかどうか
        /// </summary>
        public bool IsFile = false;

        /// <summary>
        /// 特殊フォルダかどうか
        /// </summary>
        public bool IsSpecialFolder => false;

        /// <summary>
        /// 特殊フォルダの場合のフォルダ
        /// </summary>
        public Environment.SpecialFolder SpecialFolder => Environment.SpecialFolder.Desktop;

        /// <summary>
        /// パスがあるかどうか
        /// </summary>
        public bool HasPath => true;

        /// <summary>
        /// ファイルシステム情報を取得する
        /// </summary>
        /// <returns></returns>
        public FileSystemInfo? GetFileSystemInfo()
        {
            return IsFile ? new FileInfo(Path) : new DirectoryInfo(Path);
        }
    }

    class FileListViewItemComparer : System.Collections.IComparer
    {
        public int Compare(object? x, object? y)
        {
            if (x is FileListViewItem item1 && y is FileListViewItem item2)
            {
                if (item1.IsSpecialFolder && !item2.IsSpecialFolder)
                    return -1;
                if (!item1.IsSpecialFolder && item2.IsSpecialFolder)
                    return 1;
                if (item1.IsFile && !item2.IsFile)
                    return 1;
                if (!item1.IsFile && item2.IsFile)
                    return -1;
                return string.Compare(item1.Text, item2.Text);
            }
            return 0;
        }
    }
}
