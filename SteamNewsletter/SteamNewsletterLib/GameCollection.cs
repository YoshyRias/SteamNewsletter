using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SteamNewsletterLib
{
    public class GameCollection
    {
        public List<Game> Games { get; set; } = new List<Game>();

        // ChatGPT(Prompt): Wie kann ich herausfinden was für Spiele sich in der Bibliotek des Benutzers.
        // ChatGPT(Prompt): Was ist ein SteamApiKey
        // ChaGPT(Prompt): Was macht das: (string[] args)

        static async Task LoadLibaryGames()
        {
            string steamApiKey = "EC5664F11CB8B658CD538FF3D4D88187";
            string steamId = "76561199385548855";

            string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={steamApiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(json);
                var games = data["response"]?["games"];

                string[] GameDataArray = new string[games.Count()];

                if (games != null)
                {
                    foreach (JObject game in games)
                    {
                        string GameData = ($"{game["name"]},{game["appid"]},{game["playtime_forever"]}");
                        Console.WriteLine(GameData);
                    }
                }
                else
                {
                    Console.WriteLine("No games found or the profile is private.");
                }
            }
        }


        public void AddGame(Game game)
        {
            Games.Add(game);
        }

        public void RemoveGame(Game game)
        {
            Games.Remove(game);
        }

        public void ClearGames()
        {
            Games.Clear();
        }
    }
}
