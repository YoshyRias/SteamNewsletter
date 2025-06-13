using Serilog;
using Serilog.Core;
using SteamNewsletterLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for Up_nCominPage.xaml
    /// </summary>
    public partial class NewReleasesPage : Page
    {
        private RawgRoot rawgRoot;
        public bool isRunning = false;

        public NewReleasesPage()
        {
            InitializeComponent();
            isRunning = true;

            rawgRoot = new RawgRoot(GridMain, ListViewReleases);
            // As you cant execute async code in the constructor - the async part happens when the app is fully loaded (Loaded event)
            // Which here is async for that reason
            Loaded += NewReleasesPage_Loaded; 
        }

        private async void NewReleasesPage_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Logger.Debug("Loaded App succesfully");
            LoadGames();

            LabelLoading.Visibility = Visibility.Collapsed;
            ListViewReleases.Visibility = Visibility.Visible;
            StackPanelFilters.Visibility = Visibility.Visible;
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadGames();
        }

        private async void LoadGames()
        {
            isRunning = true;
            rawgRoot.Results = await RawgFetchGames.GameFetcher();
            rawgRoot.UpdateListView();
            isRunning = false;
        }


    }
}
