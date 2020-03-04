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
    public static partial class Controller
    {
        private static MainWindow mainWindow;
        private static Dictionary<string,DetailedWord> allDetailedWords;
        private const string testsPath = @"C:\Users\chisi\Desktop\work\ChineseApp\WPF_Program\Data\testSentences.utf8";
        private static List<Sentence> sentences = new List<Sentence>();
        private static List<Sentence> wrongSentences = new List<Sentence>();

        private static int correctSentencesByNoAlg;
        private static int wrongNumberOfWords;
        private static int wrongDecompositionFound;

        private static int correctSentencesByAlg;
        private static int wrongNumberOfWordsAfterAlg;
        private static int wrongDecompositionFoundAfterAlg;

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

        private static void ShowComposeResult()
        {
            Decomposition.GetCharactersWithComponent(mainWindow.SearchBar.Text).UpdateShownWords();
        }

        private static Breakdown BreakdownFromString(string simplPOS)
        {
            string[] token = simplPOS.Split("_");
            return new Breakdown { Part = token[0], Description = token[1] };
        }

        private static List<Breakdown> GetTupleListFrom(string line) =>
            line.Split('\t')
                .Select(BreakdownFromString)
                .ToList();

        public static IEnumerable<Breakdown> GetNoAlgBreakdown(string sentence)
        {
            IEnumerable<string> wordParts = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            foreach (string part in wordParts)
            {
                if (allDetailedWords.ContainsKey(part))
                    yield return new Breakdown { Part = part, Description = allDetailedWords[part].DominantPos };
                else
                    yield return new Breakdown { Part = part, Description = "-" };
            }
        }

        //split into more functions
        public static bool CanApply(Rule rule, List<Breakdown> bd, int i) =>
            (bd[i].Part == rule.Current &&
            bd[i].Description == rule.Tag1 &&
            rule.Cond == "nextTag" &&
            bd[i + 1].Description == rule.Tag3) ||

            (bd[i].Part == rule.Current &&
            bd[i].Description == rule.Tag1 &&
            rule.Cond == "prevTag" &&
            bd[i - 1].Description == rule.Tag3) ||

            (bd[i].Part == rule.Current &&
            bd[i].Description == rule.Tag1 &&
            rule.Cond == "nextWord" && // modify Rule class
            bd[i + 1].Part == rule.Tag3); 

        public static List<Breakdown> GetAlgBreakdown(List<Breakdown> noAlg)
        {
            List<Breakdown> algList = new List<Breakdown>();
            foreach (Breakdown bd in noAlg)
            {
                algList.Add(new Breakdown { Part = bd.Part, Description = bd.Description } );
            }
            
            for (int i = 0; i < algList.Count; i++)
            {
                foreach (Rule rule in rules)
                {
                    if (CanApply(rule, algList, i))
                    {
                        algList[i].Description = rule.Tag2;
                    }
                }
            }

            return algList;
        }

        private static Sentence GetSentenceBreakdownFromLine(string line)
        {
            string[] token = line.Split("\t", 2);
            string sentence = token[0];
            string breakdownLine = token[1];

            List<Breakdown> correctBreakdown = GetTupleListFrom(breakdownLine);
            List<Breakdown> noAlgBreakdown = GetNoAlgBreakdown(sentence).ToList();
            List<Breakdown> algBreakdown = GetAlgBreakdown(noAlgBreakdown);

            return new Sentence
            {
                Text = sentence,
                Correct = correctBreakdown,
                NoAlgorithm = noAlgBreakdown,
                Algorithm = algBreakdown
            };
        }


        private static void UpdateStatistics(Sentence sentence)
        {
            if (sentence.NoAlgorithm.Count != sentence.Correct.Count)
            {
                wrongNumberOfWords++;
                wrongSentences.Add(sentence);
            }
            else
            {
                int correctWordsFoundForThisSentence = 0;
                for (int i = 0; i < sentence.NoAlgorithm.Count; i++)
                {
                    if (sentence.NoAlgorithm[i].Part != sentence.Correct[i].Part)
                    {
                        wrongDecompositionFound++;
                        wrongSentences.Add(sentence);
                        break;
                    }
                    if (sentence.NoAlgorithm[i].Description == sentence.Correct[i].Description ||
                        ChineseService.IsPunctuation(sentence.NoAlgorithm[i].Part))
                    {
                        correctWordsFoundForThisSentence++;
                    }
                }
                if (correctWordsFoundForThisSentence == sentence.Correct.Count)
                    correctSentencesByNoAlg++;
                else
                    wrongSentences.Add(sentence);
            }

            //
            // split later
            //
            if (sentence.Algorithm.Count != sentence.Correct.Count)
            {
                wrongNumberOfWordsAfterAlg++;
            }
            else
            {
                int correctWordsFoundForThisSentence = 0;
                for (int i = 0; i < sentence.Algorithm.Count; i++)
                {
                    if (sentence.Algorithm[i].Part != sentence.Correct[i].Part)
                    {
                        wrongDecompositionFoundAfterAlg++;
                        break;
                    }
                    if (sentence.Algorithm[i].Description == sentence.Correct[i].Description ||
                        ChineseService.IsPunctuation(sentence.Algorithm[i].Part))
                    {
                        correctWordsFoundForThisSentence++;
                    }
                }
                if (correctWordsFoundForThisSentence == sentence.Correct.Count)
                    correctSentencesByAlg++;
            }
        }

        internal static void InitializeStatistics()
        {
            sentences = File.ReadAllLines(testsPath)
                            .Select(GetSentenceBreakdownFromLine)
                            .ToList();
            sentences.ForEach(UpdateStatistics);
            ModifyStatisticsBox();
        }

        internal static IEnumerable<(string, string, string)> GetDescription(IEnumerable<string> simplifiedList)
        {
            foreach (string simp in simplifiedList)
            {
                if (allDetailedWords.ContainsKey(simp))
                {
                    yield return (simp, allDetailedWords[simp].DominantPos, allDetailedWords[simp].AllPos + "\n" + allDetailedWords[simp].AllPosFreq);
                }
                else
                {
                    string punctuation = simp switch
                    {
                        "?" => "engQmark",
                        "." => "engDot",
                        "," => "engComma",
                        "!" => "engExcl",
                        "？" => "chnQmark",
                        "。" => "chnDot",
                        "，" => "chnComma",
                        "！" => "chnExcl",
                        _ => "other",
                    };
                    yield return (simp, punctuation, "");
                }
            }
        }

        // Split a sentence into words, show these words and analyze the sentence
        internal static void ShowGrammarAnalysis()
        {
            string sentence = mainWindow.TestSentenceInputBox.Text;
            IEnumerable<string> simplifiedList = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            ChineseService.GetAllWordsFrom(simplifiedList).UpdateShownWords();
            string myText = sentence + "\t";

            mainWindow.MiddleWordBox.Children.Clear();
            IEnumerable<(string, string, string)> breadownDescription = GetDescription(simplifiedList);
            foreach ((string,string,string) bd in breadownDescription)
            {
                (SolidColorBrush, string) posTuple = GetPosInfo(bd.Item2);
                var wordBorder = UiFactory.BoxFactory.CreateWordBox(posTuple, bd.Item1, bd.Item3);
                mainWindow.MiddleWordBox.Children.Add(wordBorder);

                myText += $"{bd.Item1}_{bd.Item2}\t";
            }

            myText = myText.Remove(myText.Length - 1);
            mainWindow.TestSentenceResultBox.Text = myText;  
        }

        internal static void AddSentenceBreakdownToTests()
        {
            Sentence sentence = GetSentenceBreakdownFromLine(mainWindow.TestSentenceResultBox.Text);
            sentences.Add(sentence);
            UpdateStatistics(sentence);
            ModifyStatisticsBox();
        }

        internal static void ModifyStatisticsBox()
        {
            string stats = "Statistics: \n\n";
            stats += $"{sentences.Count} total sentences\n\n";

            stats += $"{wrongNumberOfWords} - Sentences with wrong number of words detected\n";
            stats += $"{wrongNumberOfWordsAfterAlg} --//-- After Algorithm\n\n";

            stats += $"{wrongDecompositionFound} - Sentences with wrong words detected\n";
            stats += $"{wrongDecompositionFoundAfterAlg} --//-- After Algorithm\n\n";

            stats += $"{sentences.Count - correctSentencesByNoAlg - wrongNumberOfWords} - Sentences with wrong POS assigned\n";
            stats += $"{sentences.Count - correctSentencesByAlg - wrongNumberOfWordsAfterAlg} --//-- After Algorithm\n\n";

            stats += $"{correctSentencesByNoAlg} - Correct sentences with no algorithm (by default)\n";
            stats += $"{correctSentencesByAlg} --//-- After Algorithm\n\n";
            
            stats += $"{((double)correctSentencesByNoAlg / sentences.Count) * 100}% - precision by default\n";
            stats += $"{((double)correctSentencesByAlg / sentences.Count) * 100}% - precision by using algorithm\n";

            mainWindow.AnalysisStatisticsBox.Text = stats;
        }

        internal static void SaveTestSentences()
        {
            using StreamWriter sw = new StreamWriter(testsPath);
            IEnumerable<Sentence> listResult =
                sentences.GroupBy(s => s.Text)
                         .Select(g => g.First())
                         .OrderBy(s => s.Text.Length);
            
            foreach (Sentence sentence in listResult)
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
            Console.WriteLine("Saved sentences to file");
        }

        private static (SolidColorBrush, string) GetPosInfo(string description)
        {
            return description switch
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
                "engExcl" => (Brushes.BlueViolet, "english exclamation"),
                "chnQmark" => (Brushes.BlueViolet, "chinese qMark"),
                "chnDot" => (Brushes.BlueViolet, "chinese dot"),
                "chnComma" => (Brushes.BlueViolet, "chinese comma"),
                "chnExcl" => (Brushes.BlueViolet, "chinese exclamation"),
                _ => (Brushes.Gray, "_")
            };

        }
    }
}
