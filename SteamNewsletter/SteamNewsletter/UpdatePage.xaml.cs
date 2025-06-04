using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamNewsletter
{
    /// <summary>
    /// Interaction logic for UpdatePage.xaml
    /// </summary>
    public partial class UpdatePage : Page
    {
        public UpdatePage()
        {
            InitializeComponent();
        }

        public class NewsFetcher
        {
            private static readonly HttpClient httpClient = new HttpClient();

            /*
            public async Task<Root> FetchNewsAsync(string apiUrl)
            {
                var json = await httpClient.GetStringAsync(apiUrl);
                var result = JsonSerializer.Deserialize<Root>(json);
                return result;
            }*/
        }

        private void ButtonMode_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new NewReleasesPage());

        }
    }
}
