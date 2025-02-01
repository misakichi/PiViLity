using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity
{
    public class FileListView : ListView
    {
        string _path = "C:\\";

        public enum DetailSubItem
        {
            None = 0,
            ModifiedDateTime = 1,
            Type = 2,
            Size = 4,
        }
        public class ItemData
        {
            public string Path = "";
            public int LoadTileIconIndex = -1;
        }
        IconStore _iconStore = new(true, true, true);
        IconStore _tileIconStore;

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
                    Columns.Add(PiViLityCore.Global.GetResourceString(Resource.ResourceManager, "FileListDetailSubItem.Name"));
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
                        Columns.Add(PiViLityCore.Global.GetResourceString(Resource.ResourceManager, "FileListDetailSubItem.Size"));
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
            _tileIconStore = new(false, false, true, null, null, TileSize);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        private void RefreshList()
        {
            var path = _path;
            if (System.IO.Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                if (dirInfo != null)
                {
                    List<ListViewItem> list = new List<ListViewItem>();
                    Items.Clear();
                    _iconStore.Clear();
                    _tileIconStore.Clear();
                    if(_tileIconStore.JumboIconList.ImageSize != TileSize)
                    {
                        _tileIconStore = new IconStore(false, false, true, null, null, TileSize);
                    }
                    var files = dirInfo.EnumerateFiles();
                    foreach (var file in files)
                    {
                        if (PiViLityCore.Global.settings.IsVisibleFile(file))
                        {
                            var item = new ListViewItem();
                            item.Text = file.Name;
                            item.Tag = new ItemData() {
                                Path = file.FullName,
                                LoadTileIconIndex = -1
                            };


                            if (DetailSubItems.HasFlag(DetailSubItem.ModifiedDateTime))
                            {
                                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, file.LastWriteTime.ToString()));
                            }
                            if (DetailSubItems.HasFlag(DetailSubItem.Type))
                            {
                                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, ""));
                            }
                            if (DetailSubItems.HasFlag(DetailSubItem.Size))
                            {
                                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, $"{PiViLityCore.Util.String.GetEasyReadFileSize(file.Length)}"));
                            }
                            list.Add(item);
                        }
                    }
                    Items.AddRange(list.ToArray());                    
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
                if (e.Item.Tag is ItemData itemData)
                {
                    //取得前の場合はロードを行います。
                    if (itemData.LoadTileIconIndex == -1)
                    {
                        itemData.LoadTileIconIndex = -2;
                        _tileIconStore.GetIconIndexNoSys(itemData.Path, index =>
                        {
                            if (index >= 0)
                            {
                                itemData.LoadTileIconIndex = index;
                                Invalidate(e.Item.Bounds);
                            }
                            else
                            {
                                itemData.LoadTileIconIndex = -3;
                                _iconStore.GetIconIndexSysSync(itemData.Path, index =>
                                {
                                    e.Item.ImageIndex = index;
                                });
                            }
                        });
                    }
                    //取得できなかった場合はシステムアイコン表示に切り替えます
                    if (itemData.LoadTileIconIndex == -3)
                    {
                        int x = e.Bounds.Left + (e.Bounds.Width - _iconStore.LargeIconList.ImageSize.Width) / 2;
                        int y = e.Bounds.Top + (e.Bounds.Height - _iconStore.LargeIconList.ImageSize.Height) / 2;
                        _iconStore.LargeIconList.Draw(e.Graphics, x, y, e.Item.ImageIndex);
                    }
                    //取得後の場合はローディングアイコンを描画します。
                    else if (itemData.LoadTileIconIndex >= 0)
                    {
                        if (_tileIconStore.JumboIconList.ImageSize == TileSize)
                        {
                            int x = e.Bounds.Left + (_tileIconStore.JumboIconList.ImageSize.Width - e.Bounds.Width) / 2;
                            int y = e.Bounds.Top + (_tileIconStore.JumboIconList.ImageSize.Height - e.Bounds.Height) / 2;
                            _tileIconStore.JumboIconList.Draw(e.Graphics, x, y, itemData.LoadTileIconIndex);
                        }
                        else
                        {
                            using (var image = _tileIconStore.JumboIconList.Images[itemData.LoadTileIconIndex])
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
                return;
            }

            //タイル以外は通常の描画
            {
                //アイコンが未設定の場合は取得して設定します。
                if (e.Item.ImageIndex == -1)
                {
                    if (e.Item.Tag is ItemData itemData)
                    {
                        _iconStore.GetIconIndexSysSync(itemData.Path, index =>
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
    }

}
