using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity
{
    internal class PluginManager
    {
        static public PluginManager Instance { get; private set; } = new();

        class PluginInformation
        {
            public required Assembly assembly;
            public required IModuleInformation information;
            public List<Type> imageReaders = new();
        }

        List<PluginInformation> plugins_ = new();

        class ImageReaderInfo
        {
            public required Type readerClass;
            public required PluginInformation plugin;
        }
        Dictionary<string, List<ImageReaderInfo>> imageReaders_ = new();

        public IImageReader? GetImageReader(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            if (imageReaders_.TryGetValue(ext, out List<ImageReaderInfo>? imageReaderInformationList))
            {
                return Activator.CreateInstance(imageReaderInformationList[0].readerClass) as IImageReader;
            }
            return null;
       }

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
            if(System.IO.File.Exists(path))
            {
                var asm = Assembly.LoadFile(path);
                if (asm != null)
                {
                    AnalyzeAssembly(asm);
                }
            }
        }

        private void AnalyzeAssembly(Assembly assembly)
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
                                if (imageReaders_.TryGetValue(lowerExt, out List<ImageReaderInfo>? imageReaderInformationList))
                                {
                                    imageReaderInformationList.Add(readerInfo);
                                }
                                else
                                {
                                    imageReaders_.Add(lowerExt, new List<ImageReaderInfo>() { readerInfo });
                                }
                            }
                        }
                    }
                }
                plugins_.Add(information);
            }     
        }
    }
}
