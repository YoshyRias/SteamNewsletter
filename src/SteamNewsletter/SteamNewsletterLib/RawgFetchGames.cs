using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteamNewsletterLib
{
    public class RawgFetchGames // ChatGPT Propmt: How can i Fetch the data from an API
    {
        public RawgFetchGames() { }

        public static async Task<List<RawgGame>> GameFetcher(RawgFilters filters)
        {
            HttpClient httpClient = new HttpClient();
            int counter = 0;
            List<RawgGame> allGames = new List<RawgGame>();

            try
            {

                string listUrl =
                    $"https://api.rawg.io/api/games?" +
                    $"dates={filters.curDateStartString},{filters.curDateEndString}" +
                    $"&ordering={filters.order}" +
                    $"&page_size={filters.page_size}" +
                    $"&platforms={filters.platforms}" +
                    $"&key={filters.apiKey}";

                string response = await httpClient.GetStringAsync(listUrl);
                RawgRoot root = JsonSerializer.Deserialize<RawgRoot>(response);

                var tasks = root.Results.Select(async game =>
                {
                    counter++;
                    game.Number = counter;
                    try
                    {
                        string detailResponse = await httpClient.GetStringAsync($"https://api.rawg.io/api/games/{game.Id}?key={filters.apiKey}");
                        using JsonDocument doc = JsonDocument.Parse(detailResponse);
                        if (doc.RootElement.TryGetProperty("developers", out var developers) && developers.GetArrayLength() > 0)
                        {
                            game.Developer = developers[0].GetProperty("name").GetString();
                            Log.Logger.Debug($"{game.Id} {game.Developer} - dev found");
                        }
                        else
                        {
                            game.Developer = "Unknown";
                            Log.Logger.Debug($"{game.Id} - dev not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        game.Developer = "Unknown";
                        Log.Logger.Error(ex, " - Loading Game Developer failed.");
                    }
                });

                await Task.WhenAll(tasks);
                allGames.AddRange(root.Results);
                

                Log.Logger.Debug("Loaded all requested pages of games.");
                return allGames;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, " - Failed to fetch games from RAWG.");
                return new List<RawgGame>();
            }
        }

        public static async Task FetchGameDetails(RawgGame game, string apiKey)
        {
            HttpClient client = new HttpClient();
            string url = $"https://api.rawg.io/api/games/{game.Id}?key={apiKey}";

            try
            {
                string detailsJson = await client.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(detailsJson);
                var root = doc.RootElement;

                game.DescriptionRaw = root.GetProperty("description_raw").GetString();
                game.Website = root.GetProperty("website").GetString();

                
                if (root.TryGetProperty("genres", out JsonElement genres))
                {
                    game.Genres = new List<string>();
                    foreach (JsonElement genresObj in genres.EnumerateArray())
                    {
                        game.Genres.Add(genresObj.GetProperty("name").GetString());  
                    }
                }

                if (root.TryGetProperty("platforms", out var platforms))
                {
                    game.Platforms = new List<string>();
                    foreach (JsonElement platformsObj in platforms.EnumerateArray())
                    {
                        JsonElement platform = platformsObj.GetProperty("platform");
                        game.Platforms.Add(platform.GetProperty("name").GetString());
                    }
                }
            }

            catch (Exception e)
            {
                Log.Logger.Error(e + " - Fetching Game details failed");
            }
        }
    }
}
