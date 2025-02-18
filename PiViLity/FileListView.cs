using PiViLityCore.Shell;
using PiViLityCore.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static PiViLity.FileListView;

namespace PiViLity
{
    public class FileListItemData : PiViLityCore.Shell.IFileSystemItem
    {
        private string _path = "";
        public string Path
        {
            set { _path = value; }
            get=> _path;
        }
        public int LoadTileIconIndex = -1;
        public bool IsFile = false;

        public bool IsSpecialFolder => false;

        public Environment.SpecialFolder SpecialFolder => Environment.SpecialFolder.Desktop;

        public bool HasPath => true;


        public FileSystemInfo? GetFileSystemInfo()
        {
            return IsFile ? new FileInfo(Path) : new DirectoryInfo(Path);
        }
    }

    public class FileListView : ListView
    {
        string _path = "C:\\";
        private FileSystemWatcher _fsw = new();
        public enum DetailSubItem
        {
            None = 0,
            ModifiedDateTime = 1,
            Type = 2,
            Size = 4,
        }

        private IconStore _iconStore = new(true, true, true);
        private IconStoreThumbnail _thumbnailStore = new();
        private List<ListViewItem> _currentDirectoryItems = new List<ListViewItem>();
        private DetailSubItem _detailSubItems = DetailSubItem.None;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DetailSubItem DetailSubItems
        {
            get => _detailSubItems;
            set
            {
                if (_detailSubItems != value)
                {
                    _detailSubItems = value;
                    Columns.Clear();
                    var name = PiViLityCore.Global.GetResourceString(Resource.ResourceManager, "FileListDetailSubItem.Name");
                    HeaderStyle = ColumnHeaderStyle.Clickable;
                    var headColum = Columns.Add(name, 100, HorizontalAlignment.Left);
                    if (value.HasFlag(DetailSubItem.ModifiedDateTime))
                    {
                        Columns.Add(PiViLityCore.Global.GetResourceString(Resource.ResourceManager, "FileListDetailSubItem.ModifiedDateTime"));
                    }
                    if (value.HasFlag(DetailSubItem.Type))
                    {
                        Columns.Add(PiViLityCore.Global.GetResourceString(Resource.ResourceManager, "FileListDetailSubItem.Type"));
                    }
                    if (value.HasFlag(DetailSubItem.Size))
                    {
                        Columns.Add(PiViLityCore.Global.GetResourceString(Resource.ResourceManager, "FileListDetailSubItem.Size")).TextAlign = HorizontalAlignment.Right;
                    }

                    
                }
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path
        {
            get => _path; 
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RefreshList();
                }
            }
        }

        public FileListView()
        {
            DetailSubItems = DetailSubItem.ModifiedDateTime | DetailSubItem.Size | DetailSubItem.Type;
            LargeImageList = _iconStore.LargeIconList;
            SmallImageList = _iconStore.SmallIconList;
            if (!DesignMode)
            {
                TileSize = Setting.AppSettings.Instance.ThumbnailSize;
            }
            _thumbnailStore = new(TileSize);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

        }

        private void FileSystem_OnChanged(object sender, FileSystemEventArgs e)
        {
            PiViLityCore.Global.InvokeMainThread(() =>
            {
                RefreshList();
            });
        }
        private void FileSystem_OnRenamed(object sender, RenamedEventArgs e)
        {
            var targetItem = _currentDirectoryItems.Find(ItemActivate => (ItemActivate.Tag as FileListItemData)?.Path == e.OldFullPath);
            if(targetItem?.Tag is FileListItemData FileListItemData)
            {
                FileListItemData.Path = e.FullPath;
                Invoke(()=>targetItem.Text = e.Name);
            }
        }

        private void FileSystem_OnDeleted(object sender, FileSystemEventArgs e)
        {
            var targetItem = _currentDirectoryItems.Find(ItemActivate => (ItemActivate.Tag as FileListItemData)?.Path == e.FullPath);
            if (targetItem != null)
            {
                _currentDirectoryItems.Remove(targetItem);
                Invoke(() => Items.Remove(targetItem));
            }
        }
        private void FileSystem_OnCreated(object sender, FileSystemEventArgs e)
        {
            var fi = new FileInfo(e.FullPath);
            int i;
            for (i = 0; i < _currentDirectoryItems.Count; i++)
            {
                if (_currentDirectoryItems[i].Text.CompareTo(fi.Name) > 0)
                    break;
            }
            var newItem = makeItem(fi);
            if (newItem != null)
            {
                _currentDirectoryItems.Insert(i, newItem);
                Invoke(() =>
                {
                    int i;
                    for (i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].Text.CompareTo(fi.Name) > 0)
                            break;
                    }

                    Items.Insert(i, newItem);
                });

            }
        }

        ListViewItem? makeItem(FileSystemInfo? entry)
        {
            if (entry!=null && PiViLityCore.Global.settings.IsVisibleEntry(entry))
            {
                bool isFile = entry is FileInfo;
                var FileListItemData = new FileListItemData()
                {
                    Path = entry.FullName,
                    LoadTileIconIndex = -1,
                    IsFile = isFile
                };
                var item = new ListViewItem();
                item.Text = entry.Name;
                item.Tag = FileListItemData;

                if (!isFile)
                {
                    FileListItemData.LoadTileIconIndex = -3;
                    _iconStore.GetIcon(entry.FullName, index =>
                    {
                        item.ImageIndex = index;
                    });
                }

                var length = (entry as FileInfo)?.Length ?? 0;
                var lengthStr = isFile ? PiViLityCore.Util.String.GetEasyReadFileSizeF(length) : "";
                item.ToolTipText = $"{entry.Name}{entry.LastWriteTime}{lengthStr}";


                if (DetailSubItems.HasFlag(DetailSubItem.ModifiedDateTime))
                {
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, entry.LastWriteTime.ToString()));
                }
                if (DetailSubItems.HasFlag(DetailSubItem.Type))
                {
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, ""));
                }
                if (DetailSubItems.HasFlag(DetailSubItem.Size))
                {
                    if (!isFile)
                        item.SubItems.Add("");
                    else if (length < 1024)
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, $"{length} B"));
                    else
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, $"{length / 1024:N0} KB"));
                }
                return item;
            }
            return null;
        }

        private void RefreshList()
        {
            TileSize = Setting.AppSettings.Instance.ThumbnailSize;
            var path = _path;
            if (System.IO.Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                if (dirInfo != null)
                {
                    _fsw?.Dispose();
                    _fsw = new FileSystemWatcher(path);
                    _fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.Size;
                    _fsw.Changed += FileSystem_OnChanged;
                    _fsw.Renamed += FileSystem_OnRenamed;
                    _fsw.Deleted += FileSystem_OnDeleted;
                    _fsw.Created += FileSystem_OnCreated;

                    List<ListViewItem> list = new List<ListViewItem>();
                    Items.Clear();
                    _iconStore.Clear();
                    _thumbnailStore.Clear();
                    if(_thumbnailStore.ThumbnailSize != TileSize)
                    {
                        _thumbnailStore = new(TileSize);
                    }
                    var entries = dirInfo.EnumerateFileSystemInfos();
                    foreach (var entry in entries)
                    {
                        var item = makeItem(entry);
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                    _currentDirectoryItems = list;
                    Items.AddRange(list.ToArray());
                    _fsw.EnableRaisingEvents = true;
                }
            }
        }

        /// <summary>
        /// Viewプロパティをオーバーライドして、タイルビューの場合はサムネイルを描画します。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new View View 
        { 
            get => base.View;
            set
            {
                if(value==View.Tile)
                {
                    LargeImageList = _iconStore.JumboIconList;
                    OwnerDraw = true;
                }
                else
                {
                    LargeImageList = _iconStore.LargeIconList;
                    OwnerDraw = true;
                }
                base.View = value;
            }
        }

        /// <summary>
        /// DrawItemイベントを発生させます。
        /// タイルの場合のみサムネイルをオーナードローで描画します。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            //タイルの場合のみサムネイルをオーナードローで描画します。
            if (View == View.Tile)
            {
                if (e.Item.Tag is FileListItemData FileListItemData)
                {
                    //取得前の場合はロードを行います。
                    if (FileListItemData.LoadTileIconIndex == -1)
                    {
                        FileListItemData.LoadTileIconIndex = -2;
                        _thumbnailStore.GetThumbnailImage(FileListItemData.Path, index =>
                        {
                            if (index >= 0)
                            {
                                FileListItemData.LoadTileIconIndex = index;
                                Invalidate(e.Item.Bounds);
                            }
                            else
                            {
                                FileListItemData.LoadTileIconIndex = -3;
                                _iconStore.GetIcon(FileListItemData.Path, index =>
                                {
                                    e.Item.ImageIndex = index;
                                });
                            }
                        });
                    }
                    //取得できなかった場合はシステムアイコン表示に切り替えます
                    if (FileListItemData.LoadTileIconIndex == -3)
                    {
                        int x = e.Bounds.Left + (e.Bounds.Width - _iconStore.LargeIconList.ImageSize.Width) / 2;
                        int y = e.Bounds.Top + (e.Bounds.Height - _iconStore.LargeIconList.ImageSize.Height) / 2;
                        _iconStore.LargeIconList.Draw(e.Graphics, x, y, e.Item.ImageIndex);
                    }
                    //取得後の場合はローディングアイコンを描画します。
                    else if (FileListItemData.LoadTileIconIndex >= 0)
                    {
                        if (_thumbnailStore.ThumbnailSize == TileSize)
                        {
                            int x = e.Bounds.Left + (_thumbnailStore.ThumbnailSize.Width - e.Bounds.Width) / 2;
                            int y = e.Bounds.Top + (_thumbnailStore.ThumbnailSize.Height - e.Bounds.Height) / 2;
                            _thumbnailStore.ImageList.Draw(e.Graphics, x, y, FileListItemData.LoadTileIconIndex);
                        }
                        else
                        {
                            using (var image = _thumbnailStore.ImageList.Images[FileListItemData.LoadTileIconIndex])
                            {
                                e.Graphics.SetClip(e.Bounds);
                                int x = e.Bounds.Left + (e.Bounds.Width - image.Width) / 2;
                                int y = e.Bounds.Top + (e.Bounds.Height - image.Height) / 2;
                                e.Graphics.DrawImage(image, x, y, image.Width, image.Height);
                                e.Graphics.ResetClip();
                            }
                        }
                    }
                }

                e.DrawFocusRectangle();
                var plateBounds = new Rectangle(e.Bounds.Left, e.Bounds.Bottom-Font.Height, e.Bounds.Width, Font.Height);
                using (var brush = new SolidBrush(Color.FromArgb(192, 24, 24, 24)))
                {
                    if (brush != null)
                        e.Graphics.FillRectangle(brush, plateBounds);
                }

                e.DrawText(TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter | TextFormatFlags.TextBoxControl);
                base.OnDrawItem(e);
                return;
            }

            //タイル以外は通常の描画
            {
                //アイコンが未設定の場合は取得して設定します。
                if (e.Item.ImageIndex == -1)
                {
                    if (e.Item.Tag is FileListItemData FileListItemData)
                    {
                        _iconStore.GetIcon(FileListItemData.Path, index =>
                    {
                        e.Item.ImageIndex = index >= 0 ? index : -2;
                    });
                    }
                }

                //通常描画を親クラスに託す
                e.DrawDefault = true;
                base.OnDrawItem(e);
            }
        }

        /// <summary>
        /// DrawColumnHeaderイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        /// <summary>
        /// DrawSubItemイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawSubItem(e);
        }
    }

}
