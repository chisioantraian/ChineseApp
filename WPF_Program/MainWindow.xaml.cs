using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ChineseAppWPF.Controllers;
using ChineseAppWPF.Logic;

namespace ChineseAppWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ChineseService.InitializeData();
            Decomposition.BuildDecompositionDict();
            Controller.SetWindow(this);

            Controller.InitializeRules();
            Controller.InitializeStatistics();
            Controller.InitializeSentenceExamples();

            Controller.ShowSomeRandomWords();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => Controller.ShowResult();

        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();

        private void SearchBar_KeyUp(object sender, KeyEventArgs e) => Controller.ShowResult();

        private void CharacterAndPinyin_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            string character = textBlock.Text;

            textBlock.Cursor = Cursors.Hand;

            textBlock.Text = "";
            textBlock.Foreground = Brushes.Black;
            textBlock.Inlines.Add(new Run(character){ FontWeight = FontWeights.Bold });
            Controller.ShowWordWithThisCharacter(textBlock.Text[0]);
        }

        private void CharacterAndPinyin_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            string character = textBlock.Text;

            textBlock.Text = "";
            textBlock.Inlines.Add(character);
            textBlock.Foreground = Brushes.DarkSlateGray;
        }

        private void CharacterAndPinyin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            SearchBar.Text = textBlock.Text;
            Controller.ShowEnglishChineseResult();
        }

        private void SentenceAnalysis_KeyUp(object sender, KeyEventArgs e) => Controller.AnalyseSentence();

        private void AnalyseSentence_Click(object sender, RoutedEventArgs e) => Controller.AnalyseSentence_TestTab();

        private void SaveSentence_Click(object sender, RoutedEventArgs e) => Controller.AddSentenceBreakdownToTests();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Controller.SaveTestSentences();

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Controller.SortResult();

        private void WritingSystemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Controller.ChangeWritingSystem();

        private void InputComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Controller.ShowResult();

        private void CharactersWithComponent_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            string value = item.Tag.ToString();

            Controller.ShowComposeResult(value);
        }

        private void WordsWithCharacter_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            string value = item.Tag.ToString();

            Controller.ShowEnglishChineseResult(value); //change
        }


    }
}
