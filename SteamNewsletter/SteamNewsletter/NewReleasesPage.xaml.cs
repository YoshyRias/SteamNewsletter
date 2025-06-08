using Serilog;
using Serilog.Core;
using SteamNewsletterLib;
using System;
using System.Collections.Generic;
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
        private string apiKey = "fdf840e885aa4fbb9aecd6b45d152b5a";
        private string listUrl = "https://api.rawg.io/api/games?dates=2025-06-01,2025-06-30&ordering=-added&page_size=20&platforms=4";
        private RawgRoot rawgRoot;

        public NewReleasesPage()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set minimum log level to debug
                .WriteTo.File("Newsletter.log", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            rawgRoot = new RawgRoot(GridMain, ListViewReleases);

            // As you cant execute async code in the constructor - the async part happens when the app is fully loaded (Loaded event)
            // Which here is async for that reason
            Loaded += NewReleasesPage_Loaded; 
        }

        private async void NewReleasesPage_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Logger.Debug("Loaded App succesfully");
            rawgRoot.Results = await GameFetcher(listUrl, apiKey);
            rawgRoot.UpdateListView();
        }

        // ChatGPT Propmt: How can
        public async Task<List<RawgGame>> GameFetcher(string listUrl, string apiKey)
        {
            // Own comments on how the things work

            var httpClient = new HttpClient(); // To generally get the data

            var response = await httpClient.GetStringAsync($"{listUrl}&key={apiKey}"); // Command to get the current trending games
            var root = JsonSerializer.Deserialize<RawgRoot>(response); // Sort the received information in the class-properties

            // Here starts the part to get the developers of the certain games aswell - as they aren't already included 
            foreach (var game in root.Results) // Go through every Game we just got
            {
                // Every Game has its own information site - we go there to get the developers 
                var detailResponse = await httpClient.GetStringAsync($"https://api.rawg.io/api/games/{game.Id}?key={apiKey}"); 
                using var doc = JsonDocument.Parse(detailResponse); // This turns the raw json string into a good usable json structure
                // This structure is simple to an array - in which you can access certain properties instead of indexes

                // This tries to acceses the "developers" part in the json "array" and looks if the array is functionall
                if (doc.RootElement.TryGetProperty("developers", out var developers) && developers.GetArrayLength() > 0)  
                {
                    game.Developer = developers[0].GetProperty("name").GetString(); // Assign the developer-property in each game, when there is a developer found
                    Log.Logger.Debug($"{game.Id} {game.Developer} - worked");
                }
                else
                {
                    game.Developer = "Unknown"; // Handle missing developers gracefully
                    Log.Logger.Debug($"{game.Id} {game.Developer} - didnt work");
                }

            }

            Log.Logger.Debug("Loaded all Trending Games");
            return root.Results;
        }
    }
}
