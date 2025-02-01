using System.Reflection;

namespace PiViLity
{
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
            System.Diagnostics.Debug.WriteLine($"IsDark={ShelAPIHelper.SystemColor.IsDarkMode()} BackGround={ShelAPIHelper.SystemColor.BackGroundColor().ToString()}");

            var appDir = Path.GetDirectoryName(Application.ExecutablePath);
            if (appDir != null)
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                PluginManager.Instance.AnalyzeAssembly(executingAssembly);
                PluginManager.Instance.LoadPlugins(appDir+"\\Plugins");
                PluginManager.Instance.LoadSettings(appDir+"\\settings.xml");
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());

            if (appDir != null)
            {
                PluginManager.Instance.SaveSettings(appDir + "\\settings.xml");
            }
        }
    }
}