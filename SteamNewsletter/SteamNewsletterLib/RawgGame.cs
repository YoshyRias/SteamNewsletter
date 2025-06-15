using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamNewsletterLib
{
    public class RawgGame
    {
        // ChatGPT Prompt: Can you adjust the api output to the properties (Jsonproperty names) - only name, release date, icon and developers

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("released")]
        public string Released { get; set; }

        [JsonPropertyName("background_image")]
        public string BackgroundImage { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        // Developers are not part of the default /games list response.
        [JsonIgnore]
        public string Developer { get; set; }

        [JsonIgnore]
        public string DescriptionRaw { get; set; }

        [JsonIgnore]
        public string Website { get; set; }

        [JsonIgnore]
        public List<string> Genres { get; set; }

        [JsonIgnore]
        public List<string> Platforms { get; set; }

        [JsonIgnore]
        public int Number { get; set; }

    }
}
