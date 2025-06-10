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
        public bool isRunning = false;

        public NewReleasesPage()
        {
            InitializeComponent();
            isRunning = true;

            rawgRoot = new RawgRoot(GridMain, ListViewReleases);
            // As you cant execute async code in the constructor - the async part happens when the app is fully loaded (Loaded event)
            // Which here is async for that reason
            Loaded += NewReleasesPage_Loaded; 
        }

        private async void NewReleasesPage_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Logger.Debug("Loaded App succesfully");

            rawgRoot.Results = await GameFetcher();
            rawgRoot.UpdateListView();
            isRunning = false;

            LabelLoading.Visibility = Visibility.Collapsed;
            ListViewReleases.Visibility = Visibility.Visible;
        }

        // ChatGPT Propmt: How can i Fetch the data from an API
        public async Task<List<RawgGame>> GameFetcher()
        {
            DateTime currentDate = DateTime.Now; // Get the current date
            // ChatGPT Prompt: Can you format my time to the start and end of the current month
            DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1);
            string curDateStartString = startDate.ToString("yyyy-MM-dd");
            string curDateEndString = endDate.ToString("yyyy-MM-dd");


            string order = "-released";
            string page_size = "40"; // number of games
            string platforms = "4"; // ids (4 - PC, 187 - PS4, ...)
            string apiKey = "fdf840e885aa4fbb9aecd6b45d152b5a";


            string listUrl = $"https://api.rawg.io/api/games?dates={curDateStartString},{curDateEndString}&ordering={order}&page_size={page_size}&platforms={platforms}";

            HttpClient httpClient = new HttpClient();

            try
            {
                string response = await httpClient.GetStringAsync($"{listUrl}&key={apiKey}");
                RawgRoot root = JsonSerializer.Deserialize<RawgRoot>(response);

                var tasks = root.Results.Select(async game =>
                {
                    try
                    {
                        string detailResponse = await httpClient.GetStringAsync($"https://api.rawg.io/api/games/{game.Id}?key={apiKey}");
                        using var doc = JsonDocument.Parse(detailResponse);
                        if (doc.RootElement.TryGetProperty("developers", out var developers) && developers.GetArrayLength() > 0)
                        {
                            game.Developer = developers[0].GetProperty("name").GetString();
                        }
                        else
                        {
                            game.Developer = "Unknown";
                            Log.Logger.Debug($"{game.Id} {game.Developer} - not found");
                        }
                    }
                    catch (Exception ex) 
                    {
                        game.Developer = "Unknown";
                        Log.Logger.Error(ex, " - Loading Games Developers failed.");
                    }
                });
                await Task.WhenAll(tasks);

                Log.Logger.Debug("Loaded all Trending Games");
                return root.Results;
            }

            catch (Exception ex)
            {
                Log.Logger.Error(ex, " - Failed to Fetch Games from RAWG");
                return new List<RawgGame>();
            }
        }

    }
}
