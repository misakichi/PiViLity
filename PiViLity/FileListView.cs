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

        IconStore _iconStore = new(true, true, true);

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
            LargeImageList = _iconStore.largeIconList;
            SmallImageList = _iconStore.smallIconList;
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
                    var files = dirInfo.EnumerateFiles();
                    foreach (var file in files)
                    {
                        if (PiViLityCore.Global.settings.IsVisibleFile(file))
                        {
                            var item = new ListViewItem();
                            item.Text = file.Name;
                            item.Tag = file;
                            item.ImageIndex = _iconStore.GetIconIndex(file.FullName);

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
                    LargeImageList = _iconStore.jumboIconList;
                }
                else
                {
                    LargeImageList = _iconStore.largeIconList;
                }
                base.View = value;
            }
        }
}

}
