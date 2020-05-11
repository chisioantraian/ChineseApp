using System;
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
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            InitializeComponent();

            ChineseService.InitializeData();
            Decomposition.BuildDecompositionDict();
            Controller.SetWindow(this);

            Controller.InitializeRules();
            Controller.InitializeStatistics();
            Controller.InitializeSentenceExamples();

            Controller.ShowSomeRandomWords();
            UndoButton.IsEnabled = false;
        }

        private void Undo_Click(object sender, RoutedEventArgs e) => Controller.Undo();

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e) => Controller.ChangeLanguage();

        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();

        private void SearchBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Controller.ShowResult();
            }
        }

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
                textBlock.Text = "";
                textBlock.Inlines.Add(new Run(character.ToString()) { FontWeight = FontWeights.Bold });
            }
        }

        private void CharacterAndPinyin_MouseUp(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            MouseButtonEventArgs ev = (MouseButtonEventArgs)e;
            char character = textBlock.Text[0];

            if (ev.ChangedButton == MouseButton.Left)
            {
                Controller.ShowWordWithThisCharacter(character);
                Controller.ShowCharsWithComponent_SidePanel(character);
                Controller.ShowWordsWithCharacter_SidePanel(character);
            }
        }

        private void CharacterAndPinyin_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            char character = textBlock.Text[0];

            if (character != '〔' &&
                character != '〕' &&
                character != '-' &&
                character != ' ')
            {
                textBlock.Text = character.ToString();
            }
        }

        private void SentenceAnalysis_KeyUp(object sender, KeyEventArgs e) => Controller.AnalyseSentence();

        private void AnalyseSentence_Click(object sender, RoutedEventArgs e) => Controller.AnalyseSentence_TestTab();

        private void SaveSentence_Click(object sender, RoutedEventArgs e) => Controller.AddSentenceBreakdownToTests();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Controller.SaveTestSentences();

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Controller.SortResult();

        private void CharactersWithComponent_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string value = item.Tag.ToString();

            Controller.ShowComposeResult(value);
        }

        private void WordsWithCharacter_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string value = item.Tag.ToString();

            Controller.ShowChineseResult(value);
        }

    }
}
