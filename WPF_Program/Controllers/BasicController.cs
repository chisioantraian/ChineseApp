using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

using ChineseAppWPF.Models;
using ChineseAppWPF.UiFactory;
using ChineseAppWPF.Logic;
using System.IO;
using System;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static MainWindow mainWindow;
        private static Dictionary<string,DetailedWord> allDetailedWords;
        private const string testsPath = @"C:\Users\chisi\Desktop\work\ChineseApp\WPF_Program\Data\testSentences.utf8";
        public static void SetWindow(MainWindow window)
        {
            mainWindow = window;
            allDetailedWords = ChineseService.GetAllDetailedWords();
        }

        public static void ShowResult()
        {
            if (string.IsNullOrEmpty(mainWindow.SearchBar.Text))
            {
                //UpdateShownWords(new List<Word>());
                return;
            }
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
            Decomposition.GetCharactersWithComponent(mainWindow.SearchBar.Text).UpdateShownWords();
        }

        // Split a sentence into words, show these words and analyze the sentence
        internal static void ShowGrammarAnalysis()
        {
            string sentence = mainWindow.TestSentenceInputBox.Text;
            List<string> simplifiedList = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            ChineseService.GetAllWordsFrom(simplifiedList).UpdateShownWords(); //todo maybe separate functions?
            string myText = sentence + "\n";

            mainWindow.MiddleWordBox.Children.Clear();
            foreach (string simp in simplifiedList)
            {
                if (allDetailedWords.ContainsKey(simp))
                {
                    DetailedWord detailedWord = allDetailedWords[simp];
                    (SolidColorBrush, string) posTuple = GetPosInfo(detailedWord);
                    myText += $"{simp}_{detailedWord.DominantPos}\t";
                    var wordBorder = UiFactory.BoxFactory.CreateWordBox(posTuple, simp);
                    mainWindow.MiddleWordBox.Children.Add(wordBorder);
                }
            }
            myText = myText.Remove(myText.Length - 1);
            myText += "\n";
            mainWindow.TestSentenceResultBox.Text = myText;
        }

        internal static void AddSentenceDecompositionToFile()
        {
            string textToAdd = mainWindow.TestSentenceResultBox.Text;
            using (StreamWriter sw = File.AppendText(testsPath))
            {
                sw.Write(textToAdd);
            }
        }

        private static List<(string, string)> GetTupleListFrom(string line)
        {
            string[] simplPosList = line.Split('\t');
            List<(string, string)> result = new List<(string, string)>();
            foreach (string sp in simplPosList)
            {
                string[] token = sp.Split("_");
                result.Add( (token[0], token[1]) );
            }
            return result;
        }

        internal static void UpdateStatistics()
        {
            var sentences = new List<string>();
            var correctList = new List<List<(string, string)>>();
            var noAlgList = new List<List<string>>();

            using (var sr = new StreamReader(testsPath))
            {
                while (! sr.EndOfStream)
                {
                    string sentence = sr.ReadLine();
                    string fileDecomposition = sr.ReadLine();
                    Console.WriteLine($"{sentence} {fileDecomposition}");
                    List<(string, string)> correctDecomposition = GetTupleListFrom(fileDecomposition);
                    List<string> noAlgDecomposition = ChineseService.GetSimplifiedWordsFromSentence(sentence);

                    sentences.Add(sentence);
                    correctList.Add(correctDecomposition);
                    noAlgList.Add(noAlgDecomposition);
                }
            }

            int correctSentencesByNoAlg = 0;
            int wrongNumberOfWords = 0;
            int wrongDecompositionFound = 0;
            for (int i = 0; i < correctList.Count; i++)
            {
                if (correctList[i].Count != noAlgList[i].Count)
                {
                    wrongNumberOfWords++;
                    break;
                }
                int correctForThisSentence = 0;
                for (int j = 0; j < correctList[i].Count; j++)
                {
                    if (correctList[i][j].Item1 != noAlgList[i][j])
                    {
                        wrongDecompositionFound++;
                        break;
                    }
                    if (allDetailedWords.ContainsKey(noAlgList[i][j]))
                    {
                        if (allDetailedWords[noAlgList[i][j]].DominantPos == correctList[i][j].Item2)
                        {
                            correctForThisSentence++;
                        }
                    }
                }
                if (correctForThisSentence == correctList[i].Count)
                    correctSentencesByNoAlg++;

            }
            string stats = "Statistics: \n\n";
            stats += $"{sentences.Count} total sentences\n\n";
            stats += $"{wrongNumberOfWords} - Sentences with wrong number of words detected\n\n";
            stats += $"{wrongDecompositionFound} - Sentences with wrong words detected\n\n";
            stats += $"{correctSentencesByNoAlg} - Correct sentences with no algorithm (by default)\n\n";
            stats += $"{((double)correctSentencesByNoAlg / sentences.Count) * 100}% - precision by default\n\n";
            stats += "...algorithm to be done\n\n";
            stats += "just simple sentences (no punctuations/phrases/etc\n\n";
            mainWindow.AnalysisStatisticsBox.Text = stats;
        }

        // From a word(DetailedWord) pos tag, get its full pos name , and also return a color which is unique to it
        private static (SolidColorBrush, string) GetPosInfo(DetailedWord detailedWord)
        {
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
}
