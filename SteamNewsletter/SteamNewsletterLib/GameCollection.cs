using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamNewsletterLib
{
    public class GameCollection
    {
        public List<Game> Games { get; set; } = new List<Game>();

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
