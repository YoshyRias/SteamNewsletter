using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 100; i++)
            {
                CollectionListView.Items.Add(i);
            }
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
    }
}