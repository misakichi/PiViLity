using PiViLityCore.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Controls
{
    public class FileListViewApp : PiViLityCore.Controls.FileListView
    {
        //============================================================
        //ディレクトリ履歴管理
        private List<string> _directoryRecent = new();
        private int _currentRecentIndex = -1;
        private bool _isInPathSetter = false;
        public event EventHandler DirectoryNavigatorButtonStatusChanged = delegate { };
        //============================================================


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileListViewApp() : base()
        {
            DirectoryChanged += FlvOnDirectoryChanged;
            LabelEdit = true;
            ThumbnailIconStore = new IconStoreThumbnail(PiViLityCore.Option.ShellSettings.Instance.ThumbnailSize);

            if (!DesignMode)
            {
                TileSize = PiViLityCore.Option.ShellSettings.Instance.ThumbnailSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether navigating to the previous directory is enabled.
        /// </summary>
        public bool IsPreviousDirectoryEnabled => _currentRecentIndex > 0;

        /// <summary>
        /// Gets a value indicating whether the next directory in the recent directory list is available.
        /// </summary>
        public bool IsNextDirectoryEnabled => _currentRecentIndex < _directoryRecent.Count - 1;

        /// <summary>
        /// Gets a value indicating whether the parent directory of the specified path exists.
        /// </summary>
        public bool IsParentDirectoryEnabled => (new DirectoryInfo(Path)).Parent != null;

        /// <summary>
        /// Disables the functionality of the directory navigator button by resetting its status change event handler.
        /// </summary>
        /// <remarks>This method clears any existing event handlers for the <see
        /// cref="DirectoryNavigatorButtonStatusChanged"/> event  by assigning an empty delegate. After calling this
        /// method, the event will no longer trigger any previously  attached handlers.</remarks>
        public void ToDisable()
        {
            DirectoryNavigatorButtonStatusChanged = delegate { };
        }

        /// <summary>
        /// Assigns the specified event handler to handle changes in the status of the directory navigator button.
        /// </summary>
        /// <param name="directoryNavigatorButtonStatusChangedFunction">The event handler to be invoked when the directory navigator button's status changes.  This parameter cannot
        /// be <see langword="null"/>.</param>
        public void ToEnable(EventHandler directoryNavigatorButtonStatusChangedFunction)
        {
            DirectoryNavigatorButtonStatusChanged = directoryNavigatorButtonStatusChangedFunction;
            DirectoryNavigatorButtonStatusChanged?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// パス変更推移の追加
        /// </summary>
        private void AddRecent()
        {
            //ボタンの有効無効状態を得ておく
            bool preIsPreviousEnabled = IsPreviousDirectoryEnabled;
            bool preIsNextEnabled = IsNextDirectoryEnabled;
            bool preIsParentEnabled = IsParentDirectoryEnabled;

            _directoryRecent.RemoveRange(_currentRecentIndex + 1, _directoryRecent.Count - _currentRecentIndex - 1);
            _directoryRecent.Add(Path);
            _currentRecentIndex = _directoryRecent.Count - 1;

            //ボタンの有効無効を更新
            if (preIsPreviousEnabled != IsPreviousDirectoryEnabled || preIsNextEnabled != IsNextDirectoryEnabled || preIsParentEnabled != IsParentDirectoryEnabled)
            {
                DirectoryNavigatorButtonStatusChanged(this, EventArgs.Empty);
            }
        }



        /// <summary>
        /// ディレクトリ推移前に戻る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MovePreviousDirectory()
        {
            if (_currentRecentIndex > 0)
            {
                //ボタンの有効無効状態を得ておく
                bool preIsPreviousEnabled = IsPreviousDirectoryEnabled;
                bool preIsNextEnabled = IsNextDirectoryEnabled;
                bool preIsParentEnabled = IsParentDirectoryEnabled;

                _currentRecentIndex--;
                _Path = _directoryRecent[_currentRecentIndex];

                //ボタンの有効無効を更新
                if (preIsPreviousEnabled != IsPreviousDirectoryEnabled || preIsNextEnabled != IsNextDirectoryEnabled || preIsParentEnabled != IsParentDirectoryEnabled)
                {
                    DirectoryNavigatorButtonStatusChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ディレクトリ推移次へ行く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveNextDirectory()
        {
            if (_currentRecentIndex < _directoryRecent.Count - 1)
            {
                //ボタンの有効無効状態を得ておく
                bool preIsPreviousEnabled = IsPreviousDirectoryEnabled;
                bool preIsNextEnabled = IsNextDirectoryEnabled;
                bool preIsParentEnabled = IsParentDirectoryEnabled;

                _currentRecentIndex++;
                _Path = _directoryRecent[_currentRecentIndex];

                //ボタンの有効無効を更新
                if (preIsPreviousEnabled != IsPreviousDirectoryEnabled || preIsNextEnabled != IsNextDirectoryEnabled || preIsParentEnabled != IsParentDirectoryEnabled)
                {
                    DirectoryNavigatorButtonStatusChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 親ディレクトリへ移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveParentDirectory()
        {
            var path = Path;
            if (path != null)
            {
                var dir = new DirectoryInfo(path);
                if (dir.Parent is DirectoryInfo parentDir)
                {
                    //ボタンの有効無効状態を得ておく
                    bool preIsPreviousEnabled = IsPreviousDirectoryEnabled;
                    bool preIsNextEnabled = IsNextDirectoryEnabled;
                    bool preIsParentEnabled = IsParentDirectoryEnabled;

                    Path = parentDir.FullName;

                    //ボタンの有効無効を更新
                    if (preIsPreviousEnabled != IsPreviousDirectoryEnabled || preIsNextEnabled != IsNextDirectoryEnabled || preIsParentEnabled != IsParentDirectoryEnabled)
                    {
                        DirectoryNavigatorButtonStatusChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void FlvOnDirectoryChanged(object? sender, EventArgs e)
        {
            //DirectoryChanged?.Invoke(this, EventArgs.Empty);
            if (!_isInPathSetter)
            {
                AddRecent();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Path
        {
            set { base.Path = value; }
            get => base.Path;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private string _Path
        {
            set { _isInPathSetter = true; Path = value; _isInPathSetter = false; }
            get => Path;
        }

    }
}
