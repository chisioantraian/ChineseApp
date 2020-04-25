using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static int correctSentencesByNoAlg;
        private static int wrongNumberOfWords;
        private static int wrongDecompositionFound;

        // Split a sentence into words, show these words and then, analyze the sentence.
        internal static void AnalyseSentence_TestTab()
        {
            string sentence = mainWindow.TestSentenceInputBox.Text;
            IEnumerable<string> simplifiedList = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            ChineseService.GetAllWordsFrom(simplifiedList, writingState).UpdateShownWords();
            string myText = sentence + "\t";

            mainWindow.MiddleWordBox.Children.Clear();
            IEnumerable<(string, string, string)> breadownDescription = GetDescription(simplifiedList);
            foreach ((string, string, string) bd in breadownDescription)
            {
                (SolidColorBrush, string) posTuple = PosInformation.GetPosInfo(bd.Item2);
                var wordBorder = UiFactory.BoxFactory.CreateWordBox(posTuple, bd.Item1, bd.Item3);
                mainWindow.MiddleWordBox.Children.Add(wordBorder);

                myText += $"{bd.Item1}_{bd.Item2}\t";
            }

            myText = myText.Remove(myText.Length - 1);
            mainWindow.TestSentenceResultBox.Text = myText;
        }

        internal static IEnumerable<(string, string, string)> GetDescription(IEnumerable<string> simplifiedList)
        {
            foreach (string simp in simplifiedList)
            {
                Console.WriteLine(simp);
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

        internal static Sentence GetSentenceBreakdownFromLine(string line)
        {
            string[] token = line.Split("\t", 2);
            string sentence = token[0];
            string breakdownLine = token[1];

            List<Breakdown> correctBreakdown = BreakdownService.GetTupleListFrom(breakdownLine);
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

        private static void UpdateStatistics(Sentence sentence)
        {
            if (sentence.NoAlgorithm.Count != sentence.Correct.Count)
            {
                wrongNumberOfWords++;
            }
            else
            {
                int correctWordsFoundForThisSentence = 0;
                for (int i = 0; i < sentence.NoAlgorithm.Count; i++)
                {
                    if (sentence.NoAlgorithm[i].Part != sentence.Correct[i].Part)
                    {
                        wrongDecompositionFound++;
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
            }

            //
            // split later
            //
            if (sentence.Algorithm.Count != sentence.Correct.Count)
            {
                wrongNumberOfWordsAfterAlg++;
                wrongSentences.Add(sentence);
            }
            else
            {
                int correctWordsFoundForThisSentence = 0;
                for (int i = 0; i < sentence.Algorithm.Count; i++)
                {
                    if (sentence.Algorithm[i].Part != sentence.Correct[i].Part)
                    {
                        wrongDecompositionFoundAfterAlg++;
                        wrongSentences.Add(sentence);
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
                else
                    wrongSentences.Add(sentence);
            }
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

    }
}
