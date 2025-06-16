using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for RawgAPIInputWindow.xaml
    /// </summary>
    public partial class RawgAPIInputWindow : Window
    {
        public string Key { get; private set; }
        public RawgAPIInputWindow()
        {
            InitializeComponent();
            TryLoadFile();
        }

        private void ApiKeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApiKeyTextBox.Background = null;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Key = ApiKeyTextBox.Text.Trim();

            if (string.IsNullOrEmpty(Key))
            {
                ApiKeyTextBox.Background = Brushes.LightCoral;
                return;
            }

            bool isValid = await ValidateAPIKey();
            if (!isValid) 
            {
                // ChatGPT Prompt: Everything fine here? (he gave an improvement option)
                ApiKeyTextBox.Background = Brushes.LightCoral;
                MessageBox.Show("The API key appears invalid or the RAWG server is unreachable.\n\nPlease make sure it's correct and your internet connection is active.", "Invalid API Key", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            SaveFile();
            this.DialogResult = true;
        }

        private void APIKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://rawg.io/apidocs",
                UseShellExecute = true
            });
        }

        // ChatGPT Prompt: How can i easily check if the api key is valid.
        private async Task<bool> ValidateAPIKey()
        {
            try
            {
                using HttpClient client = new HttpClient();
                string testUrl = $"https://api.rawg.io/api/platforms?key={Key}";

                HttpResponseMessage test = await client.GetAsync(testUrl);
                return (test.IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex + " - testing the api key didnt work.");
                return false;
            }
        }
        private void TryLoadFile()
        {
            try
            {
                // ChatGPT Prompt: Is this save? res: Yeah, but few improvements...
                using (StreamReader sr = new StreamReader("rawg-api-key.txt"))
                {
                    string line = sr.ReadLine().Trim();

                    if (!string.IsNullOrEmpty(line))
                    {
                        Key = line;
                        ApiKeyTextBox.Text = Key;
                    }
                }
            }

            catch { return; }
        }

        private void SaveFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("rawg-api-key.txt", false))
                {
                    sw.WriteLine(Key);
                }
            }

            catch (Exception ex)
            {
                Log.Logger.Error(ex + " - Failed to save API key.");
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // ChatGPT Prompt: Display this note nicer.
            MessageBox.Show(
        "If creating a RAWG account seems to fail (e.g., page reloads without any error), press F12 in your browser, go to the Console tab, and look for errors. Often, it's a missing required field or a blocked script.",
        "RAWG Signup Help");
        }
    }
}
