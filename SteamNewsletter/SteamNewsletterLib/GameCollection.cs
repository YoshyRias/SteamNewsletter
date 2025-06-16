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
using System.Windows.Documents;

namespace SteamNewsletterLib
{
    public class GameCollection
    {

        public static string Filepath = "SteamUserInfo.txt";

        public List<List<Game>> GameCollectionsList = new List<List<Game>>();

        public List<Game> LibaryGames = new List<Game>();
        public List<Game> Games { get; set; }

        public string SteamApiKey { get; set; }
        public string SteamId { get; set; }

        public int Count { get; set; }

        public int AppId { get; set; }

        // ChatGPT(Prompt): Wie kann ich herausfinden was für Spiele sich in der Bibliotek des Benutzers.
        // ChatGPT(Prompt): Was ist ein SteamApiKey
        // ChaGPT(Prompt): Was macht das: (string[] args)

        public GameCollection()
        {

            GameCollectionsList.Add(LibaryGames);
            LoadLibaryGames();

        }

        public async Task LoadLibaryGames()
        {

            string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={SteamApiKey}&steamid={SteamId}&include_appinfo=true&include_played_free_games=true";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(json);
                var games = data["response"]?["games"];

                List<string> GameDataList = new List<string>();

                if (games != null)
                {



                    foreach (JObject game in games)
                    {
                        string GameData = ($"{game["name"]},{game["appid"]},{game["playtime_forever"]}");
                        GameDataList.Add(GameData);
                        LibaryGames.Add(new Game(int.Parse(game["appid"].ToString()), game["name"].ToString()));
                    }

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
            //Games.Add(game);
        }
        public void DrawListview(int index, ListView listveiw)
        {
            List<Game> GameCollection = GameCollectionsList[index];
            foreach (Game game in GameCollection)
            {
                listveiw.Items.Add(game.Name);
            }
        }
        public void Deserialize(string Data)
        {
            string[] SplitedData = Data.Split(",");
            try {
                SteamApiKey = SplitedData[0];
                SteamId = SplitedData[1];
            }
            catch
            {
                
            }
        }

        public string Serialize()
        {
            string Data = $"{SteamApiKey},{SteamId}" ;
            return Data;
        }

        public void LoadSteamDataFromFile()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(Filepath))
                {
                    string[] Data = new string[2];
                    string line = streamReader.ReadLine();


                    this.Deserialize(line);
                };
            }
            catch
            {

            }
        }
        public void WriteIntoFile()
        {
            string SerializedData = this.Serialize();
            
           
            using(StreamWriter  streamWriter = new StreamWriter(Filepath))
            {
                
                    streamWriter.WriteLine(SerializedData);
                
            }
        }


        


    }
}
