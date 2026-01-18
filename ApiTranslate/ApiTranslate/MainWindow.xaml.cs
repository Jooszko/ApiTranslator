using System.Diagnostics.Contracts;
using System.IO;
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

namespace ApiTranslate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string subscriptionKey = "klucz_do_API";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string location = "francecentral";

        private static readonly HttpClient client = new HttpClient();

        private Dictionary<string, LanguageDetails> availableLanguages;

        public MainWindow()
        {
            InitializeComponent();

            LoadLanguagesAsync();

        }

        private async void LoadLanguagesAsync()
        {
            try
            {
                SourceLangComboBox.IsEnabled = false;
                TargetLangComboBox.IsEnabled = false;
                InputTextBox.Text = "Downloading language list...";

                string url = "https://api.cognitive.microsofttranslator.com/languages?api-version=3.0&scope=translation";
                string response = await client.GetStringAsync(url);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<LanguagesResponse>(response, options);

                if (result?.Translation != null)
                {
                    availableLanguages = result.Translation;

                    var targetList = availableLanguages.Select(lang => new LanguageItem
                    {
                        Code = lang.Key,
                        DisplayName = $"{lang.Value.Name} ({lang.Value.NativeName})"
                    }).OrderBy(x => x.DisplayName).ToList();

                    var sourceList = new List<LanguageItem>(targetList);

                    sourceList.Insert(0, new LanguageItem
                    {
                        Code = "",
                        DisplayName = "Detect Language (Auto)"
                    });

                    SourceLangComboBox.ItemsSource = sourceList;
                    SourceLangComboBox.DisplayMemberPath = "DisplayName";
                    SourceLangComboBox.SelectedValuePath = "Code";
                    SourceLangComboBox.SelectedIndex = 0; 

                    TargetLangComboBox.ItemsSource = targetList;
                    TargetLangComboBox.DisplayMemberPath = "DisplayName";
                    TargetLangComboBox.SelectedValuePath = "Code";

                    TargetLangComboBox.SelectedValue = "en";

                    InputTextBox.Text = ""; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Not able to download languages: {ex.Message}");
            }
            finally
            {
                SourceLangComboBox.IsEnabled = true;
                TargetLangComboBox.IsEnabled = true;
            }
        }


        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            string? fromLangCode = SourceLangComboBox.SelectedValue?.ToString();
            string? toLangCode = TargetLangComboBox.SelectedValue?.ToString();


            string? textToTranslate = InputTextBox.Text;


            if (string.IsNullOrWhiteSpace(textToTranslate))
            {
                OutputTextBox.Text = "Input the text to translate";
                return;
            }


            TranslateButton.Content = "Translating...";
            TranslateButton.IsEnabled = false;

            try
            {
                string translatedText = await TranslateTextAsync(textToTranslate, fromLangCode, toLangCode);
                OutputTextBox.Text = translatedText;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = "Error during translation: " + ex.Message;
            }
            finally
            {
                TranslateButton.Content = "Translate";
                TranslateButton.IsEnabled = true;
            }
        }

        private async Task<string> TranslateTextAsync(string text, string fromLanguage, string toLanguage)
        {
            string route = $"/translate?api-version=3.0&to={toLanguage}";

            if (!string.IsNullOrEmpty(fromLanguage))
            {
                route += $"&from={fromLanguage}";
            }

            object[] body = new object[] { new { Text = text } };
            string requestBody = JsonSerializer.Serialize(body);

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                HttpResponseMessage response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode}. {error}");
                }

                string result = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var translationResult = JsonSerializer.Deserialize<List<TranslationResultClass>>(result, options);

                if (translationResult != null && translationResult.Count > 0)
                {
                    var item = translationResult[0];

                    if(item.DetectedLanguage != null)
                    {
                        return $"[Detected Language: {item.DetectedLanguage.Language}]\n\n{item.Translations[0].Text}";
                    }

                    return item.Translations[0].Text;
                }

                return string.Empty;
            }
        }


        // Klasy pomocnicze do mapowania JSONa z Azure na obiekty C#

        public class LanguagesResponse
        {
            public Dictionary<string, LanguageDetails> Translation { get; set; }
        }

        public class LanguageDetails
        {
            public string Name { get; set; }
            public string NativeName { get; set; }
            public string Dir { get; set; }
        }

        public class LanguageItem
        {
            public string Code { get; set; }        
            public string DisplayName { get; set; } 
        }


        public class TranslationResultClass
        {
            public DetectedLanguageInfo DetectedLanguage { get; set; }
            public List<TranslationItem> Translations { get; set; }
        }

        public class DetectedLanguageInfo
        {
            public string Language { get; set; }
            public float Score { get; set; }
        }

        public class TranslationItem
        {
            public string Text { get; set; }
            public string To { get; set; }
        }


    }
}
