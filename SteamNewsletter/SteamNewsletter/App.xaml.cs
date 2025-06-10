using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SteamNewsletter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set minimum log level to debug
                .WriteTo.File("Newsletter.log", rollingInterval: RollingInterval.Month)
                .CreateLogger();
        }
    }

}
