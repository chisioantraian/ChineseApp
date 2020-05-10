using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
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
        private static void ShowCharacterDecomposition(string characterToBeDecomposed, string writingState)
        {
            mainWindow.tView.Items.Clear();
            var itemsToAdd = GetTreeDecomposition(characterToBeDecomposed, 0, writingState);
            foreach (TreeViewItem item in itemsToAdd)
            {
                mainWindow.tView.Items.Add(item);
            }
        }
        internal static List<TreeViewItem> GetTreeDecomposition(string ch, int level, string writingState)
        {


            if (ch.Length == 1 && (Kangxi.CheckIfKangxiRadical(ch[0]) || Kangxi.CheckIfStroke(ch[0])))
            {
                return new List<TreeViewItem>
                {
                    new TreeViewItem
                    {
                        Header = CreateBranchWord(ch, level, true, writingState),
                        //Margin = new Thickness(10, 0, 0, 0),
                        //IsExpanded = true
                    }
                };
            }

            Border b = CreateBranchWord(ch, level, false, writingState);
            //if (b == null)
            //{
            //    return new List<TreeViewItem>();
            //}
            TreeViewItem item = new TreeViewItem
            {
                Header = b,
                //Margin = new Thickness(5, 0, 0, 0),
                IsExpanded = true
            };

            
            if (ch == null || ch == "")
            {
                return new List<TreeViewItem> { item };
            }

            if (ChineseService.IsCharacter(ch[0], writingState))
            {
                if (basicDict.ContainsKey(ch))
                {
                    foreach (string c in basicDict[ch])
                    {
                        foreach (var child in GetTreeDecomposition(c, level + 1, writingState))
                            item.Items.Add(child);
                    }
                }
                return new List<TreeViewItem> { item };
            }
            else
            {
                if (basicDict.ContainsKey(ch))
                {
                    List<TreeViewItem> result = new List<TreeViewItem>();
                    foreach (string c in basicDict[ch])
                    {
                        foreach (var child in GetTreeDecomposition(c, level + 1, writingState))
                        {
                            result.Add(child);
                        }
                    }
                    return result;
                }
            }
            return new List<TreeViewItem>();
        }


        internal static Border CreateBranchWord(string ch, int level, bool isKangxi, string writingState)
        {
            List<Word> words = ChineseService
                                .GetAllWordsFrom(new List<string> { ch.ToString() }, writingState)
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

            string shownText = ch;
            if (ch.Length > 1)
                shownText = "*";
            Brush shownColor = Brushes.Black;
            if (shownText == "*")
                shownColor = Brushes.Yellow;

            MenuItem item1 = new MenuItem
            {
                Header = "Show characters which contain this component",
                FontSize = 16,
                Padding = new Thickness(10),
            };
            item1.Click += (s,e) => Controller.ShowComposeResult(shownText);

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
                ContextMenu = menu
            };
            charBlock.MouseEnter += (s, e) =>
            {
                charBlock.Text = "";
                charBlock.Foreground = Brushes.Black;
                charBlock.Inlines.Add(new Run(shownText) { FontWeight = FontWeights.Bold });
            };
            charBlock.MouseLeave += (s, e) =>
            {
                charBlock.Text = "";
                charBlock.Inlines.Add(shownText);
                charBlock.Foreground = Brushes.DarkSlateGray;
            };
            TextBlock detailBlock = new TextBlock
            {
                Text = description,
                FontSize = 12
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

        private static void Item1_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static string GetOnlyDetails(List<Word> words)
        {
            string definition = "";
            foreach (Word w in words)
            {
                definition += $"{w.Pinyin}: {w.Definitions}\n";
            }
            return definition;
        }

        internal static Brush GetColorFromLevel(int level)
        {
            return level switch
            {
                0 => Brushes.DarkRed,
                1 => Brushes.Orange,
                2 => Brushes.Olive,
                3 => Brushes.Green,
                4 => Brushes.Blue,
                5 => Brushes.Purple,
                _ => Brushes.Gray
            };
        }

    }
}
