using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChineseAppWPF.Service;
using ChineseAppWPF.UI;

namespace ChineseAppWPF
{


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Needed to be able to print asian characters to the console
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            UiLayer.SetWindow(this);
            UiLayer.InitializeStatistics();
            UiLayer.InitializeSentenceExamples();
            UiLayer.ShowSomeRandomWords();
        }

        private void SearchBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UiLayer.ShowResult();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.ContextMenu.IsOpen = true;
        }

        private void ChangeToEnglishInput_Click(object sender, RoutedEventArgs e) => UiLayer.ChangeToEnglishInput();

        private void ChangeToChineseInput_Click(object sender, RoutedEventArgs e) => UiLayer.ChangeToChineseInput();

        private void ShowRandomWords_Click(object sender, RoutedEventArgs e) => UiLayer.ShowSomeRandomWords();

        private void CharacterAndPinyin_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            char character = textBlock.Text[0];

            if (character.IsNotExtraCharacter())
            {
                textBlock.Cursor = Cursors.Hand;
                textBlock.FontWeight = FontWeights.Bold;
            }
        }

        private void CharacterAndPinyin_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            textBlock.FontWeight = FontWeights.Normal;
        }

        private void CharacterAndPinyin_MouseUp(object sender, RoutedEventArgs e) 
        {
            MouseButtonEventArgs ev = (MouseButtonEventArgs)e;

            if (ev.ChangedButton == MouseButton.Left)
            {
                TextBlock textBlock = (TextBlock)sender;
                char character = textBlock.Text[0];
                if (character.IsNotExtraCharacter())
                {
                    UiLayer.ShowDecompositionTreeOfCharacter(character);
                    UiLayer.ShowCharsWithComponent_SidePanel(character);
                    UiLayer.ShowWordsWithCharacter_SidePanel(character);
                }
            }
        }

        private void ShowDecompositionTree_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            char character = item.Tag.ToString()[0];

            if (character.IsNotExtraCharacter())
            {
                UiLayer.ShowDecompositionTreeOfCharacter(character);
            }
        }

        private void WordsWithCharacter_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            char character = item.Tag.ToString()[0];

            if (character.IsNotExtraCharacter())
            {
                UiLayer.ShowWordsWithCharacter_SidePanel(character);
            }
        }

        private void WordsContainingWord_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string simplified = item.Tag.ToString();
            UiLayer.ShowWordsContainingWord_SidePanel(simplified);
        }

        private void CharactersWithComponent_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            char component = item.Tag.ToString()[0];

            if (component.IsNotExtraCharacter())
            {
                UiLayer.ShowCharsWithComponent_SidePanel(component);
            }
        }

        private void ShowWordsInside_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string simplified = item.Tag.ToString();
            UiLayer.ShowWordsInside_SidePanel(simplified);
        }

        private void SortByFrequency_Click(object sender, RoutedEventArgs e) => UiLayer.SortByFrequency();

        private void SortByNumberOfStrokes_Click(object sender, RoutedEventArgs e) => UiLayer.SortByNumberOfStrokes();

        private void SortByPinyin_Click(object sender, RoutedEventArgs e) => UiLayer.SortByPinyin();

        private void SortByExact_Click(object sender, RoutedEventArgs e) => UiLayer.SortByExact();

        private void SentenceAnalysis_KeyUp(object sender, KeyEventArgs e) => UiLayer.AnalyseSentence();

        private void AnalyseSentence_Click(object sender, RoutedEventArgs e) => UiLayer.AnalyseSentence_TestTab();

        private void SaveSentence_Click(object sender, RoutedEventArgs e) => UiLayer.AddSentenceBreakdownToTests();

        private void Window_Closing(object sender, CancelEventArgs e) => UiLayer.SaveTestSentences();
    }
}
