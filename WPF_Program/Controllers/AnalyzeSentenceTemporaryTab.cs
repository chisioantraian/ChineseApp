using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using ChineseAppWPF.UiFactory;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static int correctSentencesByNoAlgorithm;
        private static int wrongNumberOfWords;
        private static int wrongDecompositionFound;

        private const string testsPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\testSentences.utf8";
        private static List<Sentence> sentences = new List<Sentence>();
        private static readonly List<Sentence> wrongSentences = new List<Sentence>();

        internal static void AnalyseSentence_TestTab()
        {
            string sentence = mainWindow.TestSentenceInputBox.Text;
            StringBuilder annotatedSentence = new StringBuilder(sentence + "\t");
            List<string> simplifiedList = ChineseService.GetSimplifiedWordsFromSentence(sentence).ToList();

            mainWindow.TestSentenceAnalysisBox.Children.Clear();
            foreach ((string, string, string) bd in GetAllFoundWordsWithAnnotation(simplifiedList))
            {
                var wordBox = BoxFactory.CreateWordBox(bd);
                mainWindow.TestSentenceAnalysisBox.Children.Add(wordBox);

                annotatedSentence.Append($"{bd.Item1}_{bd.Item2}\t");
            }
            annotatedSentence.Length--;
            mainWindow.TestSentenceResultBox.Text = annotatedSentence.ToString();

            ChineseService.GetAllWordsFrom(simplifiedList).UpdateShownWords(false);
        }

        private static IEnumerable<(string, string, string)> GetAllFoundWordsWithAnnotation(IEnumerable<string> simplifiedList)
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

        private static Sentence GetSentenceBreakdownFromLine(string line)
        {
            string[] token = line.Split("\t", 2);
            string sentence = token[0];
            string breakdownLine = token[1];

            List<Breakdown> correctBreakdown = BreakdownService.GetTupleListFrom(breakdownLine);
            List<Breakdown> noAlgBreakdown = GetNoAlgorithmBreakdown(sentence).ToList();
            List<Breakdown> algBreakdown = GetAlgorithmBreakdown(noAlgBreakdown);

            return new Sentence
            {
                Text = sentence,
                Correct = correctBreakdown,
                NoAlgorithm = noAlgBreakdown,
                Algorithm = algBreakdown
            };
        }

        internal static void AddSentenceBreakdownToTests()
        {
            Sentence sentence = GetSentenceBreakdownFromLine(mainWindow.TestSentenceResultBox.Text);
            sentences.Add(sentence);
            UpdateStatistics(sentence);
            ModifyStatisticsBox();
        }

        internal static void InitializeStatistics()
        {
            sentences = File.ReadAllLines(testsPath)
                            .Select(GetSentenceBreakdownFromLine)
                            .ToList();
            sentences.ForEach(UpdateStatistics);
            ModifyStatisticsBox();
        }

        private static int CorrectWordsFoundForSentence(Sentence sentence, List<Breakdown> brList)
        {
            int correctWordsFoundForThisSentence = 0;

            for (int i = 0; i < brList.Count; i++)
            {
                if (brList[i].FoundWord != sentence.Correct[i].FoundWord)
                {
                    wrongDecompositionFound++;
                    break;
                }
                if (brList[i].Annotation == sentence.Correct[i].Annotation ||
                    ChineseService.IsPunctuation(brList[i].FoundWord))
                {
                    correctWordsFoundForThisSentence++;
                }
            }
            return correctWordsFoundForThisSentence;
        }

        private static void UpdateStatistics(Sentence sentence)
        {
            if (sentence.NoAlgorithm.Count != sentence.Correct.Count)
            {
                wrongNumberOfWords++;
            }
            else if (CorrectWordsFoundForSentence(sentence, sentence.NoAlgorithm) == sentence.Correct.Count)
            {
                correctSentencesByNoAlgorithm++;
            }

            if (sentence.Algorithm.Count != sentence.Correct.Count)
            {
                wrongNumberOfWordsAfterAlg++;
                wrongSentences.Add(sentence);
            }
            else if (CorrectWordsFoundForSentence(sentence, sentence.Algorithm) == sentence.Correct.Count)
            {
                correctSentencesByAlgorithm++;
            }
            else
            {
                wrongSentences.Add(sentence);
            }
        }

        private static void ModifyStatisticsBox()
        {
            string stats = "Statistics: \n\n";
            stats += $"{sentences.Count} total sentences\n\n";

            stats += $"{wrongNumberOfWords} - Sentences with wrong number of words detected\n";
            stats += $"{wrongNumberOfWordsAfterAlg} --//-- After Algorithm\n\n";

            stats += $"{wrongDecompositionFound} - Sentences with wrong words detected\n";
            stats += $"{wrongDecompositionFoundAfterAlg} --//-- After Algorithm\n\n";

            stats += $"{sentences.Count - correctSentencesByNoAlgorithm - wrongNumberOfWords} - Sentences with wrong POS assigned\n";
            stats += $"{sentences.Count - correctSentencesByAlgorithm - wrongNumberOfWordsAfterAlg} --//-- After Algorithm\n\n";

            stats += $"{correctSentencesByNoAlgorithm} - Correct sentences with no algorithm (by default)\n";
            stats += $"{correctSentencesByAlgorithm} --//-- After Algorithm\n\n";

            stats += $"{((double)correctSentencesByNoAlgorithm / sentences.Count) * 100}% - precision by default\n";
            stats += $"{((double)correctSentencesByAlgorithm / sentences.Count) * 100}% - precision by using algorithm\n";

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
                StringBuilder breakdownText = new StringBuilder("");
                foreach (var breakdown in sentence.Correct)
                {
                    breakdownText.Append($"{breakdown.FoundWord}_{breakdown.Annotation}\t");
                }
                breakdownText.Length--;
                sw.WriteLine(breakdownText.ToString());
            }
        }
    }
}
