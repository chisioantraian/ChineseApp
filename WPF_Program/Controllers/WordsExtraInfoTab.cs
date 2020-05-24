using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static void ShowCharacterDecomposition(char characterToBeDecomposed)
        {
            mainWindow.tView.ItemsSource = GetTreeDecomposition(characterToBeDecomposed.ToString());
            mainWindow.DecompositionPanelTitle.Text = $"Decomposition to radicals of character {characterToBeDecomposed}";
        }

        //TODO ch, from string to ch
        internal static IEnumerable<TreeViewItem> GetTreeDecomposition(string ch)
        {
            if (!basicDict.ContainsKey(ch))
            {
                return new List<TreeViewItem>();
            }

            if (Kangxi.CheckIfKangxiRadical(ch[0]) || Kangxi.CheckIfStroke(ch[0]))
            {
                return new List<TreeViewItem> { new TreeViewItem { Header = CreateTreeInfoBox(ch[0], true) } };
            }

            List<TreeViewItem> result = basicDict[ch]
                                            .SelectMany(GetTreeDecomposition)
                                            .ToList();

            if (ChineseService.IsCharacter(ch[0]))
            {
                return new List<TreeViewItem> { new TreeViewItem { Header = CreateTreeInfoBox(ch[0], false), ItemsSource = result, IsExpanded = true } };
            }
            else
            {
                return result;
            }
        }

        //TODO remove isKangxi inside this method?
        internal static Border CreateTreeInfoBox(char character, bool isKangxi)
        {
            List<Word> words = ChineseService
                                .GetAllWordsFrom(character)
                                .ToList();
            if (words.Count() == 0)
            {
                Console.WriteLine($"{character} has no definition");
                return null;
            }

            Brush borderColor = isKangxi ? Brushes.Green : Brushes.Gray;
            Border charBorder = DecompositionTreeCharBorder(character, borderColor);
            Border detailBorder = DecompositionTreeDetailBorder(words);
            StackPanel wordPanel = DecompositionTreeWordPanel(charBorder, detailBorder);

            return DecompositionTreeWordBorder(wordPanel, borderColor);
        }

        private static Border DecompositionTreeWordBorder(StackPanel wordPanel, Brush borderColor)
        {
            return new Border
            {
                BorderBrush = borderColor,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 1, 1, 1),
                Padding = new Thickness(1),
                Child = wordPanel
            };
        }

        private static Border DecompositionTreeCharBorder(char character, Brush borderColor)
        {
            return new Border
            {
                BorderBrush = borderColor,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(2),
                Child = DecompositionTreeCharacterBlock(character)
            };
        }

        private static Border DecompositionTreeDetailBorder(List<Word> words)
        {
            return new Border
            {
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(2),
                Child = new TextBlock
                {
                    Text = GetOnlyDetails(words),
                    FontSize = 12,
                    Background = Brushes.White
                }
            };
        }

        private static StackPanel DecompositionTreeWordPanel(Border charBorder, Border detailBorder)
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };
            panel.Children.Add(charBorder);
            panel.Children.Add(detailBorder);
            return panel;
        }

        private static TextBlock DecompositionTreeCharacterBlock(char character)
        {
            TextBlock charBlock = new TextBlock
            {
                Text = character.ToString(),
                FontSize = 30,
                Foreground = Brushes.DarkSlateGray,
                Cursor = Cursors.Hand,
                Background = Brushes.White,
                ContextMenu = DecompositionTreeContextMenu(character),
            };
            charBlock.MouseEnter += (s, e) =>
            {
                charBlock.Foreground = Brushes.Black;
                charBlock.FontWeight = FontWeights.Bold;
            };
            charBlock.MouseLeave += (s, e) =>
            {
                charBlock.Foreground = Brushes.DarkSlateGray;
                charBlock.FontWeight = FontWeights.Normal;
            };
            return charBlock;
        }

        private static ContextMenu DecompositionTreeContextMenu(char character)
        {
            return new ContextMenu
            {
                Items =
                {
                    CharsWithComponentMenuItem(character),
                    WordsWithCaracterMenuItem(character)
                }
            };
        }

        private static MenuItem CharsWithComponentMenuItem(char character)
        {
            MenuItem menuItem = new MenuItem
            {
                Header = "Show characters which contain this component",
                FontSize = 16,
                Padding = new Thickness(10),
            };
            menuItem.Click += (s, e) => ShowCharsWithComponent_SidePanel(character);//ShowComposeResult(character);
            return menuItem;
        }

        private static MenuItem WordsWithCaracterMenuItem(char character)
        {
            MenuItem menuItem = new MenuItem
            {
                Header = "Show words with this character",
                FontSize = 16,
                Padding = new Thickness(10)
            };
            menuItem.Click += (s, e) => ShowWordsWithCharacter_SidePanel(character);//ShowChineseResult(character.ToString());
            return menuItem;
        }

        internal static string GetOnlyDetails(List<Word> words)
        {
            StringBuilder definition = new StringBuilder();
            foreach (Word w in words)
            {
                definition.Append($"{w.Pinyin}: {w.Definitions}\n");
            }
            return definition.ToString();
        }
    }
}
