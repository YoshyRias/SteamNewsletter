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

        private(bool SteamApiKeyValid, bool SteamIdValid) UserInfoValid { get;  set; }


        public UserDataInput()
        {
            InitializeComponent();

        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserInfoValid.SteamApiKeyValid == true && UserInfoValid.SteamIdValid == true)
            {
                DialogResult = true;
            }
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
        public async Task IsSteam_Apikey_ID_Valid()
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

                UserInfoValid = (true, steamIdValid);

            }
            catch (HttpRequestException e)
            {
               UserInfoValid = (false, false);
            }


        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApiKeyTextBox.Text = SteamApiKey;
            SteamIDTextBox.Text = SteamId;
        }

        private async void ApiKeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SteamApiKey = ApiKeyTextBox.Text;
            await IsSteam_Apikey_ID_Valid();
            if (UserInfoValid.SteamApiKeyValid== true)
            {
                ApiKeyTextBox.Background = Brushes.DarkSeaGreen;
            }
            else
            {
                ApiKeyTextBox.Background = Brushes.IndianRed;
            }
            if (UserInfoValid.SteamIdValid == true)
            {

                SteamIDTextBox.Background = Brushes.DarkSeaGreen;
            }
            else
            {
                SteamIDTextBox.Background = Brushes.IndianRed;
            }

        }

        private async void SteamIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SteamId = SteamIDTextBox.Text;
            await IsSteam_Apikey_ID_Valid();
            if (UserInfoValid.SteamIdValid == true)
            {

                SteamIDTextBox.Background = Brushes.DarkSeaGreen;
            }
            else
            {
                SteamIDTextBox.Background = Brushes.IndianRed;
            }
            if (UserInfoValid.SteamApiKeyValid == true)
            {
                ApiKeyTextBox.Background = Brushes.DarkSeaGreen;
            }
            else
            {
                ApiKeyTextBox.Background = Brushes.IndianRed;
            }
        }

       
    }
}
