using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Controls
{
    /// <summary>
    /// ファイルリスト用ListView(FileListView)のアイテムデータ
    /// </summary>
    public class FileListViewItem : ListViewItem, IFileSystemItem
    {
        private FileListViewSubItemTypes _fileListViewSubItemType;
        private string _path = "";
        public string FileName { get; private set; } = "";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path"></param>
        public FileListViewItem(string path)
        {
            RefreshItems(path);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fsi"></param>
        public FileListViewItem(FileSystemInfo fsi) : this(fsi, FileListViewSubItemTypes.All)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fsi"></param>
        /// <param name="subItemType"></param>
        public FileListViewItem(FileSystemInfo fsi, FileListViewSubItemTypes subItemType)
        {
            _fileListViewSubItemType = subItemType;
            RefreshItems(fsi);
        }

        /// <summary>
        /// アイテムを更新する
        /// </summary>
        /// <param name="fsi"></param>
        private void RefreshItems(FileSystemInfo fsi)
        {
            _path = fsi.FullName;
            FileName = fsi.Name;
            Text = fsi.Name;
            var fi = fsi as FileInfo;
            IsFile = fi != null;

            var length = fi?.Length ?? 0;
            var lengthStr = IsFile ? PiViLityCore.Util.String.GetEasyReadFileSizeF(length) : "";
            ToolTipText = $"{Name}{fsi.LastWriteTime}{lengthStr}";

            ModifiedDateTime = fsi.LastWriteTime;
            Length = length;
            FileType = PiVilityNative.FileInfo.GetFileTypeName(_path);

            if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.ModifiedDateTime))
            {
                SubItems.Add(fsi.LastWriteTime.ToString("yyyy/MM/dd H:mm", CultureInfo.CurrentCulture)/*$"{fsi.LastWriteTime:yyyy/MM/dd H:m}"*/);
            }
            if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.Type))
            {
                SubItems.Add(FileType);
            }
            if (fi != null)
            {
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.Size))
                {
                    SubItems.Add(new ListViewItem.ListViewSubItem(this, Util.String.GetEasyReadFileSize(length, false)));
                }
            }
            else
            {
                if (_fileListViewSubItemType.HasFlag(FileListViewSubItemTypes.Size))
                    SubItems.Add("");
            }
        }

        /// <summary>
        /// アイテムを更新する
        /// </summary>
        private void RefreshItems(string path)
        {
            if (System.IO.File.Exists(path))
            {
                RefreshItems(new System.IO.FileInfo(path));
            }
            else if (System.IO.Directory.Exists(path))
            {
                RefreshItems(new System.IO.DirectoryInfo(path));
            }
        }

        public FileListViewSubItemTypes FileListViewSubItemType
        {
            get => _fileListViewSubItemType;
            set
            {
                _fileListViewSubItemType = value;
            }
        }

        /// <summary>
        /// パス
        /// </summary>
        public string Path
        {
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RefreshItems(_path);
                }
            }
            get => _path;
        }
        public string FileType { get; private set; } = "";

        public DateTime ModifiedDateTime { get; private set; } = DateTime.MinValue;
        public long Length { get; protected set; } = 0;

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

    internal enum FileListViewSubItemBit : int
    {
        Name = 0,
        ModifiedDateTime,
        Type,
        Size,
        Max,

    }
    [Flags]
    public enum FileListViewSubItemTypes
    {
        Name = 1 << FileListViewSubItemBit.Name,
        ModifiedDateTime = 1 << FileListViewSubItemBit.ModifiedDateTime,
        Type = 1 << FileListViewSubItemBit.Type,
        Size = 1 << FileListViewSubItemBit.Size,

        All = ModifiedDateTime | Type | Size
    }


}
