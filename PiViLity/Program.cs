using PiViLity.COM;
using PiViLityCore.Plugin;
using PiViLityPlugin.Difinition;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace PiViLity
{
    public class AppModule : IModuleInformation
    {
        public string Name => "PiViLity App";
        public string Description => "";

        public string OptionItemName => "アプリケーション";

        public bool Initialize() => true;

        public bool Terminate() => true;
    }



    internal static class App
    {
        public static ResourceManager AppResource { get; } = new ResourceManager("PiViLity.Resource", Assembly.GetExecutingAssembly());


        internal static bool SystemIsLightThemeSetting()
        {
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
            int.TryParse(key?.GetValue("AppsUseLightTheme")?.ToString(), out var isLight);
            return isLight != 0;
        }

        static string? AppDir = null;

        static void Initialize()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            System.Diagnostics.Debug.Assert(Application.RenderWithVisualStyles);
            System.Diagnostics.Debug.WriteLine($"IsDark={PiViLityCore.Windows.SystemColor.IsDarkMode()} BackGround={PiViLityCore.Windows.SystemColor.BackGroundColor().ToString()}");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            //CultureInfo.CurrentCulture = new CultureInfo("en-US");
            //CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            PluginManager.Create();

            AppDir = Path.GetDirectoryName(Application.ExecutablePath);
            if (AppDir != null)
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                PluginManager.Instance.AnalyzeAssembly(executingAssembly);
                PluginManager.Instance.AnalyzeAssembly(typeof(PiViLityCore.Global).Assembly);
                PluginManager.Instance.LoadPlugins(AppDir + "\\Plugins");
                PluginManager.Instance.LoadSettings(AppDir + "\\settings.json");
            }

            var isSystemColor = Option.AppSettings.Instance.Theme == Option.ColorTheme.SystemDefault;
            if (isSystemColor)
            {
                if (SystemIsLightThemeSetting())
                    Application.SetColorMode(SystemColorMode.Classic);
                else
                    Application.SetColorMode(SystemColorMode.Dark);
            }
            else
            {
                if (Option.AppSettings.Instance.Theme == Option.ColorTheme.Light)
                    Application.SetColorMode(SystemColorMode.Classic);
                else
                    Application.SetColorMode(SystemColorMode.Dark);
            }

            //言語設定にょってリソースカルチャー指定
            switch (Option.AppSettings.Instance.AppLanguage)
            {
                case Option.Language.SystemDefault:
                    {
                        CultureInfo.CurrentCulture = CultureInfo.InstalledUICulture;
                        CultureInfo.CurrentUICulture = CultureInfo.InstalledUICulture;
                    }
                    break;
                case Option.Language.Japanese:
                    {
                        CultureInfo.CurrentCulture = new CultureInfo("ja-JP");
                        CultureInfo.CurrentUICulture = new CultureInfo("ja-JP");
                    }
                    break;
                case Option.Language.English:
                    {
                        CultureInfo.CurrentCulture = new CultureInfo("en-US");
                        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
                    }
                    break;
            }
        }

        /// <summary lang="ja">
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] _args)
        {
            Initialize();

            List<string> openFiles = new(); ;
            for (int i = 0; i < _args.Length; i++)
            {
                if (_args[i].Equals("/f", StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    if (i < _args.Length)
                    {
                        var path = _args[i];
                        path = path.Trim([' ', '\t', '\"']);
                        if (File.Exists(path))
                        {
                            openFiles.Add(path);
                        }
                        else
                        {
                            i--;
                        }
                    }
                }
            }



            //サムネイルエンジン初期化
            ThumbnailCache.Create();
            ThumbnailCache.Instance.Initialize(Option.AppSettings.Instance.CacheDb);

            //スレッドプール調整
            ThreadPool.SetMinThreads(32,32);
            ThreadPool.SetMaxThreads(64, 64);

            //SusiePlugin関連の初期化
            SusiePluginManager.Create();
            SusiePluginManager.Instance.ReloadPlugins();


            //susieのことを思い、サポートチェックは後で行う
            PluginManager.Instance.AnalyzeReader();

            try
            {
                if (openFiles.Count == 0)
                {
                    Application.Run(new Forms.MainForm());
                }
                else
                {
                    Application.Run(new MultiVieweriFormApplicationContext(openFiles));
                }

            }catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.ToString(), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //SusiePlugin関連終了
            SusiePluginManager.Release();
            ThumbnailCache.Release();

            if (AppDir != null)
            {
                PluginManager.Instance.SaveSettings(AppDir + "\\settings.json");
            }

            PluginManager.Release();

        }
    }
}