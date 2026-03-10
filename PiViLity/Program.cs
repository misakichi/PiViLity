using PiViLityCore.Plugin;
using PiViLityPlugin.Difinition;
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

        /// <summary lang="ja">
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(SystemIsLightThemeSetting())
                Application.SetColorMode(SystemColorMode.Classic);
            else
                Application.SetColorMode(SystemColorMode.Dark);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            System.Diagnostics.Debug.Assert(Application.RenderWithVisualStyles);
            System.Diagnostics.Debug.WriteLine($"IsDark={PiVilityNative.SystemColor.IsDarkMode()} BackGround={PiVilityNative.SystemColor.BackGroundColor().ToString()}");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            //CultureInfo.CurrentCulture = new CultureInfo("en-US");
            //CultureInfo.CurrentUICulture = new CultureInfo("en-US");


            var appDir = Path.GetDirectoryName(Application.ExecutablePath);
            if (appDir != null)
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                PluginManager.Instance.AnalyzeAssembly(executingAssembly);
                PluginManager.Instance.AnalyzeAssembly(typeof(PiViLityCore.Global).Assembly);
                PluginManager.Instance.LoadPlugins(appDir + "\\Plugins");
                PluginManager.Instance.LoadSettings(appDir + "\\settings.json");
            }

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
     

            ThumbnailCache.Initialize(Option.AppSettings.Instance.CacheDb);

            ThreadPool.SetMinThreads(32,32);
            ThreadPool.SetMaxThreads(64, 64);

            try
            {
                Application.Run(new Forms.MainForm());
            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ThumbnailCache.Terminate();

            if (appDir != null)
            {
                PluginManager.Instance.SaveSettings(appDir + "\\settings.json");
            }
        }
    }
}