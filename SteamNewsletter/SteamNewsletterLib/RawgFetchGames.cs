using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteamNewsletterLib
{
    public class RawgFetchGames // ChatGPT Propmt: How can i Fetch the data from an API
    {
        public RawgFetchGames() { }

        public static async Task<List<RawgGame>> GameFetcher()
        {
            RawgFilters filters = new RawgFilters();

            string listUrl = 
                $"https://api.rawg.io/api/games?" +
                $"dates={filters.curDateStartString},{filters.curDateEndString}" +
                $"&ordering={filters.order}" +
                $"&page_size={filters.page_size}" +
                $"&platforms={filters.platforms}" +
                $"&key={filters.apiKey}";

            HttpClient httpClient = new HttpClient();

            try
            {
                string response = await httpClient.GetStringAsync($"{listUrl}");
                RawgRoot root = JsonSerializer.Deserialize<RawgRoot>(response);


                if (root.Results == null)
                {
                    Log.Logger.Warning("No results from RAWG API.");
                    return new List<RawgGame>();
                }

                var tasks = root.Results.Select(async game =>
                {
                    try
                    {
                        string detailResponse = await httpClient.GetStringAsync($"https://api.rawg.io/api/games/{game.Id}?key={filters.apiKey}");
                        using var doc = JsonDocument.Parse(detailResponse);
                        if (doc.RootElement.TryGetProperty("developers", out var developers) && developers.GetArrayLength() > 0)
                        {
                            game.Developer = developers[0].GetProperty("name").GetString();
                            Log.Logger.Debug($"{game.Id} {game.Developer} - dev found");
                        }
                        else
                        {
                            game.Developer = "Unknown";
                            Log.Logger.Debug($"{game.Id} {game.Developer} - dev not found");
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
