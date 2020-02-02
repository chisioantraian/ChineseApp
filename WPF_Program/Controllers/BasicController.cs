using Chinese;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp2;

using static MyTypes;

namespace WPF_program.Controllers
{
    public static partial class Controller
    {
        private static MainWindow mainWindow;
        private static List<Word> allWords;
        private static List<DetailedWord> allDetailedWords;
        private static Dictionary<char, List<char>> dict;

        public static void setWindow(MainWindow window)
        {
            mainWindow = window;
            allWords = ChineseService.getAllWords();
            allDetailedWords = ChineseService.getAllDetailedWords().ToList();
            dict = Decomposition.getCharacterDecomposition();
        }

        public static void ShowResult(Key lastEnteredKey = Key.Enter)
        {
            if (lastEnteredKey != Key.Enter)
                return;
            ComboBoxItem typeItem = (ComboBoxItem)mainWindow.InputComboBox.SelectedItem;

            switch (typeItem.Content.ToString())
            {
                case "Chinese": ShowChineseResult(); break;
                case "English": ShowEnglishResult(); break;
                case "Pronounciation": ShowPronounciationResult(); break;
                case "Compose": ShowComposeResult(); break;
            }
        }

        private static void ShowCharacterDecomposition(char characterToBeDecomposed)
        {
            if (!dict.ContainsKey(characterToBeDecomposed))
                return;
            string decompositionText = Decomposition.decomposeCharToRadicals(characterToBeDecomposed);
            mainWindow.DecompositionBlock.Text = $"{characterToBeDecomposed} : {decompositionText} ";
        }

        // From a single character, show all other chinese characters which contain it as a component
        private static void ShowComposeResult()
        {
            List<Word> filteredWords = Decomposition.getCharactersWithComponent(mainWindow.SearchBar.Text);
            UpdateShownWords(filteredWords);
        }

        // Split a sentence into words, show these words and analyze the sentence
        private static void ShowDecomposed(string sentence)
        {
            List<Word> result = GetWordsFromSentence(sentence);
            UpdateShownWords(result);

            mainWindow.MiddleWordBox.Children.Clear();
            foreach (Word w in result)
            {
                DetailedWord? detailedWord = allDetailedWords.Find(dw => dw.Simplified == w.Simplified);
                (SolidColorBrush, string) posTuple = GetPosInfo(detailedWord);

                TextBlock wBlock = new TextBlock
                {
                    FontSize = 32,
                    Foreground = posTuple.Item1,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5),
                    Text = w.Simplified,
                };
                Border wordBorder = new Border
                {
                    BorderBrush = posTuple.Item1,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(5)
                };
                TextBlock posBox = new TextBlock
                {
                    FontSize = 18,
                    Foreground = posTuple.Item1,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, -5, 0, 0),
                    Text = $"{posTuple.Item2}"
                    //Text = $"{posTuple.Item2} \n {detailedWord.AllPos} \n {detailedWord.AllPosFreq}", //modify
                };
                StackPanel wordPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };

                wordPanel.Children.Add(wBlock);
                wordPanel.Children.Add(posBox);
                wordBorder.Child = wordPanel;
                mainWindow.MiddleWordBox.Children.Add(wordBorder);
            }
        }

        // Split a sentence represented as a string into a list of words
        private static List<Word> GetWordsFromSentence(string sentence)
        {
            string constructedWord = string.Empty;
            var resultedWord = new List<Word>();
            var toInsert = new List<Word>();
            var result = new List<Word>();

            foreach (char curr in sentence)
            {
                resultedWord = GetResultedWord(constructedWord + curr);
                if (resultedWord.Count > 0)
                {
                    toInsert = resultedWord;
                    constructedWord += curr;
                }
                else
                {
                    if (toInsert.Count > 0)
                        result.Add(toInsert[0]); //toInsert.ForEach(w => result.Add(w));
                    toInsert = GetResultedWord(curr.ToString());
                    constructedWord = curr.ToString();
                }
            }
            if (toInsert.Count > 0)
                result.Add(toInsert[0]); //toInsert.ForEach(w => result.Add(w));
            return result;
        }

        // From a word(DetailedWord) pos tag, get its full pos name , and also return a color, unique to it
        private static (SolidColorBrush, string) GetPosInfo(DetailedWord? detailedWord)
        {
            if (detailedWord == null)
                return (Brushes.Gray, "_");
            return detailedWord.DominantPos switch
            {
                "a" => (Brushes.Purple, "adjective"),
                "ad" => (Brushes.Purple, "adj as adv"),
                "ag" => (Brushes.Purple, "adj morpheme"),
                "an" => (Brushes.Purple, "adj w. nom fun"),
                "b" => (Brushes.Purple, "non-pred adje"),
                "c" => (Brushes.Pink, "conjunction"),
                "cc" => (Brushes.Pink, "conjunction"),
                "d" => (Brushes.LightBlue, "adverb"),
                "dg" => (Brushes.LightBlue, "adv morpheme"),
                "e" => (Brushes.DarkBlue, "interjection"),
                "f" => (Brushes.DarkBlue, "dir. locality"),
                "g" => (Brushes.DarkBlue, "morpheme"),
                "h" => (Brushes.DarkBlue, "prefix"),
                "i" => (Brushes.Cyan, "idiom"),
                "j" => (Brushes.DarkBlue, "interjection"),
                "k" => (Brushes.DarkBlue, "suffix"),
                "l" => (Brushes.DarkBlue, "fixed expr"),
                "m" => (Brushes.Orange, "numeral"),
                "mg" => (Brushes.Orange, "num morpheme"),
                "mq" => (Brushes.Orange, "num classifier"),
                "n" => (Brushes.Red, "noun"),
                "ng" => (Brushes.Red, "noun morpheme"),
                "nr" => (Brushes.Red, "pers name"),
                "ns" => (Brushes.Red, "place name"),
                "nt" => (Brushes.Red, "org name"),
                "nx" => (Brushes.Red, "nom. string"),
                "nz" => (Brushes.Red, "other prop. noun"),
                "o" => (Brushes.LightBlue, "onomatopoeia"),
                "p" => (Brushes.Aquamarine, "preposition"),
                "q" => (Brushes.DarkKhaki, "classifier"),
                "r" => (Brushes.DarkOrange, "pronoun"),
                "rg" => (Brushes.DarkOrange, "pron. morpheme"),
                "s" => (Brushes.Beige, "space word"),
                "t" => (Brushes.SandyBrown, "time word"),
                "tg" => (Brushes.SandyBrown, "time morpheme"),
                "u" => (Brushes.DarkBlue, "auxiliary"),
                "v" => (Brushes.LightGreen, "verb"),
                "vd" => (Brushes.LightGreen, "vb as adv"),
                "vg" => (Brushes.LightGreen, "vb morpheme"),
                "vn" => (Brushes.LightGreen, "vb nom fun"),
                "w" => (Brushes.DarkBlue, "symbol.."),
                "x" => (Brushes.Gold, "unclassififed"),
                "y" => (Brushes.Brown, "modal part."),
                "z" => (Brushes.Honeydew, "descriptive"),
                _ => (Brushes.Gray, "_")
            };
        }

        // Get all chinese words which have the simplified form equal to the passed string
        private static List<Word> GetResultedWord(string simpl)
        {
            return allWords.Where(w => w.Simplified == simpl).ToList();
        }


        // Get a character's decomposition, till radicals/strokes level
        /*private string GetNiceDecomposed(char c)
        {
            string result = String.Empty;

            Queue<char> qu = new Queue<char>();
            qu.Enqueue(c);

            while (qu.Count > 0)
            {
                char curr = qu.Dequeue();
                if (dict.ContainsKey(curr))
                {
                    List<char> currList = dict[curr];
                    if (currList == null)
                    {
                        result += $"{curr} : Kangxi radical\n";
                    }
                    else if (currList.Count >= 2)
                    {
                        result += $"{curr} : {currList[0]}   {currList[1]}\n";
                        qu.Enqueue(currList[0]);
                        qu.Enqueue(currList[1]);
                    }
                    else if (currList.Count == 1)
                    {
                        result += $"{curr} : {currList[0]}\n";
                        qu.Enqueue(currList[0]);
                    }
                    else
                    {
                        result += $"{curr} : basic stroke\n";
                    }
                }
            }

            result += "\n definitions : \n";

            foreach (Word w in allWords)
            {
                if (w.Simplified.Length == 1 && w.Simplified[0] == c)
                {
                    string def = w.Definitions.Replace('/', '\n');
                    result += $"{c}: {def}";
                }
            }

            return result;
        }*/
    }
}
