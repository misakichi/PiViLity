using Microsoft.VisualBasic.FileIO;
using PiViLityCore.Option;
using PiViLityCore.Shell;
using PiViLityCore.Util;
using PiVilityNative;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PiViLityCore.Controls
{
    public class FileListViewItemEventArgs : EventArgs
    {
        public FileListViewItem Item { get; }
        public bool Default { get; set; } = true;

        public FileListViewItemEventArgs(FileListViewItem item) : base()
        {
            Item = item;
        }
    }
    /// <summary>
    /// ファイル一覧用ListView
    /// </summary>
    public class FileListView : ListView
    {
        public delegate void FileListViewItemEventHandler(FileListViewItemEventArgs item);
        public event FileListViewItemEventHandler? ItemDoubleClick;
        public event EventHandler? DirectoryChanged;
        /// <summary>
        /// ファイルリストのサブアイテムの種類
        /// </summary>
        private class FileListColumnHeader : ColumnHeader
        {
            public FileListViewSubItemBit SubItemBit;
            public System.Collections.IComparer? Comparer;
        }

        //constant variables
        private const int DefaultSubItemWidth = 200;

        //private fields
        private string _path = "C:\\";
        private FileSystemWatcher _fsw = new();
        private IconStoreSystem _iconStore;
        public IIconStore? ThumbnailIconStore = null;
        private List<FileListViewItem> _currentDirectoryItems = new ();
        private List<FileListViewItem> _currentViewItems = new();
        private FileListViewSubItemTypes _detailSubItems = FileListViewSubItemTypes.Name;
        private FileListColumnHeader[] SubItemColums = new FileListColumnHeader[(int)FileListViewSubItemBit.Max];
        private int _activeSortIndex = 0;
        private int _dragEnterKeyState = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileListView()
        {
            CreateIconStore();
            _iconStore =new(true,true, true, null, null);

            for (int i = 0; i < SubItemColums.Length; i++)
            {
                var subItemName = ((FileListViewSubItemBit)i).ToString();
                SubItemColums[i] = new()
                {
                    Name = subItemName,
                    Text = PiViLityCore.Global.GetResourceString($"FileListDetailSubItem.{subItemName}"),
                    SubItemBit = (FileListViewSubItemBit)i,
                    Comparer = FileListViewItemComparerBase.CreateComparer(this, (FileListViewSubItemTypes)(1 << i)),
                    Width = DefaultSubItemWidth,
                    TextAlign = HorizontalAlignment.Left
                };
            }
            SubItemColums[(int)FileListViewSubItemBit.Size].TextAlign = HorizontalAlignment.Right;

            ListViewItemSorter = new FileListViewItemComparerName(this);
            Sorting = SortOrder.Ascending;
            DetailSubItems = FileListViewSubItemTypes.All;
            LargeImageList = _iconStore.LargeIconList;
            SmallImageList = _iconStore.SmallIconList;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            DoubleBuffered = true;
            PiViLityCore.Event.Option.ApplySettings += OnApplySettings;
        }

        void CreateIconStore()
        {
            var c = Math.Min(Option.ShellSettings.Instance.ThumbnailSize.Width, Option.ShellSettings.Instance.ThumbnailSize.Height);
            var tileSize = new Size(c,c);
            _iconStore = new IconStoreSystem(true, true, true, null, null, tileSize);
            if (View == View.Tile)
            {
                LargeImageList = _iconStore.TileIconList;
            }
            else
            {
                LargeImageList = _iconStore.LargeIconList;
            }
            SmallImageList = _iconStore.SmallIconList;
        }
        private void OnApplySettings(object? sender, EventArgs e)
        {
            var c = Math.Min(Option.ShellSettings.Instance.ThumbnailSize.Width, Option.ShellSettings.Instance.ThumbnailSize.Height);
            var tileSize = new Size(c, c);
            if (_iconStore.TileIconList.ImageSize.Equals(tileSize) == false)
            {
                _iconStore.TileIconList.Images.Clear();
                _iconStore.TileIconList.ImageSize = tileSize;
                if (View == View.Tile)
                {
                    TileSize = Option.ShellSettings.Instance.ThumbnailSize;
                    RefreshFileList();
                }
            }
        }


        /// <summary>
        /// ファイルリストのサブアイテムの種類を取得または設定します。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileListViewSubItemTypes DetailSubItems
        {
            get => _detailSubItems;
            set
            {
                //Nameは必ず設定される
                value |= FileListViewSubItemTypes.Name;

                if (_detailSubItems != value)
                {
                    _detailSubItems = value;
                    RefreshSubItems();
                }
            }
        }

        /// <summary>
        /// サブアイテムを再設定します。
        /// </summary>
        private void RefreshSubItems()
        {
            //設定が変更された場合はサブアイテムを再設定します。
            Columns.Clear();

            var headColum = Columns.Add(SubItemColums[(int)FileListViewSubItemBit.Name]);
            if (_detailSubItems.HasFlag(FileListViewSubItemTypes.ModifiedDateTime))
            {
                Columns.Add(SubItemColums[(int)FileListViewSubItemBit.ModifiedDateTime]);
            }
            if (_detailSubItems.HasFlag(FileListViewSubItemTypes.Type))
            {
                Columns.Add(SubItemColums[(int)FileListViewSubItemBit.Type]);
            }
            if (_detailSubItems.HasFlag(FileListViewSubItemTypes.Size))
            {
                var sub = Columns.Add(SubItemColums[(int)FileListViewSubItemBit.Size]);
            }
            
            _currentDirectoryItems.ForEach(item => item.FileListViewSubItemType = View != View.Tile ? _detailSubItems : FileListViewSubItemTypes.Name);
        }

        protected override void DestroyHandle()
        {
            _fsw?.Dispose();
            if (PiViLityCore.Event.Option.ApplySettings != null)
            {
#pragma warning disable CS8601
                PiViLityCore.Event.Option.ApplySettings -= OnApplySettings;
#pragma warning restore CS8601
            }
            base.DestroyHandle();

        }

        /// <summary>
        /// ファイルリストの表示パスを取得または設定します。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path
        {
            get => _path; 
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RemakeFileList();
                    DirectoryChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 現在表示中のPathの中にあるなんらかのステータス内容に変更があった場合に発生するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystem_OnChanged(object sender, FileSystemEventArgs e)
        {
            PiViLityCore.Global.InvokeMainThread(() =>
            {
                RemakeFileList();
            });
        }

        /// <summary>
        /// 現在表示中のPathの中にある項目に名称変更があった場合に発生するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystem_OnRenamed(object sender, RenamedEventArgs e)
        {
            var targetItem = _currentDirectoryItems.Find(item => item.Path == e.OldFullPath);
            if(targetItem!=null)
            {
                Invoke(() => targetItem.Path = e.FullPath);
            }
        }

        /// <summary>
        /// 現在表示中のPathの中にある項目が削除された場合に発生するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystem_OnDeleted(object sender, FileSystemEventArgs e)
        {
            var targetItem = _currentDirectoryItems.Find(item => item.Path == e.FullPath);
            if (targetItem != null)
            {
                _currentDirectoryItems.Remove(targetItem);
                Invoke(() => Items.Remove(targetItem));
            }
        }

        /// <summary>
        /// 現在表示中のPathの中にある項目が新規作成された場合に発生するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystem_OnCreated(object sender, FileSystemEventArgs e)
        {
            var fi = new System.IO.FileInfo(e.FullPath);
            int i;
            for (i = 0; i < _currentDirectoryItems.Count; i++)
            {
                if (_currentDirectoryItems[i].Text.CompareTo(fi.Name) > 0)
                    break;
            }
            var newItem = MakeItem(fi);
            if (newItem != null)
            {
                _currentDirectoryItems.Insert(i, newItem);
                Invoke(() =>
                {
                    //int i;
                    //for (i = 0; i < Items.Count; i++)
                    //{
                    //    if (Items[i].Text.CompareTo(fi.Name) > 0)
                    //        break;
                    //}
                    Items.Add(newItem);
                });

            }
        }

        /// <summary>
        /// ファイルリストのアイテムを作成します。
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private FileListViewItem? MakeItem(FileSystemInfo? entry)
        {
            if (entry!=null && PiViLityCore.Global.settings.IsVisibleEntry(entry))
            {
                bool isFile = entry is System.IO.FileInfo;
                var item = new FileListViewItem(entry, DetailSubItems);

                if (!isFile)
                {
                    item.LoadTileIconIndex = -3;
                    _iconStore.GetIcon(entry.FullName, index =>
                    {
                        item.ImageIndex = index;
                    });
                }
                return item;
            }
            return null;
        }

        /// <summary>
        /// ファイルリストを再作成します。
        /// </summary>
        private void RemakeFileList()
        {
            var path = _path;
            if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                if (dirInfo != null)
                {
                    _fsw?.Dispose();
                    _fsw = new FileSystemWatcher(path);
                    _fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.LastWrite | NotifyFilters.Size;
                    _fsw.Changed += FileSystem_OnChanged;
                    _fsw.Renamed += FileSystem_OnRenamed;
                    _fsw.Deleted += FileSystem_OnDeleted;
                    _fsw.Created += FileSystem_OnCreated;

                    List<FileListViewItem> list = new();
                    _iconStore.Clear();
                    ThumbnailIconStore?.Clear();
                    var entries = dirInfo.EnumerateFileSystemInfos();
                    foreach (var entry in entries)
                    {
                        var item = MakeItem(entry);
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                    _currentDirectoryItems = list;
                    _currentViewItems = list;
                    RefreshFileList();
                    _fsw.EnableRaisingEvents = true;
                }
            }
        }

        /// <summary>
        /// ファイルリストを更新します。
        /// </summary>
        private void RefreshFileList()
        {
            List<ListViewItem> preSelected = new();
            foreach (ListViewItem item in SelectedItems)
            {
                preSelected.Add(item);
            }
            ;
            BeginUpdate();
            Items.Clear();
            Items.AddRange(_currentViewItems.ToArray());
            preSelected.ForEach(item =>item.Selected = true);
            preSelected.Find(item=> _currentViewItems.Contains(item))?.EnsureVisible();
            EndUpdate();
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
                if (base.View != value)
                {
                    if (value == View.Tile)
                    {
                        LargeImageList = _iconStore.TileIconList;
                        OwnerDraw = true;
                    }
                    else
                    {
                        LargeImageList = _iconStore.LargeIconList;
                        OwnerDraw = true;
                    }
                    base.View = value;
                    RefreshSubItems();
                }
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
            if (View == View.Tile && ThumbnailIconStore != null)
            {
                if (e.Item is FileListViewItem FileListViewItem)
                {
                    //取得前の場合はロードを行います。
                    if (FileListViewItem.LoadTileIconIndex == -1)
                    {
                        FileListViewItem.LoadTileIconIndex = -2;
                        ThumbnailIconStore.GetIcon(FileListViewItem.Path, index =>
                        {
                            if (index >= 0)
                            {
                                FileListViewItem.LoadTileIconIndex = index;
                                Invalidate(e.Item.Bounds);
                            }
                            else
                            {
                                FileListViewItem.LoadTileIconIndex = -3;
                                _iconStore.GetIcon(FileListViewItem.Path, index =>
                                {
                                    e.Item.ImageIndex = index;
                                });
                            }
                        });
                    }
                    //取得できなかった場合はシステムアイコン表示に切り替えます
                    if (FileListViewItem.LoadTileIconIndex == -3)
                    {
                        int x = e.Bounds.Left + (e.Bounds.Width - _iconStore.LargeIconList.ImageSize.Width) / 2;
                        int y = e.Bounds.Top + (e.Bounds.Height - _iconStore.LargeIconList.ImageSize.Height) / 2;
                        _iconStore.LargeIconList.Draw(e.Graphics, x, y, e.Item.ImageIndex);
                    }
                    //取得後の場合はローディングアイコンを描画します。
                    else if (FileListViewItem.LoadTileIconIndex >= 0)
                    {
                        if (ThumbnailIconStore.TileIconList.ImageSize == TileSize)
                        {
                            int x = e.Bounds.Left + (TileSize.Width - e.Bounds.Width) / 2;
                            int y = e.Bounds.Top + (TileSize.Height - e.Bounds.Height) / 2;
                            ThumbnailIconStore.TileIconList.Draw(e.Graphics, x, y, FileListViewItem.LoadTileIconIndex);
                        }
                        else
                        {
                            using var image = ThumbnailIconStore.TileIconList.Images[FileListViewItem.LoadTileIconIndex];
                            e.Graphics.SetClip(e.Bounds);
                            int x = e.Bounds.Left + (e.Bounds.Width - image.Width) / 2;
                            int y = e.Bounds.Top + (e.Bounds.Height - image.Height) / 2;
                            e.Graphics.DrawImage(image, x, y, image.Width, image.Height);
                            e.Graphics.ResetClip();
                        }
                    }
                }

                if(e.Item.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 64, 64, 160)), e.Bounds);
                }

                e.DrawFocusRectangle();
                
                var plateBounds = new Rectangle(e.Bounds.Left, e.Bounds.Bottom-Font.Height, e.Bounds.Width, Font.Height);
                using (var brush = new SolidBrush(Color.FromArgb(192, 24, 24, 24)))
                {
                    if (brush != null)
                        e.Graphics.FillRectangle(brush, plateBounds);
                }

                e.DrawText(TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis);
                base.OnDrawItem(e);
                return;
            }

            //タイル以外は通常の描画
            {
                //アイコンが未設定の場合は取得して設定します。
                if (e.Item.ImageIndex == -1)
                {
                    if (e.Item is FileListViewItem FileListViewItem)
                    {
                        _iconStore.GetIcon(FileListViewItem.Path, index =>
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
        /// カルムヘッダクリック時（ソート順変更）
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            BeginUpdate();
            if(_activeSortIndex!= e.Column)
            {
                _activeSortIndex = e.Column;
                ListViewItemSorter = SubItemColums[_activeSortIndex].Comparer;
                Sorting = SortOrder.Ascending;
            }
            else
            {
                Sorting = Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            EndUpdate();
        }
        /// <summary>
        /// DrawColumnHeaderイベントを発生させます。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            //e.DrawDefault = true;
            //base.OnDrawColumnHeader(e);
            //e.DrawDefault = false; ;
            e.DrawBackground();
            e.DrawText();
            if (e.ColumnIndex == _activeSortIndex)
            {
                if (Sorting == SortOrder.Ascending)
                {
                    e.Graphics.DrawLines(Pens.Gray, new Point[] {
                        new(e.Bounds.Left + e.Bounds.Width / 2, e.Bounds.Top + 1),
                        new(e.Bounds.Left + e.Bounds.Width / 2 -10, e.Bounds.Top + 5),
                        new(e.Bounds.Left + e.Bounds.Width / 2, e.Bounds.Top + 1),
                        new(e.Bounds.Left + e.Bounds.Width / 2 + 10, e.Bounds.Top + 5),
                    });
                }
                else
                {
                    e.Graphics.DrawLines(Pens.Gray, new Point[] {
                        new(e.Bounds.Left + e.Bounds.Width / 2, e.Bounds.Top + 5),
                        new(e.Bounds.Left + e.Bounds.Width / 2 -10, e.Bounds.Top + 1),
                        new(e.Bounds.Left + e.Bounds.Width / 2, e.Bounds.Top + 5),
                        new(e.Bounds.Left + e.Bounds.Width / 2 + 10, e.Bounds.Top + 1),
                    });
                }
            }
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

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (SelectedItems.Count == 1)
            {
                if (SelectedItems[0] is FileListViewItem item)
                {
                    FileListViewItemEventArgs args = new(item);
                    ItemDoubleClick?.Invoke(args);
                    if (args.Default)
                    {
                        if (item.IsFile)
                        {

                        }
                        else
                        {
                            Path = item.Path;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// MouseClickイベントを処理し、必要に応じてファイルの右クリックメニューを表示します。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // lsvFileの右クリックイベント処理
            if (e.Button == MouseButtons.Right)
            {
                List<string> list = new();
                foreach (ListViewItem? item in SelectedItems)
                {
                    if (item is FileListViewItem data)
                    {
                        list.Add(data.Path);
                    }
                }
                if (list.Count > 0)
                {
                    List< CustomMenuItem> menuList = new();
                    bool hasDirectory = false;
                    foreach (var path in list)
                    {
                        if (System.IO.Directory.Exists(path))
                        {
                            hasDirectory = true;
                        }
                    }
                    if (hasDirectory)
                    {
                        menuList.Add(new CustomMenuItem()
                        {
                            name = "タブで開く",
                            isDefault = true,
                            action = () =>
                            {
                                var path = list[0];
                                //var newTab = new FileListView();
                                //newTab.Path = path;
                                //newTab.RestoreSettings(new FileListViewSettings());
                                //PiViLityCore.Global.MainForm.AddTab(newTab);
                                MessageBox.Show($"タブで開きたい({path})");
                            }
                        });

                    }
                    // ファイルの右クリックメニューを表示
                    var screen = PointToScreen(e.Location);
                    PiVilityNative.ShellAPI.ShowShellContextMenu(list.ToArray(), Handle, screen.X, screen.Y, menuList.ToArray());
                }
            }
            base.OnMouseClick(e);
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if (e.Item is IFileSystemItem isi)
            {
                if (isi.HasPath)
                {
                    var paths = new string[1];
                    var path = isi.Path;
                    if (Directory.Exists(path) || File.Exists(path))
                    {
                        paths[0] = path;
                        DoDragDrop(new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
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
                var item = test?.Item;
                if (item is IFileSystemItem isi&& isi.HasPath)
                {
                    var isDir = PiViLityCore.Util.Shell.IsDirectory(isi.Path);
                    var isExe = PiViLityCore.Util.Shell.IsExecute(isi.Path);
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
                                        PiViLityCore.Util.Shell.Copy(srcPath, isi.Path);
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Shift))
                                    {
                                        PiViLityCore.Util.Shell.Move(srcPath, isi.Path);
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Alt))
                                    {
                                        PiViLityCore.Util.Shell.CreateShortCut(srcPath, isi.Path, "new shortcut");
                                    }
                                    else
                                    {
                                        PiViLityCore.Util.Shell.Move(srcPath, isi.Path);
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
        /// 設定復元
        /// </summary>
        /// <param name="fileView"></param>
        public void RestoreSettings(FileListViewSettings fileView)
        {
            for(int i = 0; i < SubItemColums.Length; i++)
            {
                if (fileView.SubItemWidth.Length > i)
                {
                    if (fileView.SubItemWidth[i] > 0)
                    {
                        SubItemColums[i].Width = fileView.SubItemWidth[i];
                    }
                }
            }
            Path = fileView.Path;
        }

        /// <summary>
        /// 設定保存
        /// </summary>
        /// <param name="fileView"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SaveSettings(FileListViewSettings fileView)
        {
            fileView.SubItemWidth = new int[SubItemColums.Length];
            for (int i = 0; i < SubItemColums.Length; i++)
            {
                fileView.SubItemWidth[i] = SubItemColums[i].Width;
            }
            fileView.Path = Path;
        }
    }

    public class FileListViewSettings
    {
        [OptionItem]
        public string Path = SpecialDirectories.MyPictures;

        [OptionItem]
        public int[] SubItemWidth = new int[(int)FileListViewSubItemBit.Max];
    }


}
