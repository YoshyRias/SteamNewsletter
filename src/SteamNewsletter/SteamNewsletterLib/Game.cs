using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SteamNewsletterLib
{
    public class Game
    {
        [JsonPropertyName("gid")]
        public string Gid { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("is_external_url")]
        public bool IsExternalUrl { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("contents")]
        public string Contents { get; set; }

        [JsonPropertyName("feedlabel")]
        public string FeedLabel { get; set; }

        [JsonPropertyName("date")]
        public long Date { get; set; }

        [JsonPropertyName("feedname")]
        public string Name { get; set; }

        [JsonPropertyName("feed_type")]
        public int FeedType { get; set; }

        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        public List<JToken> RecentUpdates { get; set; } = new List<JToken>();

        public Game(int appID)
        {
            AppId = appID;
        }
        public Game(int appID, string name) : this(appID)
        {
            Name = name;
        }


        public void ShowUpdatLog()
        {

        }

        // ChatGPT: How do I access the Update Logs of a Steam Game
        // ChatGPT: what data type does json have
        // ChatGPT: What is a JToken

        public async Task GetGameUpdates()
        {
            string url = $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={AppId}&count=10";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                JObject json = JObject.Parse(response);

                RecentUpdates = json["appnews"]?["newsitems"]?.ToList() ?? new List<JToken>();
            }


        }

        // ChatGPT: Can you visuallize the jason data fromt the api and clean it
        // ChatGPT: How can I make the links work
        // ChatGPT: How do i render the images

        public async Task VisuallizeUpdates(StackPanel panel)
        {
            await GetGameUpdates();
            panel.Children.Clear();

            foreach (var news in RecentUpdates)
            {
                string title = news["title"]?.ToString();
                string rawContent = news["contents"]?.ToString() ?? "";

                long unixTime = long.Parse(news["date"]?.ToString() ?? "0");
                DateTime date = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;

                // Create base stack
                var contentStack = new StackPanel();

                // Title
                contentStack.Children.Add(new TextBlock
                {
                    Text = title,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 5)
                });

                // Date
                contentStack.Children.Add(new TextBlock
                {
                    Text = date.ToString("dd.MM.yyyy HH:mm"),
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 0, 0, 5)
                });

                // Extract <img src="..."> and add images
                foreach (Match match in Regex.Matches(rawContent, "<img[^>]*src=[\"']([^\"']+)[\"'][^>]*>", RegexOptions.IgnoreCase))
                {
                    string imageUrl = match.Groups[1].Value;

                    if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult) &&
                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        try
                        {
                            var image = new Image
                            {
                                Source = new System.Windows.Media.Imaging.BitmapImage(uriResult),
                                Height = 200,
                                Margin = new Thickness(0, 0, 0, 5),
                                Stretch = Stretch.UniformToFill
                            };
                            contentStack.Children.Add(image);
                        }
                        catch
                        {
                            // Skip invalid or unreachable images
                        }
                    }
                }

                // Build TextBlock with clickable links
                var textBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14
                };

                string content = System.Net.WebUtility.HtmlDecode(rawContent);
                content = Regex.Replace(content, "<img[^>]+>", ""); // remove images
                content = ConvertBBCodeLinks(content);              // convert BBCode to <a> format

                string pattern = @"<a href=[""'](.*?)[""']>(.*?)<\/a>";
                int lastIndex = 0;

                foreach (Match match in Regex.Matches(content, pattern))
                {
                    int index = match.Index;

                    // Text before the link
                    string beforeLink = content.Substring(lastIndex, index - lastIndex);
                    textBlock.Inlines.Add(new Run(StripHtml(beforeLink)));

                    // Extract link
                    string url = match.Groups[1].Value;
                    string linkText = StripHtml(match.Groups[2].Value);

                    if (Uri.TryCreate(url, UriKind.Absolute, out Uri linkUri) &&
                        (linkUri.Scheme == Uri.UriSchemeHttp || linkUri.Scheme == Uri.UriSchemeHttps))
                    {
                        var hyperlink = new Hyperlink(new Run(linkText))
                        {
                            NavigateUri = linkUri,
                            Foreground = Brushes.Blue,
                            TextDecorations = TextDecorations.Underline
                        };

                        // Safe handling of hyperlink click
                        hyperlink.RequestNavigate += (sender, e) =>
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
                                {
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to open link: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            e.Handled = true;
                        };

                        textBlock.Inlines.Add(hyperlink);
                    }
                    else
                    {
                        // If invalid URL, render as plain text
                        textBlock.Inlines.Add(new Run(linkText));
                    }

                    lastIndex = index + match.Length;
                }

                // Remaining text after the last link
                if (lastIndex < content.Length)
                {
                    textBlock.Inlines.Add(new Run(StripHtml(content.Substring(lastIndex))));
                }
                // Remaining text
                if (lastIndex < content.Length)
                {
                    textBlock.Inlines.Add(new Run(StripHtml(content.Substring(lastIndex))));
                }

                contentStack.Children.Add(textBlock);

                var border = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Background = Brushes.WhiteSmoke,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    Child = contentStack
                };

                panel.Children.Add(border);
            }
        }

        private string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        private string ConvertBBCodeLinks(string input)
        {
            // Converts [url=https://...]Text[/url] ? <a href="https://...">Text</a>
            return Regex.Replace(input, @"\[url=(.*?)\](.*?)\[/url\]", "<a href=\"$1\">$2</a>", RegexOptions.IgnoreCase);
        }

        private string CleanContent(string content)
        {
            // Remove HTML tags
            content = Regex.Replace(content, "<.*?>", string.Empty);

            // Convert BBCode links to normal URLs
            content = Regex.Replace(content, @"\[url=(.*?)\](.*?)\[/url\]", "$2 ($1)", RegexOptions.IgnoreCase);

            // Remove remaining BBCode tags (like [b], [i])
            content = Regex.Replace(content, @"\[(.*?)\]", string.Empty);

            // Optionally decode HTML entities (like &amp;)
            content = System.Net.WebUtility.HtmlDecode(content);

            return content.Trim();
        }


    }

}