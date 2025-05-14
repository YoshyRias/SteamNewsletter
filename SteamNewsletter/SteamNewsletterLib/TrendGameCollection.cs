using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SteamNewsletterLib
{
    public class TrendGameCollection
    {
        private List<Game> games = new();
        private ListView listView;
        private Grid grid;

        public TrendGameCollection( Grid grid)
        {
            this.grid = grid;
        }

        public void UpdateListView()
        {
            //TODO: Update the ListView with the games from the collection
        }

        public void GetGames()
        {
            //TODO: Get games from SteamDB 
        }

    }
}
