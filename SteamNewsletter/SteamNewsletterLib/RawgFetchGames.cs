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

        public static async Task<List<RawgGame>> GameFetcher(RawgFilters filters)
        {
            HttpClient httpClient = new HttpClient();
            int totalPages = int.Parse(filters.pages);
            int counter = 0;
            List<RawgGame> allGames = new List<RawgGame>();

            try
            {
                for (int page = 1; page <= totalPages; page++)
                {
                    string listUrl =
                        $"https://api.rawg.io/api/games?" +
                        $"dates={filters.curDateStartString},{filters.curDateEndString}" +
                        $"&ordering={filters.order}" +
                        $"&page_size={filters.page_size}" +
                        $"&page={page}" +
                        $"&platforms={filters.platforms}" +
                        $"&key={filters.apiKey}";

                    string response = await httpClient.GetStringAsync(listUrl);
                    RawgRoot root = JsonSerializer.Deserialize<RawgRoot>(response);

                    if (root.Results == null || root.Results.Count == 0)
                    {
                        Log.Logger.Warning($"No results from RAWG API on page {page}.");
                        break;
                    }

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
                }

                Log.Logger.Debug("Loaded all requested pages of games.");
                return allGames;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, " - Failed to fetch games from RAWG.");
                return new List<RawgGame>();
            }
        }

    }
}
