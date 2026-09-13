using PiViLity.Option;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace PiViLity.COM
{
    internal class SusiePluginThreadJob : IDisposable
    {
        public SusiePluginThreadJob(Action action, bool fromIsTask)
        {
            taskCompletionSource_ = new();
            action_ = action;
        }

        public void Run()
        {
            action_();
            taskCompletionSource_?.SetResult();
            eventWaitHandle.Set();
        }
        public void Wait()
        {
            eventWaitHandle.WaitOne();
        }
        public Task? Task() => taskCompletionSource_?.Task;

        public void Dispose()
        {
            eventWaitHandle.Dispose();
        }

        System.Threading.Tasks.TaskCompletionSource? taskCompletionSource_ = null;
        Action action_;
        EventWaitHandle eventWaitHandle = new(false,EventResetMode.ManualReset);
    }
    internal class SusiePluginManager : PiViLityPlugin.Singleton<SusiePluginManager>
    {
        class SusiePluginInfo
        {
            public required PiVilityNative.SusiePluginCom plugin;
            public List<string> extensions = new();
            public string Version = "";
            public string Description = "";
            public string Path = "";

        }


        List<SusiePluginInfo> _plugins = new();
        Dictionary<string, List<SusiePluginInfo>> _extensionPluginsMap = new();

        public IEnumerable<string> Extensions => _extensionPluginsMap.Keys;

        public EventHandler PluginLoaded = delegate { };


        [DllImport("ole32.dll")]
        private static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

        [DllImport("ole32.dll")]
        private static extern void CoUninitialize();

        private const uint COINIT_APARTMENTTHREADED = 0x2;   // STA
        private const uint COINIT_MULTITHREADED = 0x0;   // MTA
        public void ComThread()
        {
            CoInitializeEx(0, COINIT_MULTITHREADED);


            while (!exitThread_)
            {
                if (!jobSemaphire_.Wait(TimeSpan.FromSeconds(10)))
                    continue;

                if (jobs_.TryDequeue(out var job))
                {
                    job.Run();
                }
            }

            CoUninitialize();
            jobShutdownSemaphire_.Release();
        }

        public void AddJob(SusiePluginThreadJob job)
        {
            jobs_.Enqueue(job);
            jobSemaphire_.Release();
        }
        public SusiePluginThreadJob AddJobSync(Action action)
        {
            var job = new SusiePluginThreadJob(action, false);
            AddJob(job);
            return job;
        }

        ConcurrentQueue<SusiePluginThreadJob> jobs_ = new();
        SemaphoreSlim jobSemaphire_ = new(0);
        SemaphoreSlim jobShutdownSemaphire_ = new(0);
        bool exitThread_ = false;
        List<Thread> comThreads_ = new();

        public SusiePluginManager()
        {
            SusiePluginSettings.Instance.Changed += OptionChanged;

            int logicalCores = Environment.ProcessorCount;
            for (int i = 0; i < logicalCores; i++)
            {
                var thread = new Thread(ComThread);
                thread.Start();
                thread.Name = $"SusiePluginThread{i}";
                comThreads_.Add(thread);
            }

        }
        public override void Dispose()
        {
            exitThread_ = true;
            jobSemaphire_.Release(comThreads_.Count);
            jobShutdownSemaphire_.Wait(comThreads_.Count);
            UnloadPlugins();
        }

        private void OptionChanged(object? sender, EventArgs e)
        {
            ReloadPlugins();
        }

        public SusiePluginInstance? GetPluginInstanceForFile(string filepath)
        {
            if (File.Exists(filepath) == false)
                return null;

            if (Path.GetExtension(filepath) is string ext)
            {
                if (_extensionPluginsMap.TryGetValue(ext.ToLower().Replace(".",""), out var plugins))
                {
                    foreach (var pluginInfo in plugins)
                    {
                        var plugin = new PiVilityNative.SusiePluginCom();
                        if (plugin.Load(pluginInfo.Path))
                        {
                            return new SusiePluginInstance(plugin, filepath);
                        }
                        plugin.Dispose();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// プラグインのリロード
        /// </summary>
        public void ReloadPlugins()
        {
            UnloadPlugins();

            //プラグインディレクトリが存在した場合にifプラグインを検索して読み込む
            if (Directory.Exists(SusiePluginSettings.Instance.PluginPath))
            {
                var files = Directory.EnumerateFiles(SusiePluginSettings.Instance.PluginPath, "if*.spi");
                foreach (var file in files)
                {
                    try
                    {
                        //COMオブジェクトが生成でき、ロードできた場合
                        var plugin = new PiVilityNative.SusiePluginCom();
                        if (plugin.Load(file))
                        {
                            var pluginInfo = new SusiePluginInfo() { plugin = plugin, Path = file };
                            if (plugin.GetPluginInfo(0, out var verStr))
                                pluginInfo.Version = verStr;
                            if (plugin.GetPluginInfo(1, out var descStr))
                                pluginInfo.Description = descStr;
                            int infoIdx = 2;
                            while (plugin.GetPluginInfo(infoIdx, out var extStr))
                            {
                                pluginInfo.extensions.AddRange(extStr.ToLower().Split(';'));
                                infoIdx += 2;
                            }

                            _plugins.Add(pluginInfo);
                            foreach (var ext in pluginInfo.extensions)
                            {
                                var extOnly = ext.Replace("*","").Replace(".","").Trim();
                                if (_extensionPluginsMap.TryGetValue(extOnly, out var extInfos))
                                {
                                    extInfos.Add(pluginInfo);
                                }
                                else
                                {
                                    _extensionPluginsMap.TryAdd(extOnly, new List<SusiePluginInfo>([pluginInfo]));
                                }
                            }

                        }
                        else
                        {
                            plugin?.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        PiViLityCore.Global.WarningLog(e.ToString());
                    }
                }
            }
        }

        public void UnloadPlugins()
        {
            //com破棄のためにdisposeはいる
            foreach (var plugin in _plugins)
                plugin.plugin.Dispose();

            _plugins.Clear();
            _extensionPluginsMap.Clear();
        }

    }
}
