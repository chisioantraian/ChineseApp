using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;

using WpfApp2;
using WPF_program.Models;
using WPF_program.Ui_Factory;
using WPF_program.Logic;

namespace WPF_program.Controllers
{
    public static partial class Controller
    {
        private static MainWindow mainWindow;
        private static List<Word> allWords;
        private static Dictionary<string,DetailedWord> allDetailedWords;
        private static Dictionary<char, List<char>> dict;

        public static void setWindow(MainWindow window)
        {
            mainWindow = window;
            allWords = ChineseService.GetAllWords().ToList();
            allDetailedWords = ChineseService.GetAllDetailedWords();
            dict = Decomposition.GetCharacterDecomposition();
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
            string decompositionText = Decomposition.DecomposeCharToRadicals(characterToBeDecomposed);
            mainWindow.DecompositionBlock.Text = $"{decompositionText} ";
        }

        // From a single character, show all other chinese characters which contain it as a component
        private static void ShowComposeResult()
        {
            List<Word> filteredWords = Decomposition.GetCharactersWithComponent(mainWindow.SearchBar.Text);
            UpdateShownWords(filteredWords);
        }

        static int i = 0;
        // Split a sentence into words, show these words and analyze the sentence
        private static void ShowDecomposed(string sentence)
        {
            Stopwatch stopWatch = new StopWatch();
            stopWatch.Start();
            //List<Word> result = ChineseService.GetWordsFromSentence(sentence);
            List<string> simplifiedList = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            List<Word> wordsList = ChineseService.GetAllWordsFrom(simplifiedList);
            stopWatch.Stop();
            mainWindow.SearchBar.Text = $"ms: {stopWatch.Elapsed.TotalMilliseconds}";
            UpdateShownWords(wordsList);


            mainWindow.MiddleWordBox.Children.Clear();
            foreach (string simp in simplifiedList)
            {
                //DetailedWord? detailedWord = allDetailedWords.Find(dw => dw.Simplified == w.Simplified);
                if (allDetailedWords.ContainsKey(simp))
                {
                    DetailedWord detailedWord = allDetailedWords[simp];
                    (SolidColorBrush, string) posTuple = GetPosInfo(detailedWord);

                    var wordBorder = UiFactory.CreateWordBox(posTuple, simp);
                    mainWindow.MiddleWordBox.Children.Add(wordBorder);
                }
            }
        }

        // From a word(DetailedWord) pos tag, get its full pos name , and also return a color which is unique to it
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

    }

    internal class StopWatch : Stopwatch
    {
    }
}
