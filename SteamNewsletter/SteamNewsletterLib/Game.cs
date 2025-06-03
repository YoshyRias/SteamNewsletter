
using System.Text.Json.Serialization;

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
        public string FeedName { get; set; }

        [JsonPropertyName("feed_type")]
        public int FeedType { get; set; }

        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }
}
