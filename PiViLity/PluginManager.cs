using Microsoft.VisualBasic;
using PiViLity.Setting;
using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PiViLity
{
    public class AppModule : IModuleInformation
    {
        public string Name => "PiViLity App";
        public string Description => "";

        public string OptionItemName => "アプリケーション";

        public System.Resources.ResourceManager? ResourceManager { get => Resource.ResourceManager; }

    }

    /// <summary>
    /// プラグイン情報
    /// </summary>
    public class PluginInformation
    {
        public required Assembly assembly;
        public required IModuleInformation information;
        public List<Type> imageReaders = new();
        public List<SettingBase> settings = new();
        public string Name = "";
    }
    
    /// <summary>
    /// プラグイン管理クラス
    /// </summary>
    internal class PluginManager
    {
        static public PluginManager Instance { get; private set; } = new();

        /// <summary>
        /// プラグインリスト
        /// </summary>
        List<PluginInformation> _plugins = new();

        public List<PluginInformation> Plugins => _plugins;

        /// <summary>
        /// 画像リーダー情報
        /// </summary>
        class ImageReaderInfo
        {
            public required Type readerClass;
            public required PluginInformation plugin;
        }

        /// <summary>
        /// 拡張子から画像リーダー情報を取得するためのディクショナリ
        /// </summary>
        Dictionary<string, List<ImageReaderInfo>> _imageReadersFromExtension = new();

        /// <summary>
        /// 拡張子から画像リーダー情報を取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IImageReader? GetImageReader(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            if (_imageReadersFromExtension.TryGetValue(ext, out List<ImageReaderInfo>? imageReaderInformationList))
            {
                return Activator.CreateInstance(imageReaderInformationList[0].readerClass) as IImageReader;
            }
            return null;
        }

        /// <summary>
        /// プラグインをロード
        /// </summary>
        /// <param name="dirPath"></param>
        public void LoadPlugins(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                var directory = new DirectoryInfo(dirPath);
                var dllFiles = directory.EnumerateFiles("*.dll");
                foreach (var dllFile in dllFiles)
                {
                    LoadFile(dllFile.FullName);
                }
            }
        }

        private void LoadFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var asm = Assembly.LoadFile(path);
                if (asm != null)
                {
                    AnalyzeAssembly(asm);
                }
            }
        }

        internal void AnalyzeAssembly(Assembly assembly)
        {
            PluginInformation? information = null;
            int moduleInfoCount = 0;
            foreach (var type in assembly.GetTypes())
            {
                foreach (var hasInterface in type.GetInterfaces())
                {
                    if (hasInterface == typeof(IModuleInformation))
                    {
                        moduleInfoCount++;
                        if (Activator.CreateInstance(type) is IModuleInformation moduleInfo)
                        {
                            information = new()
                            {
                                assembly = assembly,
                                information = moduleInfo
                            };

                        }
                    }
                }
            }

            if (moduleInfoCount == 1 && information != null)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var hasInterface in type.GetInterfaces())
                    {
                        /// 画像リーダーの場合
                        if (hasInterface == typeof(IImageReader) && Activator.CreateInstance(type) is IImageReader reader)
                        {
                            information.imageReaders.Add(type);

                            ImageReaderInfo readerInfo = new()
                            {
                                readerClass = type,
                                plugin = information
                            };
                            foreach (var ext in reader.GetSupportedExtensions())
                            {
                                var lowerExt = "." + ext.ToLower();
                                if (_imageReadersFromExtension.TryGetValue(lowerExt, out List<ImageReaderInfo>? imageReaderInformationList))
                                {
                                    imageReaderInformationList.Add(readerInfo);
                                }
                                else
                                {
                                    _imageReadersFromExtension.Add(lowerExt, new List<ImageReaderInfo>() { readerInfo });
                                }
                            }
                        }
                    }

                    //SettingBase継承のクラスはInstanceメソッドがあればそれで得られるインスタンスを取得する
                    if (PiViLityCore.Util.Types.HasParentType(type, typeof(SettingBase)))
                    {
                        if( type.GetField("Instance", BindingFlags.Public | BindingFlags.Static) is FieldInfo getInstanceField)
                        {
                            if(getInstanceField.GetValue(null) is SettingBase setting)
                                information.settings.Add(setting);
                        }
                    }
                }
                _plugins.Add(information);
            }
        }

        /// <summary>
        /// 設定を保存
        /// </summary>
        public void SaveSettings(string filePath)
        {
            var settings = _plugins.SelectMany(p => p.settings).ToList();
            var types = settings.Select(s => s.GetType()).Distinct().ToArray();
            var serializer = new XmlSerializer(typeof(List<SettingBase>), types);
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, settings);
            }
        }

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        public void LoadSettings(string filePath)
        {
            if (File.Exists(filePath))
            {
                var settings = _plugins.SelectMany(p => p.settings).ToList();
                var types = settings.Select(s => s.GetType()).Distinct().ToArray();
                var serializer = new XmlSerializer(typeof(List<SettingBase>), types);

                using (var reader = new StreamReader(filePath))
                {
                    if (serializer.Deserialize(reader) is List<SettingBase> loadedSettings)
                    {
                        foreach (var setting in loadedSettings)
                        {
                            var plugin = _plugins.FirstOrDefault(p => p.settings.Any(s => s.Name == setting.Name));
                            if (plugin != null)
                            {
                                var existingSetting = plugin.settings.FirstOrDefault(s => s.Name == setting.Name);
                                if (existingSetting != null)
                                {
                                    var fields = existingSetting.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField);
                                    var properties = existingSetting.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                                    foreach (FieldInfo field in fields)
                                    {
                                        if (field.GetValue(setting) is object value)
                                        {
                                            field.SetValue(existingSetting, value);
                                        }
                                    }
                                    foreach (PropertyInfo prop in properties)
                                    {
                                        if (prop.CanWrite)
                                        {
                                            if (prop.GetValue(setting) is object value)
                                            {
                                                prop.SetValue(existingSetting, value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

