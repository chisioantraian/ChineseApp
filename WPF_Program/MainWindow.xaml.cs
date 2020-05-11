using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ChineseAppWPF.Controllers;
using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;

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

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            Controller.ChangeLanguage();
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();

        private void SearchBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Controller.ShowResult();
            }
        }

        private string fillers = "〔〕-";

        private void CharacterAndPinyin_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            //string character = textBlock.Text;
            //if (!fillers.Contains(character))
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
            var textBlock = (TextBlock)sender;
            MouseButtonEventArgs ev = (MouseButtonEventArgs)e;
            if (ev.ChangedButton == MouseButton.Left)
            {
                Controller.ShowWordWithThisCharacter(textBlock.Text[0]);
                Controller.ShowCharsWithComponent_SidePanel(textBlock.Text[0]);
                Controller.ShowWordsWithCharacter_SidePanel(textBlock.Text[0]);
            }
        }

        private void CharacterAndPinyin_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            //string character = textBlock.Text;
            //if (!fillers.Contains(character))
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

            Controller.ShowChineseResult(value); //change
        }


    }
}
