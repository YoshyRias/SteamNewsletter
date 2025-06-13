using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for UserDataInput.xaml
    /// </summary>
    public partial class UserDataInput : Window
    {
        public string SteamApiKey {  get; set; }

        public string SteamId { get; set; }
        public UserDataInput()
        {
            InitializeComponent();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var (ApiKeyValid, SteamIdValid) = await IsSteam_Apikey_ID_Valid();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // ChatGPT: Wie Öffne ich eine Webseite mit c#
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://steamcommunity.com/dev/apikey",
                UseShellExecute = true
            });
        }

        // ChatGPT: Gibt es eine Steam Test ID
        // ChatGPT: Wie returnt man daten mit einem async task
        // ChatGPT: Is there a simple way to check if a Steam id is valid
        public async Task<(bool ApiKeyValid, bool SteamIdValid)> IsSteam_Apikey_ID_Valid()
        {
            string steamId = "76561197960435530"; // Steam Test ID

            string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={SteamApiKey}&steamids={steamId}";

            using HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetAsync(url);
                string result = await client.GetStringAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                bool steamIdValid = !content.Contains("\"players\":[]");

                return (true, steamIdValid);

            }
            catch (HttpRequestException e)
            {
                return (false, false);
            }


        }
    }
}
