using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

        // ch, from string to ch
        internal static IEnumerable<TreeViewItem> GetTreeDecomposition(string ch)
        {
            if (!basicDict.ContainsKey(ch.ToString()))
            {
                return new List<TreeViewItem>();
            }

            if (Kangxi.CheckIfKangxiRadical(ch[0]) || Kangxi.CheckIfStroke(ch[0]))
            {
                return new List<TreeViewItem> { new TreeViewItem { Header = CreateBranchWord(ch[0], true) } };
            }

            List<TreeViewItem> result = basicDict[ch.ToString()]
                                            .SelectMany(c => GetTreeDecomposition(c))
                                            .ToList();

            if (ChineseService.IsCharacter(ch[0]))
            {
                return new List<TreeViewItem> { new TreeViewItem { Header = CreateBranchWord(ch[0], false), ItemsSource = result, IsExpanded = true } };
            }
            else
            {
                return result;
            }

        }
        /*
        internal static IEnumerable<TreeViewItem> GetTreeDecomposition(char ch)
        {
            if (Kangxi.CheckIfKangxiRadical(ch) || Kangxi.CheckIfStroke(ch))
            {
                return new List<TreeViewItem> { new TreeViewItem { Header = CreateBranchWord(ch, true) } };
            }

            if (!basicDict.ContainsKey(ch.ToString()))
            {
                return new List<TreeViewItem>();
            }

            List<TreeViewItem> result = basicDict[ch.ToString()]
                                            .SelectMany(c => GetTreeDecomposition(c[0]))
                                            .ToList();

            if (ChineseService.IsCharacter(ch))
            {
                return new List<TreeViewItem> { new TreeViewItem { Header = CreateBranchWord(ch, false), ItemsSource = result, IsExpanded = true } };
            }
            else
            {
                return result;
            }
        }*/
        

        //TODO remove isKangxi inside this method?
        internal static Border CreateBranchWord(char ch, bool isKangxi)
        {
            List<Word> words = ChineseService
                                .GetAllWordsFrom(new List<string> { ch.ToString() })
                                .ToList();
            
            //if (words.Count() == 0)
            //{
            //    return null;
            //}

            string description = GetOnlyDetails(words);

            Brush color = Brushes.Gray;
            if (isKangxi)
                color = Brushes.Green;

            Border border = new Border
            {
                BorderBrush = color,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0,1,1,1),
                Padding = new Thickness(1),
            };
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            string shownText = ch.ToString();

            MenuItem item1 = new MenuItem
            {
                Header = "Show characters which contain this component",
                FontSize = 16,
                Padding = new Thickness(10),
            };
            //item1.Click += (s,e) => Controller.ShowComposeResult(shownText);
            item1.Click += (s, e) => Controller.ShowComposeResult(ch);

            MenuItem item2 = new MenuItem
            {
                Header = "Show words with this character",
                FontSize = 16,
                Padding = new Thickness(10)
            };
            item2.Click += (s, e) => Controller.ShowChineseResult(shownText);

            ContextMenu menu = new ContextMenu
            {
                Items = { item1, item2 }
            };
            TextBlock charBlock = new TextBlock
            {
                Text = shownText,
                FontSize = 30,
                Foreground = Brushes.DarkSlateGray,
                Cursor = Cursors.Hand,
                ContextMenu = menu,
                Background = Brushes.White
            };
            charBlock.MouseEnter += (s, e) =>
            {
                charBlock.Text = "";
                charBlock.Foreground = Brushes.Black;
                charBlock.Inlines.Add(new Run(shownText) { FontWeight = FontWeights.Bold });
            };
            charBlock.MouseLeave += (s, e) =>
            {
                charBlock.Text = shownText;
                charBlock.Foreground = Brushes.DarkSlateGray;
            };
            TextBlock detailBlock = new TextBlock
            {
                Text = description,
                FontSize = 12,
                Background = Brushes.White
            };

            Border charBorder = new Border
            {
                BorderBrush = color,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(2),
            };
            Border detailBorder = new Border
            {
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(2),
            };

            charBorder.Child = charBlock;
            detailBorder.Child = detailBlock;

            panel.Children.Add(charBorder);
            panel.Children.Add(detailBorder);

            border.Child = panel;

            return border;
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
