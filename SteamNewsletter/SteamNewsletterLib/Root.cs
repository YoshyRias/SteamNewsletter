using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamNewsletterLib
{
    public class Root
    {
        [JsonPropertyName("appnews")]
        public GameCollection GameCollection { get; set; }

    }
}
