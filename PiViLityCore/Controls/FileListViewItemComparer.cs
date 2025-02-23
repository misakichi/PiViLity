using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Controls
{

    /// <summary>
    /// ファイルリスト用ListView(FileListView)のアイテムコンパレータ
    /// </summary>
    public abstract class FileListViewItemComparerBase : System.Collections.IComparer
    {
        protected FileListView _listView;
        public FileListViewItemComparerBase(FileListView listView)
        {
            _listView = listView;
        }

        public virtual int Compare(object? x, object? y)
        {
            int ret = 0;
            if (x is FileListViewItem item1 && y is FileListViewItem item2)
            {
                if (item1.IsSpecialFolder && !item2.IsSpecialFolder)
                    ret = -1;
                else if (!item1.IsSpecialFolder && item2.IsSpecialFolder)
                    ret = 1;
                else if (item1.IsFile && !item2.IsFile)
                    ret = 1;
                else if (!item1.IsFile && item2.IsFile)
                    ret = -1;
            }
            return ret;
        }
    }

    /// <summary>
    /// ファイルリスト用ListView(FileListView)のアイテムコンパレータ(ファイル名)
    /// </summary>
    public class FileListViewItemComparerName : FileListViewItemComparerBase
    {

        public FileListViewItemComparerName(FileListView listView) : base(listView) { }

        public override int Compare(object? x, object? y)
        {
            int ret = base.Compare(x, y);
            if (ret == 0 && x is FileListViewItem item1 && y is FileListViewItem item2)
            {
                ret = string.Compare(item1.Text, item2.Text);
            }
            return ret * (_listView.Sorting == SortOrder.Ascending ? 1 : -1);
        }
    }

    /// <summary>
    /// ファイルリスト用ListView(FileListView)のアイテムコンパレータ(サイズ)
    /// </summary>
    public class FileListViewItemComparerSize : FileListViewItemComparerBase
    {
        public FileListViewItemComparerSize(FileListView listView) : base(listView) { }

        public override int Compare(object? x, object? y)
        {
            int ret = base.Compare(x, y);
            if (ret == 0 && x is FileListViewItem item1 && y is FileListViewItem item2)
            {
                ret = item1.Length == item2.Length ? 0 :
                        item1.Length < item2.Length ? -1 : 1;
            }
            return ret * (_listView.Sorting == SortOrder.Ascending ? 1 : -1);
        }
    }

    /// <summary>
    /// ファイルリスト用ListView(FileListView)のアイテムコンパレータ(更新日時)
    /// </summary>
    public class FileListViewItemComparerModifiedDateTime : FileListViewItemComparerBase
    {
        public FileListViewItemComparerModifiedDateTime(FileListView listView) : base(listView) { }

        public override int Compare(object? x, object? y)
        {
            int ret = base.Compare(x, y);
            if (ret == 0 && x is FileListViewItem item1 && y is FileListViewItem item2)
            {
                ret = item1.ModifiedDateTime.Ticks < item2.ModifiedDateTime.Ticks ? -1 : 1;
            }
            return ret * (_listView.Sorting == SortOrder.Ascending ? 1 : -1);
        }
    }

    /// <summary>
    /// ファイルリスト用ListView(FileListView)のアイテムコンパレータ(タイプ)
    /// </summary>
    public class FileListViewItemComparerType : FileListViewItemComparerBase
    {
        public FileListViewItemComparerType(FileListView listView) : base(listView) { }

        public override int Compare(object? x, object? y)
        {
            int ret = base.Compare(x, y);
            if (ret == 0 && x is FileListViewItem item1 && y is FileListViewItem item2)
            {
                ret = string.Compare(item1.FileType, item2.FileType);
            }
            return ret * (_listView.Sorting == SortOrder.Ascending ? 1 : -1);
        }
    }
}
