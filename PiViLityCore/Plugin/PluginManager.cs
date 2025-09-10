using Microsoft.VisualBasic;
using PiViLityCore;
using PiViLityCore.Option;
using PiViLityCore.Plugin;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PiViLityCore.Plugin
{


    /// <summary>
    /// プラグイン情報
    /// </summary>
    public class PluginInformation
    {
        public required Assembly assembly;
        public required IModuleInformation information;
        public List<Type> imageReaders = new();
        public List<Type> propertyReaders = new();
        public List<SettingBase> settings = new();
        public string Name = "";
    }
    
    /// <summary>
    /// プラグイン管理クラス
    /// </summary>
    public class PluginManager
    {
        static public PluginManager Instance { get; private set; } = new();

        PluginManager()
        {
        }

        /// <summary>
        /// プラグインリスト
        /// </summary>
        List<PluginInformation> _plugins = new();

        public List<PluginInformation> Plugins => _plugins;

        public List<Type> ImageViewers { get; private set; } = new();
        public List<Type> PreViewers { get; private set; } = new();

        [Flags]
        enum ReaderType
        {
            None = 0,
            Image = 1<<0,
            Property = 1<<1,

            All = Image | Property

        }
        /// <summary>
        /// リーダー情報
        /// </summary>
        class ReaderInfo
        {
            public required Type? ReaderClass;
            public required ReaderType SupportRead;
            public required PluginInformation Plugin;
        }

        /// <summary>
        /// 拡張子から画像リーダー情報を取得するためのディクショナリ
        /// </summary>
        Dictionary<string, List<ReaderInfo>> _imageReadersFromExtension = new();

        List<string> supportImageExtensions = new();
        public IReadOnlyList<string> SupportImageExtensions { get => supportImageExtensions; }

        /// <summary>
        /// 拡張子から画像リーダー情報を取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IImageReader? GetImageReader(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            if (_imageReadersFromExtension.TryGetValue(ext, out List<ReaderInfo>? ReaderInformationList))
            {
                foreach (var info in ReaderInformationList)
                {
                    if ((info.SupportRead & ReaderType.Image) != 0)
                    {
                        var reader = info.ReaderClass;
                        if (reader != null)
                        {
                            return Activator.CreateInstance(reader) as IImageReader;
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 拡張子から画像リーダー情報を取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IPropertyReader? GetPropertyReader(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            if (_imageReadersFromExtension.TryGetValue(ext, out List<ReaderInfo>? ReaderInformationList))
            {
                foreach( var info in ReaderInformationList)
                {
                    if( (info.SupportRead & ReaderType.Property) != 0)
                    {
                        var reader = info.ReaderClass;
                        if (reader != null)
                        {
                            return Activator.CreateInstance(reader) as IPropertyReader;
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// プラグインをロード
        /// </summary>
        /// <param name="dirPath"></param>
        public void LoadPlugins(string dirPath)
        {
            AnalyzeAssembly(typeof(PiViLityCore.Plugin.IModuleInformation).Assembly);

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

        public void AnalyzeAssembly(Assembly assembly)
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
                    if (type.IsAbstract)
                        continue;

                    var interfaces = type.GetInterfaces();
                    if (interfaces.Where(t => t == typeof(IImageReader)).Count() > 0 ||
                        interfaces.Where(t => t == typeof(IPropertyReader)).Count() > 0)
                    {

                        ///リーダーサポート状況
                        var tmpInstance = Activator.CreateInstance(type);
                        var reader = tmpInstance as IReader;
                        ReaderType readerType = ReaderType.None;
                        readerType |= tmpInstance is IImageReader ? ReaderType.Image : ReaderType.None;
                        readerType |= tmpInstance is IPropertyReader ? ReaderType.Property : ReaderType.None;
                        if (reader != null)
                        {
                            information.imageReaders.Add(type);

                            ReaderInfo readerInfo = new()
                            {
                                ReaderClass = type,
                                SupportRead = readerType,
                                Plugin = information
                            };
                            foreach (var ext in reader.GetSupportedExtensions())
                            {
                                var lowerExt = "." + ext.ToLower();
                                if (_imageReadersFromExtension.TryGetValue(lowerExt, out List<ReaderInfo>? ReaderInformationList))
                                {
                                    ReaderInformationList.Add(readerInfo);
                                }
                                else
                                {
                                    _imageReadersFromExtension.Add(lowerExt, new List<ReaderInfo>() { readerInfo });
                                    supportImageExtensions.Add(lowerExt);
                                }
                            }
                        }
                    }

                    //画像ビューアー
                    if (interfaces.Where(t => t == typeof(IImageViewer)).Count() > 0 && Activator.CreateInstance(type) is IImageViewer imageViewer)
                    {
                        ImageViewers.Add(type);
                    }
                    //プレビュー
                    if (interfaces.Where(t => t == typeof(IPreViewer)).Count() > 0 && Activator.CreateInstance(type) is IPreViewer previewer)
                    {
                        PreViewers.Add(type);
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
            using (var writer = new StreamWriter(filePath))
            {
                SaveSettings(writer);
            }
        }
        
        /// <summary>
        /// 指定ストリームに設定を保存
        /// </summary>
        /// <param name="writer"></param>
        public void SaveSettings(StreamWriter writer)
        {
            var settings = _plugins.SelectMany(p => p.settings);
            using (Utf8JsonWriter jsonWriter = new(writer.BaseStream, new JsonWriterOptions()
            {
                Indented = true,
                SkipValidation = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }))
            {
                jsonWriter.WriteStartObject();
                var categories = settings.Select(s => s.CategoryName).Distinct();
                foreach (var category in categories)
                {
                    jsonWriter.WritePropertyName(category);
                    foreach (var setting in settings)
                    {
                        if (setting.CategoryName == category)
                        {
                            PiViLityCore.Option.SerializeHelper.writeSettingValue(jsonWriter, null, setting);
                        }

                    }
                }
                jsonWriter.WriteEndObject();
            }
        }
        /// <summary>
        /// 設定を読み込む
        /// </summary>
        public void LoadSettings(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    LoadSettings(reader);
                }
            }
        }
        /// <summary>
        /// 指定ストリームから設定を読み込み
        /// </summary>
        /// <param name="writer"></param>
        public void LoadSettings(StreamReader reader)
        {
            if(reader== null)
                return;
            string jsonStr = reader.ReadToEnd();
            byte[] buffer = Encoding.UTF8.GetBytes(jsonStr);
            var settings = _plugins.SelectMany(p => p.settings);
            IEnumerable<SettingBase> targetSettings = settings;

            Utf8JsonReader jsonReader = new(buffer, new JsonReaderOptions() { });
            {
                jsonReader.Read();
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonTokenType.StartObject)
                    {
                        PiViLityCore.Option.SerializeHelper.readSettingValue(ref jsonReader, targetSettings);
                    }
                    else if (jsonReader.TokenType == JsonTokenType.EndObject)
                    {
                    }
                    else if (jsonReader.TokenType == JsonTokenType.PropertyName)
                    {
                        var catName = jsonReader.GetString();
                        targetSettings = settings.Where(s => s.CategoryName == catName);
                    }
                    else
                    {
                        
                    }
                }
            }
        }

    }
}

