using Serilog;
using Serilog.Core;
using SteamNewsletterLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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

        private RawgFilters rawgFilters = new();

        public NewReleasesPage()
        {
            InitializeComponent();
            isRunning = true;

            // ChatGPT Prompt: short way to make it the first day of cur month
            DateTime now = DateTime.Now;
            DatePickerStart.SelectedDate = new DateTime(now.Year, now.Month, 1);
            DatePickerEnd.SelectedDate = new DateTime(now.Year, now.Month, 1).AddMonths(1);

            rawgRoot = new RawgRoot(GridMain, ListViewReleases);
            // As you cant execute async code in the constructor - the async part happens when the app is fully loaded (Loaded event)
            // Which here is async for that reason
            Loaded += NewReleasesPage_Loaded; 
        }

        private async void NewReleasesPage_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Logger.Debug("Loaded App succesfully");
            LoadGames();
        }


        private async void LoadGames()
        {
            isRunning = true;
            ShowLoading(isRunning);

            Log.Logger.Debug(rawgFilters.ToString());
            rawgRoot.Results = await RawgFetchGames.GameFetcher(rawgFilters);
            rawgRoot.UpdateListView();

            isRunning = false;
            ShowLoading(isRunning);
        }
        // ChatGPT Prompt: Toggle Method for visibility
        private void ShowLoading(bool isLoading)
        {
            LabelLoading.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            ListViewReleases.Visibility = isLoading ? Visibility.Collapsed : Visibility.Visible;
            StackPanelFilters.Visibility = isLoading ? Visibility.Collapsed : Visibility.Visible;
        }



        private void ComboBoxPlatforms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxPlatforms.SelectedIndex == 0) rawgFilters.platforms = "1,4,7,18,186,187"; // IDs of all platforms

            else
            {
                // ChatGPT Prompt: How can I get the Tag of the selected item
                ComboBoxItem selectedItem = ComboBoxPlatforms.SelectedItem as ComboBoxItem;
                rawgFilters.platforms = selectedItem.Tag.ToString();
            }
        }

        private void DatePickerStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // ChatGPT Prompt: How to format the time from the Datepicker 
            if (DatePickerStart.SelectedDate != null)
            {
                DateTime start = DatePickerStart.SelectedDate.Value;

                rawgFilters.curDateStartString = start.ToString("yyyy-MM-dd");

                if (DatePickerEnd.SelectedDate == null || DatePickerEnd.SelectedDate < start)
                {
                    DatePickerEnd.SelectedDate = start;
                    rawgFilters.curDateEndString = start.ToString("yyyy-MM-dd");
                }
            }
        
        }

        private void DatePickerEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePickerEnd.SelectedDate != null)
            {
                DateTime end = DatePickerEnd.SelectedDate.Value;

                rawgFilters.curDateEndString = end.ToString("yyyy-MM-dd");

                if (DatePickerStart.SelectedDate == null || DatePickerStart.SelectedDate > end)
                {
                    DatePickerStart.SelectedDate = end;
                    rawgFilters.curDateStartString = end.ToString("yyyy-MM-dd");
                }
            }
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadGames();
        }

        private void SliderGameCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double gameCount = SliderGameCount.Value;
            LabelGameCount.Content = gameCount;
            if (gameCount <= 40)
            {
                rawgFilters.page_size = gameCount.ToString();
                rawgFilters.pages = "1";
            }

            else
            {
                double pages = Math.Ceiling(gameCount / 40);
                double page_size = gameCount / pages;
                rawgFilters.page_size = page_size.ToString();
                rawgFilters.pages = pages.ToString();
            }

        }

        private void ListViewReleases_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RawgGame selectedGame = ListViewReleases.SelectedItem as RawgGame;
            if (selectedGame != null)
            {
                GameDetailWindow window = new GameDetailWindow(selectedGame, rawgFilters.apiKey);
                window.ShowDialog();
            }
        }
    }
}
