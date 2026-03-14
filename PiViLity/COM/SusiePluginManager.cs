using PiViLity.COM;
using PiViLity.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace PiViLity.COM
{
    internal class SusiePluginManager : PiViLityPlugin.Singleton<SusiePluginManager>
    {
        List<COM.SusiePluginCom> _plugins = new();

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

        public void ReloadPlugins()
        {

        }

        public void UnloadPlugins()
        {
            foreach (var plugin in _plugins)
                plugin.Dispose();
            _plugins.Clear();
        }

    }
}
