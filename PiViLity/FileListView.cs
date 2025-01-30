using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        IconStore _tileIconStore = new(false, false, true);

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
                    _tileIconStore = new IconStore(false, false, true, null,null,TileSize);
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
                            _iconStore.GetIconIndexNoThumbnail(file.FullName, index =>
                            {
                                item.ImageIndex = index;
                            });

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
                    OwnerDraw = false;
                }
                base.View = value;
            }
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (View == View.Tile)
            {
                bool drawedThumbnail = false;
                if (e.Item.Tag is ItemData itemData)
                {
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
                        });
                    }
                    else if(itemData.LoadTileIconIndex >= 0)
                    {
                        drawedThumbnail = true;
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

                if (!drawedThumbnail)
                {
                    if (e.Item.ImageIndex >= 0 && e.Item.ImageList != null)
                    {
                        e.Item.ImageList.Draw(e.Graphics, e.Bounds.Left, e.Bounds.Top, e.Item.ImageIndex);
                    }
                }
            }
            else
            {
                base.OnDrawItem(e);
            }
        }
    }

}
