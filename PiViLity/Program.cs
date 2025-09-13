using PiViLityCore.Plugin;
using System.Globalization;
using System.Reflection;

namespace PiViLity
{
    public class AppModule : IModuleInformation
    {
        public string Name => "PiViLity App";
        public string Description => "";

        public string OptionItemName => "アプリケーション";

        public System.Resources.ResourceManager? ResourceManager { get => Resource.ResourceManager; }

    }

    internal static class App
    {
        public static PiViLityCore.Resource.Manager AppResource { get; } = new ("PiViLity.Resource", Assembly.GetExecutingAssembly());

        /// <summary lang="ja">
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#pragma warning disable WFO5001 
            Application.SetColorMode(SystemColorMode.System);
            //Application.SetColorMode(SystemColorMode.Classic);
#pragma warning restore WFO5001
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