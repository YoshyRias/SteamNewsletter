
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SteamNewsletterLib
{
    public class Game
    {
        [JsonPropertyName("gid")]
        public string Gid { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("is_external_url")]
        public bool IsExternalUrl { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("contents")]
        public string Contents { get; set; }

        [JsonPropertyName("feedlabel")]
        public string FeedLabel { get; set; }

        [JsonPropertyName("date")]
        public long Date { get; set; }

        [JsonPropertyName("feedname")]
        public string Name { get; set; }

        [JsonPropertyName("feed_type")]
        public int FeedType { get; set; }

        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        public List<string> RecentUpdates = new List<string>();

        public Game(int appID)
        {
            AppId = appID;
        }
        public Game(int appID, string name) : this(appID)
        {
            Name = name;
        }


        public void ShowUpdatLog()
        {

        }

        // ChatGPT: How do I access the Update Logs of a Steam Game
        // ChatGPT: what data type does json have
        // ChatGPT: What is a JToken

        public async Task GetGameUpdates()
        {
            string url = $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={AppId}&count=1";

            using (HttpClient client = new HttpClient())
            {


                var response = await client.GetStringAsync(url);
                JObject json = JObject.Parse(response);

                JToken newsItems = json["appnews"]?["newsitems"];
                if (newsItems != null)
                {
                    foreach (JToken item in newsItems)
                    {
                        string title = item["title"]?.ToString();
                        string contents = item["contents"]?.ToString();

                        //string link = item["url"]?.ToString();

                        RecentUpdates.Add($"Title: {Regex.Replace(title, "<.*?>", string.Empty)}\n{Regex.Replace(contents, "<.*?>", string.Empty)}\n");
                    }
                }
            }
            

        }

        public async Task VisuallizeUpdates(Label UpdateTitle)
        {
            await GetGameUpdates();
            UpdateTitle.Content = RecentUpdates[0];

        }


    }

}