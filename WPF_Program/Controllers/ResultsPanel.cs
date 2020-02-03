using Chinese;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfApp2;

using static MyTypes;

namespace WPF_program.Controllers
{
    public static partial class Controller
    {
        internal static void ShowChineseResult()
        {
            List<Word> filteredWords = ChineseService.searchBySimplified(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void ShowEnglishResult()
        {
            List<Word> filteredWords = ChineseService.getEnglishResult(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void ShowPronounciationResult()
        {
            List<Word> filteredWords = ChineseService.searchByPinyin(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void UpdateShownWords(List<Word> filteredWords)
        {
            mainWindow.ResultCountBlock.Text = $"{filteredWords.Count} words found";

            mainWindow.WordsList.Items.Clear();
            mainWindow.WordsList.Items.Add(mainWindow.ResultCountBlock);

            foreach (var word in filteredWords)
            {
                AddWordToResultsPanel(word);
            }
        }

        private static void AddWordToResultsPanel(Word word)
        {
            StackPanel sPanel = new StackPanel { Orientation = Orientation.Horizontal };

            List<char> singleChar = word.Simplified.ToList();
            List<string> singlePron = word.Pronounciation.Split(" ").ToList();

            for (int i = 0; i < singleChar.Count && i < singlePron.Count; i++)
            {
                TextBlock sBox = new TextBlock
                {
                    FontSize = 48,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = singleChar[i].ToString(),
                };
                //sBox.MouseEnter += (s, e) => SBox_MouseEnter(sBox.Text);
                sBox.MouseEnter += (e, s) =>
                {
                    //mainWindow.SearchBar.Text = sBox.Text;
                    mainWindow.ZoomedCharacterBox.Text = sBox.Text;
                    ShowCharacterDecomposition(sBox.Text[0]);
                };
                //
                sBox.MouseUp += (e, s) =>
                {
                    mainWindow.SearchBar.Text = sBox.Text;
                    List<Word> filteredWords = ChineseService.searchBySimplified(sBox.Text).ToList();
                    UpdateShownWords(filteredWords);
                    //ShowComposeResult();
                };

                TextBlock pBox = new TextBlock
                {
                    FontSize = 12,
                    Foreground = Brushes.DarkGreen,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(-10),
                };
                pBox.Inlines.Add(new Bold(new Run(singlePron[i])));

                StackPanel cPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };

                cPanel.Children.Add(sBox);
                cPanel.Children.Add(pBox);
                sPanel.Children.Add(cPanel);
            }

            StackPanel wordPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 15),
                Orientation = Orientation.Vertical,
            };
            Border wordBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8)
            };
            TextBlock definitionsBox = new TextBlock
            {
                FontSize = 16,
                Foreground = Brushes.Brown,
                Text = word.Definitions,
            };

            wordPanel.Children.Add(sPanel);
            wordPanel.Children.Add(definitionsBox);
            wordBorder.Child = wordPanel;
            mainWindow.WordsList.Items.Add(wordBorder);
        }

        internal static void ShowSomeRandomWords()
        {
            List<Word> randomWords = ChineseService.getRandomWords();
            UpdateShownWords(randomWords);
        }
    }
}
