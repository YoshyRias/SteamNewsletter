using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamNewsletterLib
{
    public class GameCollection
    {
        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("newsitems")]
        public List<Game> Games { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
        
    }
}
