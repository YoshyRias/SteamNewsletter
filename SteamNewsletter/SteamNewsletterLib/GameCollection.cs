using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace SteamNewsletterLib
{
    public class GameCollection
    {
        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        // ChatGPT(Prompt): Wie kann ich herausfinden was für Spiele sich in der Bibliotek des Benutzers.
        // ChatGPT(Prompt): Was ist ein SteamApiKey
        // ChaGPT(Prompt): Was macht das: (string[] args)

        static async Task LoadLibaryGames()
        {
            string steamApiKey = "";
            string steamId = "76561199385548855";

            string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={steamApiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(json);
                var games = data["response"]?["games"];

                List<string> GameDataList = new List<string>();

                if (games != null)
                {
                    ListView listView = new ListView
                    {
                        Width = 300,
                        Height = 200,
                        Margin = new Thickness(10)
                    };
                    
                    
                    foreach (JObject game in games)
                    {
                        string GameData = ($"{game["name"]},{game["appid"]},{game["playtime_forever"]}");
                        Console.WriteLine(GameData);
                        GameDataList.Add(GameData);
                        listView.Items.Add(game["name"]);
                    }
                }
                else
                {
                    Console.WriteLine("No games found or the profile is private.");
                }
            }
        }

        private void CreateSteamNewsletterLib_APIkey(string path, int APIkey)
        {

            // How to make a massage box in c# with wpf
            using (StreamWriter streamwriter = new StreamWriter(path))
            {

            }
        }
        public void AddGame(Game game)
        {
            Games.Add(game);
        }
        [JsonPropertyName("newsitems")]
        public List<Game> Games { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
        
    }
}
