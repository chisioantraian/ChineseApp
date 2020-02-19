using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

using ChineseAppWPF.Models;
using ChineseAppWPF.UiFactory;
using ChineseAppWPF.Logic;
using System.IO;
using System;
using System.Linq;

namespace ChineseAppWPF.Controllers
{
    /*public class SentenceBreakdown
    {
        public string Sentence { get; set; }
        public List<(string, string)> Breakdown { get; set; }
    }*/

    public class Breakdown
    {
        public string Part { get; set; }
        public string Description { get; set; }
    }

    public class Sentence
    {
        public string Text { get; set; }
        public List<Breakdown> Correct { get; set; } = null;
        public List<Breakdown> NoAlgorithm { get; set; } = null;
    }

    public static partial class Controller
    {
        private static MainWindow mainWindow;
        private static Dictionary<string,DetailedWord> allDetailedWords;
        private const string testsPath = @"C:\Users\chisi\Desktop\work\ChineseApp\WPF_Program\Data\testSentences.utf8";

        //private static List<SentenceBreakdown> correctList = new List<SentenceBreakdown>();
        //private static List<List<string>> noAlgList = new List<List<string>>();
        //private static List<string> sentenceExamples = new List<string>();
        internal static List<Sentence> sentences = new List<Sentence>(); // ? 
        
        private static int correctSentencesByNoAlg = 0;
        private static int wrongNumberOfWords = 0;
        private static int wrongDecompositionFound = 0;

        public static void SetWindow(MainWindow window)
        {
            mainWindow = window;
            allDetailedWords = ChineseService.GetAllDetailedWords();
        }

        public static void ShowResult()
        {
            if (string.IsNullOrEmpty(mainWindow.SearchBar.Text))
            {
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

        private static List<Breakdown> GetTupleListFrom(string line)
        {
            string[] simplPosList = line.Split('\t');
            List<Breakdown> result = new List<Breakdown>();
            foreach (string sp in simplPosList)
            {
                string[] token = sp.Split("_");
                result.Add(new Breakdown { Part = token[0], Description = token[1] });
            }
            return result;
        }

        public static List<Breakdown> GetNoAlgBreakdown(string sentence)
        {
            List<string> wordParts = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            List<Breakdown> result = new List<Breakdown>();
            foreach (string part in wordParts)
            {
                if (allDetailedWords.ContainsKey(part))
                    result.Add(new Breakdown { Part = part, Description = allDetailedWords[part].DominantPos });
                else
                    result.Add(new Breakdown { Part = part, Description = "-" });
            }
            return result;
        }

        private static Sentence GetSentenceBreakdownFromLine(string line)
        {
            string[] token = line.Split("\t", 2);
            string sentence = token[0];
            string breakdownLine = token[1];
            List<Breakdown> correctBreakdown = GetTupleListFrom(breakdownLine);
            List<Breakdown> noAlgBreakdown = GetNoAlgBreakdown(sentence);

            return new Sentence
            {
                Text = sentence,
                Correct = correctBreakdown,
                NoAlgorithm = noAlgBreakdown
            };
        }

        internal static void InitializeStatistics()
        {
            //todo maybe use streams. 
            /*using (var sr = new StreamReader(testsPath))
            {
                while (!sr.EndOfStream)
                {
                    string sentence = sr.ReadLine();
                    string sentenceBreakdownLine = sr.ReadLine();

                    List<(string, string)> correctBreakdown = GetTupleListFrom(sentenceBreakdownLine);
                    List<string> noAlgBreakdown = ChineseService.GetSimplifiedWordsFromSentence(sentence);

                    correctList.Add(new SentenceBreakdown
                    {
                        Sentence = sentence,
                        Breakdown = correctBreakdown
                    });

                    noAlgList.Add(noAlgBreakdown);
                }
            }*/
            sentences = File.ReadAllLines(testsPath)
                            .Select(GetSentenceBreakdownFromLine)
                            .ToList();
            
            foreach (Sentence sentence in sentences)
            {
                if (sentence.Correct.Count != sentence.NoAlgorithm.Count)
                {
                    wrongNumberOfWords++;
                    continue;
                }
                int correctWordsFoundForThisSentence = 0;
                for (int i = 0; i < sentence.Correct.Count; i++)
                {
                    if (sentence.Correct[i].Part != sentence.NoAlgorithm[i].Part)
                    {
                        wrongDecompositionFound++;
                        break;
                    }
                    if (sentence.Correct[i].Description == sentence.NoAlgorithm[i].Description ||
                            ChineseService.IsPunctuation(sentence.NoAlgorithm[i].Part))
                    { 
                        correctWordsFoundForThisSentence++; 
                    }
                }
                if (correctWordsFoundForThisSentence == sentence.Correct.Count)
                    correctSentencesByNoAlg++;
            }
            ModifyStatisticsBox();
            //
            //
            //
            /*for (int i = 0; i < correctList.Count; i++)
            {
                if (correctList[i].Breakdown.Count != noAlgList[i].Count)
                {
                    wrongNumberOfWords++;
                    sentenceExamples.Add(correctList[i].Sentence);
                    break;
                }
                int correctWordsFoundForThisSentence = 0;
                for (int j = 0; j < correctList[i].Breakdown.Count; j++)
                {
                    if (correctList[i].Breakdown[j].Item1 != noAlgList[i][j])
                    {
                        wrongDecompositionFound++;
                        sentenceExamples.Add(correctList[i].Sentence);
                        break;
                    }
                    if (allDetailedWords.ContainsKey(noAlgList[i][j]))
                    {
                        if (allDetailedWords[noAlgList[i][j]].DominantPos == correctList[i].Breakdown[j].Item2)
                        {
                            correctWordsFoundForThisSentence++;
                        }
                    }
                    if (ChineseService.IsPunctuation(correctList[i].Breakdown[j].Item1))
                    {
                        correctWordsFoundForThisSentence++;
                    }

                }
                if (correctWordsFoundForThisSentence == correctList[i].Breakdown.Count)
                    correctSentencesByNoAlg++;
            }
            ModifyStatisticsBox();*/
        }

        internal static List<(string, string, string)> GetDescription(List<string> simplifiedList)
        {
            List<(string, string, string)> result = new List<(string, string, string)>();
            foreach (string simp in simplifiedList)
            {
                if (allDetailedWords.ContainsKey(simp))
                {
                    result.Add((simp, allDetailedWords[simp].DominantPos, allDetailedWords[simp].AllPos + "\n" + allDetailedWords[simp].AllPosFreq));
                }
                else
                {
                    string punctuation = simp switch
                    {
                        "?" => "engQmark",
                        "." => "engDot",
                        "," => "engComma",
                        "？" => "chnQmark",
                        "。" => "chnDot",
                        "，" => "chnComma", //more to come
                        _ => "other",
                    };
                    result.Add((simp, punctuation, ""));
                }
            }

            ///
            // Here algorithm
            ///

            return result;
        }

        // Split a sentence into words, show these words and analyze the sentence
        internal static void ShowGrammarAnalysis()
        {
            string sentence = mainWindow.TestSentenceInputBox.Text;
            List<string> simplifiedList = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            ChineseService.GetAllWordsFrom(simplifiedList).UpdateShownWords();
            string myText = sentence + "\t";

            mainWindow.MiddleWordBox.Children.Clear();
            List<(string, string, string)> breadownDescription = GetDescription(simplifiedList);
            foreach ((string,string,string) bd in breadownDescription)
            {
                (SolidColorBrush, string) posTuple = GetPosInfo(bd.Item2);
                var wordBorder = UiFactory.BoxFactory.CreateWordBox(posTuple, bd.Item1, bd.Item3);
                mainWindow.MiddleWordBox.Children.Add(wordBorder);

                myText += $"{bd.Item1}_{bd.Item2}\t";
            }

            myText = myText.Remove(myText.Length - 1);
            //myText += "\n";
            mainWindow.TestSentenceResultBox.Text = myText;  
        }

        internal static void AddSentenceBreakdownToTests()
        {
            //string resultText = mainWindow.TestSentenceResultBox.Text;
            //string sentence = resultText.Split('\n')[0];
            /*List<(string, string)> breakdown = GetTupleListFrom(resultText.Split('\n')[1]);
            correctList.Add(new SentenceBreakdown
            {
                Sentence = sentence,
                Breakdown = breakdown
            });*/
            //List<Breakdown> correctBreakdown = Get
            //List<string> simplifiedList = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            Sentence sentence = GetSentenceBreakdownFromLine(mainWindow.TestSentenceResultBox.Text);

            if (sentence.Correct.Count != sentence.NoAlgorithm.Count)
            {
                wrongNumberOfWords++;
            }
            else
            {
                int correctWordsFoundForThisSentence = 0;
                for (int i = 0; i < sentence.Correct.Count; i++)
                {
                    if (sentence.Correct[i].Part != sentence.NoAlgorithm[i].Part)
                    {
                        wrongDecompositionFound++;
                        break;
                    }
                    if (sentence.Correct[i].Description == sentence.NoAlgorithm[i].Description || 
                        ChineseService.IsPunctuation(sentence.NoAlgorithm[i].Part))
                    {
                        correctWordsFoundForThisSentence++;
                    }
                }
                if (correctWordsFoundForThisSentence == sentence.Correct.Count)
                    correctSentencesByNoAlg++;
            }
            sentences.Add(sentence);
            ModifyStatisticsBox();
            /*if (breakdown.Count != simplifiedList.Count)
            {
                Console.WriteLine($"sentence = {sentence}");
                Console.WriteLine("%%%");
                Console.WriteLine($"{breakdown.Count} - {simplifiedList.Count} sga");
                foreach (string s in simplifiedList)
                    Console.Write($"{s} : ");
                Console.WriteLine(",.,.");
                wrongNumberOfWords++;
            }
            else
            {


                for (int j = 0; j < breakdown.Count; j++)
                {
                    if (breakdown[j].Item1 != simplifiedList[j])
                    {
                        Console.WriteLine($"wrong decomposition found {breakdown[j].Item1}");
                        wrongDecompositionFound++;
                        break;
                    }
                    if (allDetailedWords.ContainsKey(simplifiedList[j]))
                    {
                        if (allDetailedWords[simplifiedList[j]].DominantPos == breakdown[j].Item2)
                        {
                            correctWordsFoundForThisSentence++;
                        }
                    }
                    if (ChineseService.IsPunctuation(breakdown[j].Item1))
                    {
                        correctWordsFoundForThisSentence++;
                    }
                }
                if (correctWordsFoundForThisSentence == breakdown.Count)
                    correctSentencesByNoAlg++;
            }
            ModifyStatisticsBox();*/
        }

        internal static void ModifyStatisticsBox()
        {
            string stats = "Statistics: \n\n";
            stats += $"{sentences.Count} total sentences\n\n";
            stats += $"{wrongNumberOfWords} - Sentences with wrong number of words detected\n\n";
            stats += $"{wrongDecompositionFound} - Sentences with wrong words detected\n\n";
            stats += $"{sentences.Count - correctSentencesByNoAlg} - Sentences with wrong POS assigned\n\n";
            stats += $"{correctSentencesByNoAlg} - Correct sentences with no algorithm (by default)\n\n";
            stats += $"{((double)correctSentencesByNoAlg / sentences.Count) * 100}% - precision by default\n\n";
            stats += "...algorithm to be done\n\n";
            stats += "just simple sentences (no punctuations/phrases/etc\n\n";
            mainWindow.AnalysisStatisticsBox.Text = stats;
        }



        internal static void SaveTestSentences()
        {
            using (StreamWriter sw = new StreamWriter(testsPath))
            {

                //foreach (SentenceBreakdown sent in correctList.OrderBy(sb => sb.Sentence.Length))//.Distinct((SentenceBreakdown sb1, SentenceBreakdown sb2) => sb1.Equals(sb2)))
                foreach (Sentence sentence in sentences)
                {
                    sw.Write(sentence.Text + "\t");
                    string breakdownText = "";
                    foreach (var breakdown in sentence.Correct)
                    {
                        breakdownText += $"{breakdown.Part}_{breakdown.Description}\t";
                    }
                    breakdownText = breakdownText.Remove(breakdownText.Length - 1);
                    sw.WriteLine(breakdownText);
                }
            }
        }


        internal static void UpdateStatistics()
        {

        }

        // From a word(DetailedWord) pos tag, get its full pos name , and also return a color which is unique to it
        private static (SolidColorBrush, string) GetPosInfo(string description)//(DetailedWord detailedWord)
        {
            return description switch //.DominantPos switch
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
                "engQmark" => (Brushes.BlueViolet, "english qMark"),
                "engDot" => (Brushes.BlueViolet, "english dot"),
                "engComma" => (Brushes.BlueViolet, "english comma"),
                "chnQmark" => (Brushes.BlueViolet, "chinese qMark"),
                "chnDot" => (Brushes.BlueViolet, "chinese dot"),
                "chnComma" => (Brushes.BlueViolet, "chinese comma"),
                _ => (Brushes.Gray, "_")
            };


        }
    }
}
