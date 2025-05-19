
namespace PiViLityCore.Shell
{
    public class  FileChangeEventArgs(string fullPath) : EventArgs
    {
        public string FullPath = fullPath;
    }

    /// <summary>
    /// 指定したディレクトリ内のファイルを列挙（移動）するクラス
    /// </summary>
    public class DirectoryFilesDualterator
    {
        private FileSystemWatcher? _fsw;
        private string _path = string.Empty;
        private List<string> _fileList = [];
        private int _currentIndex = -1;
        public string[] FilterExtensions { get; set; } = [];

        public event EventHandler<FileChangeEventArgs>? FileChanged;

        /// <summary>
        /// 対象のディレクトリ
        /// </summary>
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _fsw?.Dispose();
                    _path = "";

                    var dirpath = File.Exists(value) ? System.IO.Path.GetDirectoryName(value) : value;
                    if (Directory.Exists(dirpath) == false)
                    {
                        throw new DirectoryNotFoundException(dirpath);
                    }

                    //現状、対象フォルダ自体の変更（リネーム・削除）に対処できていない
                    _fsw = new FileSystemWatcher(dirpath);
                    var fileList = Directory.EnumerateFiles(dirpath, "*", SearchOption.TopDirectoryOnly)
                        .Where(file => CheckFilter(file))
                        .OrderBy(static f=>f)
                        .ToList();

                    _currentIndex = fileList.FindIndex(f => f.Equals(value, StringComparison.OrdinalIgnoreCase));
                    _currentIndex = _currentIndex < 0 ? 0 : _currentIndex;
                    _currentIndex = fileList.Count == 0 ? -1 : _currentIndex;
                    _fileList = fileList;
                    _path = value;


                    _fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                    _fsw.Changed += FileSystem_OnChanged;
                    _fsw.Renamed += FileSystem_OnRenamed;
                    _fsw.Deleted += FileSystem_OnDeleted;
                    _fsw.Created += FileSystem_OnCreated;
                    _fsw.EnableRaisingEvents = true;

                    FileChanged?.Invoke(this, new(FilePath));
                }
            }
        }

        private bool CheckFilter(string fileName)
        {
            var fi = new FileInfo(fileName);
            if (fi.Attributes.HasFlag(FileAttributes.Directory) == false)
            {
                if (FilterExtensions.Length == 0)
                {
                    return true;
                }
                foreach (var ext in FilterExtensions)
                {
                    if (string.Equals(fi.Extension, ext, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void FileSystem_OnCreated(object sender, FileSystemEventArgs e)
        {
            if(CheckFilter(e.FullPath))
            {
                var target = _fileList.BinarySearch(e.FullPath);
                if (target < 0)
                {
                    target = ~target;
                    _fileList.Insert(target, e.FullPath);
                    if(_currentIndex >= target)
                    {
                        _currentIndex++;
                    }
                }
            }
        }

        private void FileSystem_OnDeleted(object sender, FileSystemEventArgs e)
        {

            var delOnList = _fileList.FindIndex(fn => fn.Equals(_path, StringComparison.OrdinalIgnoreCase));
            if (delOnList >= 0)
            {
                _fileList.RemoveAt(delOnList);
                if (_currentIndex > delOnList)
                {
                    _currentIndex--;
                }
                else if(delOnList >= _fileList.Count)
                {
                    _currentIndex = _fileList.Count - 1;
                }
                FileChanged?.Invoke(this, new(FilePath));
            }
        }

        private void FileSystem_OnRenamed(object sender, RenamedEventArgs e)
        {
            var target = _fileList.FindIndex(fn => fn.Equals(e.OldFullPath, StringComparison.OrdinalIgnoreCase));
            if (target >= 0)
            {
                bool changeCurrentFilename = target== _currentIndex;
                _fileList[target] = e.FullPath;
                if (_currentIndex >= 0)
                {
                    var currentPath = _fileList[_currentIndex];
                    _fileList.Sort();
                    _currentIndex = _fileList.FindIndex(fn => fn.Equals(currentPath));
                }
                if (changeCurrentFilename)
                {
                    FileChanged?.Invoke(this, new(FilePath));
                }
            }
        }

        private void FileSystem_OnChanged(object sender, FileSystemEventArgs e)
        {
            var target = _fileList.FindIndex(fn => fn.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase));
            if(_currentIndex==target)
            {
                FileChanged?.Invoke(this, new(FilePath));
            }
        }

        /// <summary>
        /// 現在のファイル名
        /// setすることでディレクトリが変更したりindexが変わったりする
        /// </summary>
        public string FilePath
        {
            get => _currentIndex < 0 ? string.Empty : _fileList[_currentIndex];
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (File.Exists(value) == false)
                {
                    throw new FileNotFoundException(value);
                }

                var target = _fileList.FindIndex(fn => fn.Equals(value, StringComparison.OrdinalIgnoreCase));
                if (target < 0)
                {
                    Path = value ?? string.Empty;
                }
                else
                {
                    var changeCurrentFilename = target != _currentIndex;
                    _currentIndex = target;
                    if (changeCurrentFilename)
                    {
                        FileChanged?.Invoke(this, new(FilePath));
                    }
                }
            }
        }

        /// <summary>
        /// 次のファイルへ移動します
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if(_fileList.Count-1> _currentIndex)
            {
                _currentIndex++;
                FileChanged?.Invoke(this, new(FilePath));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 前のファイルへ移動します
        /// </summary>
        /// <returns></returns>
        public bool MovePrevious()
        {
            if(_currentIndex > 0)
            {
                _currentIndex--;
                FileChanged?.Invoke(this, new(FilePath));
                return true;
            }
            return false;
        }

    }
}
