using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
            Console.WriteLine("test 12");
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
            ZoomedCharacterBox.Text = textBlock.Text;
            Controller.ShowWordWithThisCharacter(textBlock.Text[0]);
        }

        private void CharacterAndPinyin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            SearchBar.Text = textBlock.Text;
            Controller.ShowChineseResult();
        }

        private void SentenceAnalysis_KeyUp(object sender, KeyEventArgs e) => Controller.AnalyseSentence();

        private void AnalyseSentence_Click(object sender, RoutedEventArgs e) => Controller.ShowGrammarAnalysis();

        private void SentenceAnalysis_Click(object sender, RoutedEventArgs e) => Controller.AnalyseSentence();

        private void SaveSentence_Click(object sender, RoutedEventArgs e) => Controller.AddSentenceBreakdownToTests();

        //private void ToggleWords_Click(object sender, RoutedEventArgs e) => Controller.ToggleWordsPanel();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Controller.SaveTestSentences();
    }
}
