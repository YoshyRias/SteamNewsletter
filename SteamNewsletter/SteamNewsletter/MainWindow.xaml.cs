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
        private int curPage = 1;

        private UpdatePage updatePage;
        private NewReleasesPage newReleasesPage;

        public MainWindow()
        {
            InitializeComponent();

            if (curPage == 0)
            {
                updatePage = new UpdatePage();
                MainFrame.Navigate(updatePage);
            }

            else if (curPage == 1)
            {
                newReleasesPage = new NewReleasesPage();
                MainFrame.Navigate(newReleasesPage);
            }
        }

        private void ButtonMode_Click(object sender, RoutedEventArgs e)
        {
            if (curPage == 1 && !newReleasesPage.isRunning)
            {
                updatePage = new UpdatePage();
                MainFrame.Navigate(updatePage);
                curPage = 0;
            }

            else if (curPage == 0)
            {
                newReleasesPage = new NewReleasesPage();
                MainFrame.Navigate(newReleasesPage);
                curPage++;
            }
        }
    }
}