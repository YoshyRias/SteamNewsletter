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

            int curPage = 0
                ;

            if (curPage == 0)
            {
                UpdatePage updatePage = new UpdatePage();
                MainFrame.Navigate(updatePage);
            }

            else if (curPage == 1)
            {
                NewReleasesPage newReleasesPage = new NewReleasesPage();
                MainFrame.Navigate(newReleasesPage);
            }
        }
        
    }
}