using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();



        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceSelectedItem = (ComboBoxItem)SourceLangComboBox.SelectedItem;
            var targetSelectedItem = (ComboBoxItem)TargetLangComboBox.SelectedItem;

            string? sourceLanguage = sourceSelectedItem.Content.ToString();
            string? targetLanguage = targetSelectedItem.Content.ToString();

            string? textToTranslate = InputTextBox.Text;
            string? translatedText = "";




            if (sourceLanguage.Equals(targetLanguage))
            {
                OutputTextBox.Text = "Nie można przetłumaczyć na ten sam język!";
            }
            else
            {
                if (textToTranslate.Equals(""))
                {
                    OutputTextBox.Text = "Nie wprowadziłeś tekstu do tłumaczenia";
                }
                else
                {

                    if (sourceLanguage.Equals("Polski"))
                    {
                        sourceLanguage = "polish";
                    }
                    else if (sourceLanguage.Equals("Angielski"))
                    {
                        sourceLanguage = "english";
                    }
                    else if (sourceLanguage.Equals("Niemiecki"))
                    {
                        sourceLanguage = "german";
                    }
                    else if (sourceLanguage.Equals("Hiszpański"))
                    {
                        sourceLanguage = "spanish";
                    }



                    if (targetLanguage.Equals("Polski"))
                    {
                        targetLanguage = "polish";
                    }
                    else if (targetLanguage.Equals("Angielski"))
                    {
                        targetLanguage = "english";
                    }
                    else if (targetLanguage.Equals("Niemiecki"))
                    {
                        targetLanguage = "german";
                    }
                    else if (targetLanguage.Equals("Hiszpański"))
                    {
                        targetLanguage = "spanish";
                    }

                    //pewnie w api będą języki po ang stąd taka zamiana, ew możemy od razu dawać nazwy zgodne z api ale tak ładniej
                    //chyba że będziemy pobierać listę języków z bazy



                    //tu miejsce na łączenie api i obsługę itd
                    // textToTranslate-text do tłumaczenia 









                    OutputTextBox.Text = targetLanguage; //wypisanie gotowego textu




                }
            }
           

            

        }
    }
}