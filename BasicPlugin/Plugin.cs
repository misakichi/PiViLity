using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin
{
    public class Plugin : PiViLityCore.Plugin.IModuleInformation
    {
        public string Name => "PiViLity Basic Plugin.";

        public string Description => "基本的な画像フォーマットを簡易的にサポートします。";
    }
}
