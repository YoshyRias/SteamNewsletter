using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SteamNewsletterLib
{
    public class RawgRoot
    {
        private ListView listView;
        private Grid grid;

        [JsonPropertyName("results")]
        public List<RawgGame> Results { get; set; }

        public RawgRoot() {}

        public RawgRoot(Grid grid, ListView listivew)
        {
            this.grid = grid;
            this.listView = listivew;
        }

        public void UpdateListView()
        {
            listView.Items.Clear();
            listView.ItemsSource = Results;
        }

    }
}
