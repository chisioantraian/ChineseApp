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
using System.Windows.Media;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static void ShowCharacterDecomposition(string characterToBeDecomposed)
        {
            mainWindow.tView.Items.Clear();
            mainWindow.tView.Items.Add(GetTreeDecomposition(characterToBeDecomposed, 0));
        }
        internal static TreeViewItem GetTreeDecomposition(string ch, int level)
        {


            if (ch.Length == 1 && (Kangxi.CheckIfKangxiRadical(ch[0]) || Kangxi.CheckIfStroke(ch[0])))
            {
                return new TreeViewItem
                {
                    Header = CreateBranchWord(ch, level, true),
                    Margin = new Thickness(30, 0, 0, 0),
                    IsExpanded = true
                };
            }

            TreeViewItem item = new TreeViewItem
            {
                Header = CreateBranchWord(ch, level),
                Margin = new Thickness(30, 0, 0, 0),
                IsExpanded = true
            };

            if (basicDict.ContainsKey(ch))
            {
                foreach (string c in basicDict[ch])
                {
                    //TreeViewItem child = GetTreeDecomposition(c, level + 1);
                    //if (child.Header.)
                    item.Items.Add(GetTreeDecomposition(c, level + 1));
                }
            }

            return item;
        }

        internal static Border CreateBranchWord(string ch, int level, bool isKangxi = false)
        {
            List<Word> words = ChineseService
                                .GetAllWordsFrom(new List<string> { ch.ToString() })
                                .ToList();
            string description = GetOnlyDetails(words);

            Brush color = Brushes.Gray;
            if (isKangxi)
                color = Brushes.Green;

            Border border = new Border
            {
                BorderBrush = color,//GetColorFromLevel(level),
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0,2,2,2),
                Padding = new Thickness(2),
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
            
            TextBlock charBlock = new TextBlock
            {
                Text = shownText,
                FontSize = 60,
                Foreground = shownColor,
                //Padding = new Thickness(5)
            };
            TextBlock detailBlock = new TextBlock
            {
                Text = description,
                FontSize = 16
            };

            Border charBorder = new Border
            {
                BorderBrush = color, //GetColorFromLevel(level),
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

            //panel.Children.Add(charBlock);
            //panel.Children.Add(detailBlock);

            panel.Children.Add(charBorder);
            panel.Children.Add(detailBorder);

            border.Child = panel;

            return border;
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
