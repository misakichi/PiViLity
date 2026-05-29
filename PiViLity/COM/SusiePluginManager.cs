using PiViLity.Option;

namespace PiViLity.COM
{
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

        public SusiePluginManager()
        {
            SusiePluginSettings.Instance.Changed += OptionChanged;
        }
        public override void Dispose()
        {
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
                if (_extensionPluginsMap.TryGetValue(ext.ToLower(), out var plugins))
                {
                    foreach (var pluginInfo in plugins)
                    {
                        var plugin = new PiVilityNative.SusiePluginCom();
                        if (plugin.Load(pluginInfo.Path))
                        {
                            return new SusiePluginInstance(plugin, pluginInfo.Path);
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
                                pluginInfo.extensions.Add(extStr.ToLower());
                                infoIdx += 2;
                            }

                            _plugins.Add(pluginInfo);
                            foreach (var ext in pluginInfo.extensions)
                            {
                                var extOnly = ext.Replace("*","").Replace(".", "").Trim();
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
                    catch (Exception)
                    {
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
