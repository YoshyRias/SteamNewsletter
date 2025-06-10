using Serilog;
using Serilog.Core;
using SteamNewsletterLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamNewsletter
{
    /// <summary>
    /// Interaction logic for Up_nCominPage.xaml
    /// </summary>
    public partial class NewReleasesPage : Page
    {
        private RawgRoot rawgRoot;

        public NewReleasesPage()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set minimum log level to debug
                .WriteTo.File("Newsletter.log", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            rawgRoot = new RawgRoot(GridMain, ListViewReleases);
            MessageBox.Show("Loading ... ");
            // As you cant execute async code in the constructor - the async part happens when the app is fully loaded (Loaded event)
            // Which here is async for that reason
            Loaded += NewReleasesPage_Loaded; 
        }

        private async void NewReleasesPage_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Logger.Debug("Loaded App succesfully");

            rawgRoot.Results = await GameFetcher();
            rawgRoot.UpdateListView();
        }

        // ChatGPT Propmt: How can
        public async Task<List<RawgGame>> GameFetcher()
        {
            DateTime currentDate = DateTime.Now; // Get the current date
            int month = currentDate.Month;
            string curDateStartString = currentDate.ToString($"yyyy-{month:D2}-01"); // Format the date to a string in the format YYYY-MM-DD
            string curDateEndString = currentDate.ToString($"yyyy-{month+1:D2}-01");

            string order = "rating";
            string page_size = "40"; // number of games
            string platforms = "4"; // ids (4 - PC, 187 - PS4, ...)
            string apiKey = "fdf840e885aa4fbb9aecd6b45d152b5a";


            string listUrl = $"https://api.rawg.io/api/games?dates={curDateStartString},{curDateEndString}&ordering={order}&page_size={page_size}&platforms={platforms}";

            HttpClient httpClient = new HttpClient(); 

            string response = await httpClient.GetStringAsync($"{listUrl}&key={apiKey}");
            RawgRoot root = JsonSerializer.Deserialize<RawgRoot>(response); 
            int counter = 0;
            foreach (var game in root.Results) 
            {
                var detailResponse = await httpClient.GetStringAsync($"https://api.rawg.io/api/games/{game.Id}?key={apiKey}"); 
                using var doc = JsonDocument.Parse(detailResponse);
                counter++;
                if (doc.RootElement.TryGetProperty("developers", out var developers) && developers.GetArrayLength() > 0)  
                {
                    game.Developer = developers[0].GetProperty("name").GetString() + counter;
                    Log.Logger.Debug($"{game.Id} {game.Developer} - worked");
                }
                else
                {
                    game.Developer = "Unknown" + counter;
                    Log.Logger.Debug($"{game.Id} {game.Developer} - didnt work");
                }
            }

            Log.Logger.Debug("Loaded all Trending Games");
            return root.Results;
        }

    }
}
