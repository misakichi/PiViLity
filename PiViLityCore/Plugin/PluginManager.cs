using Microsoft.VisualBasic;
using PiViLityCore;
using PiViLityCore.Option;
using PiViLityPlugin.Difinition;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PiViLityCore.Plugin
{

    public sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly string _pluginDirectory;
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath, bool isCollectible)
            : base(isCollectible)
        {
            _pluginDirectory = Path.GetDirectoryName(pluginPath)!;
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var defaultAsm = Default.Assemblies;
            foreach (var asm in defaultAsm)
            {
                if (AssemblyName.ReferenceMatchesDefinition(asm.GetName(), assemblyName))
                    return asm;
            }

            var path = _resolver.ResolveAssemblyToPath(assemblyName);
            if (path != null)
                return LoadFromAssemblyPath(path);
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (path != null)
                return LoadUnmanagedDllFromPath(path);
            return IntPtr.Zero;
        }
    }
    public class SettingInstanceInfo
    {
        public required MethodInfo Create;
        public required MethodInfo Release;
        public required Type Type;
    }

    /// <summary>
    /// プラグイン情報
    /// </summary>
    public class PluginInformation
    {
        public required Assembly assembly;
        public required IModuleInformation information;
        public List<Type> imageReaders = new();
        public List<Type> propertyReaders = new();
        public List<ISetting> settings = new();
        public List<SettingInstanceInfo> settingInstanceInfo = new();
        public string Name = "";
    }
    
    /// <summary>
    /// プラグイン管理クラス
    /// </summary>
    public class PluginManager : PiViLityPlugin.Singleton<PluginManager>
    {
        public PluginManager()
        {
        }
        public override void Dispose()
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
            AnalyzeAssembly(typeof(IModuleInformation).Assembly);

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
                var alc = new PluginLoadContext(path, false);
                var asm = alc.LoadFromAssemblyPath(path);
                //var asm = Assembly.LoadFile(path);
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
            foreach (var type in assembly.GetExportedTypes())
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
                    if (interfaces.Any(t => t == typeof(IImageReader)) ||
                        interfaces.Any(t => t == typeof(IPropertyReader)))
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
                    if (interfaces.Any(t => t == typeof(IImageViewer)) && Activator.CreateInstance(type) is IImageViewer imageViewer)
                    {
                        ImageViewers.Add(type);
                    }
                    //プレビュー
                    if (interfaces.Any(t => t == typeof(IPreViewer)) && Activator.CreateInstance(type) is IPreViewer previewer)
                    {
                        PreViewers.Add(type);
                    }

                    //SettingBase継承のクラスはInstanceメソッドがあればそれで得られるインスタンスを取得する
                    if (interfaces.Any(t=>t==typeof(ISetting)))
                    {
                        ISetting? settingInstance = null;
                        MethodInfo? createMethod = null;
                        MethodInfo? releaseMethod = null;
                        PropertyInfo? instanceProp = null;
                        var searchType = type;
                        while (searchType != null && searchType!=typeof(object))
                        {
                            if(createMethod==null)
                                createMethod = searchType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
                            if (releaseMethod == null)
                                releaseMethod = searchType.GetMethod("Release", BindingFlags.Public | BindingFlags.Static);
                            if (instanceProp == null)
                                instanceProp = searchType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                            searchType = searchType.BaseType;
                        }
                        if (createMethod != null && releaseMethod != null  && instanceProp != null) 
                        {

                            createMethod.Invoke(null, null);
                            //if (type.GetField("Instance", BindingFlags.Public | BindingFlags.Static) is FieldInfo getInstanceField)
                            //{
                            //    if (getInstanceField.GetValue(null) is ISetting setting)
                            //        settingInstance = setting;
                            //}
                            settingInstance = instanceProp.GetValue(null) as ISetting;

                            if (settingInstance != null)
                                information.settings.Add(settingInstance);
                                information.settingInstanceInfo.Add(new()
                                {
                                    Create=createMethod,
                                    Release = releaseMethod,
                                    Type = type
                                });
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
            IEnumerable<ISetting> targetSettings = settings;

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

