using PiViLityCore.Plugin;
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

    internal static class Program
    {
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


            var appDir = Path.GetDirectoryName(Application.ExecutablePath);
            if (appDir != null)
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                PluginManager.Instance.AnalyzeAssembly(executingAssembly);
                PluginManager.Instance.LoadPlugins(appDir+"\\Plugins");
                PluginManager.Instance.LoadSettings(appDir+"\\settings.json");
            }

            ThumbnailCache.Initialize(Option.AppSettings.Instance.CacheDb);

            ThreadPool.SetMinThreads(32,32);
            ThreadPool.SetMaxThreads(64, 64);

            Application.Run(new Forms.MainForm());

            ThumbnailCache.Terminate();

            if (appDir != null)
            {
                PluginManager.Instance.SaveSettings(appDir + "\\settings.json");
            }
        }
    }
}