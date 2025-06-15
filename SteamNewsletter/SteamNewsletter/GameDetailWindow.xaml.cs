using SteamNewsletterLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SteamNewsletter
{
    /// <summary>
    /// Interaction logic for GameDetailWindow.xaml
    /// </summary>
    public partial class GameDetailWindow : Window
    {
        private RawgGame game;
        private string apiKey;
        public GameDetailWindow()
        {
            InitializeComponent();
        }

        public GameDetailWindow(RawgGame game, string apiKey)
        {
            InitializeComponent();
            DataContext = game;
            this.game = game;
            this.apiKey = apiKey;

            this.Loaded += GameDetailWindow_Loaded;
        }

        private async void GameDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await RawgFetchGames.FetchGameDetails(game, apiKey);

            GameTitle.Text = game.Name;
            GameDescription.Text = game.DescriptionRaw;
            // ChatGPT Prompt: How can i show a list nicely in a textblock
            GameGenres.Text = string.Join(", ", game.Genres);
            GamePlatforms.Text = string.Join(", ", game.Platforms);
        }

        private void OpenWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(game.Website)) {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = game.Website,
                    UseShellExecute = true
                });
            }
            else
            {
                OpenWebsiteButton.IsEnabled = false;
                MessageBox.Show("The chosen game doesn't provide a official website.");
            }
        }
    }
}
