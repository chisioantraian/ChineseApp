using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ChineseAppWPF.Controllers;
using ChineseAppWPF.Logic;

namespace ChineseAppWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Needed to be able to print asian characters to the console
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ChineseService.InitializeData();
            Decomposition.BuildDecompositionDict();
            Controller.SetWindow(this);

            Controller.InitializeRules();
            //Controller.InitializeStatistics();
            //Controller.InitializeSentenceExamples();
            Controller.ShowSomeRandomWords();

        }
        private void SearchBar_KeyUp(object sender, KeyEventArgs e)
        {
            //TODO if search too slow, change to search on enter; 
            if (e.Key == Key.Enter)
                Controller.ShowResult();
        }

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e) => Controller.ChangeLanguage();

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Controller.SortResult();

        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();

        private void CharacterAndPinyin_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            char character = textBlock.Text[0];

            if (character != '〔' &&
                character != '〕' &&
                character != '-' &&
                character != ' ')
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
                Controller.ShowWordWithThisCharacter(character);
                Controller.ShowCharsWithComponent_SidePanel(character);
                Controller.ShowWordsWithCharacter_SidePanel(character);
            }
        }

        private void WordsWithCharacter_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string value = item.Tag.ToString();
            Controller.ShowChineseResult(value);
        }

        private void CharactersWithComponent_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            char component = item.Tag.ToString()[0];
            Controller.ShowComposeResult(component);
        }

        private void SentenceAnalysis_KeyUp(object sender, KeyEventArgs e) => Controller.AnalyseSentence();

        private void AnalyseSentence_Click(object sender, RoutedEventArgs e) => Controller.AnalyseSentence_TestTab();

        private void SaveSentence_Click(object sender, RoutedEventArgs e) => Controller.AddSentenceBreakdownToTests();

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Controller.SaveTestSentences();

            //Controller.SerializeWords();
            //Decomposition.SerializeWordsDecomposition();
        }
    }
}
