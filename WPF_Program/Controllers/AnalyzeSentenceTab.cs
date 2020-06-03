using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using ChineseAppWPF.UiFactory;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static int correctSentencesByAlgorithm;
        private static int wrongNumberOfWordsAfterAlg;
        private static int wrongDecompositionFoundAfterAlg;

        internal static void AnalyseSentence()
        {
            mainWindow.SentenceAnalysisBox.Children.Clear();
            string sentenceText = mainWindow.SentenceAnalysisInputBox.Text;
            Sentence sentence = ComputeSentenceBreakdown(sentenceText);

            foreach (Breakdown breakdown in sentence.Algorithm)
            {
                var wordBox = BoxFactory.CreateAnalysisWordBox(breakdown);
                mainWindow.SentenceAnalysisBox.Children.Add(wordBox);
            }
        }

        private static Sentence ComputeSentenceBreakdown(string sentence)
        {
            List<Breakdown> noAlgorithmBreakdown = GetNoAlgorithmBreakdown(sentence).ToList();
            List<Breakdown> algorithmBreakdown = GetAlgorithmBreakdown(noAlgorithmBreakdown);

            return new Sentence
            {
                Text = sentence,
                NoAlgorithm = noAlgorithmBreakdown,
                Algorithm = algorithmBreakdown
            };
        }

        private static IEnumerable<Breakdown> GetNoAlgorithmBreakdown(string sentence)
        {
            List<string> wordParts = ChineseService.GetSimplifiedWordsFromSentence(sentence).ToList();
            foreach (string part in wordParts)
            {
                if (allDetailedWords.ContainsKey(part))
                    yield return new Breakdown { FoundWord = part, Annotation = allDetailedWords[part].DominantPos };
                else
                    yield return new Breakdown { FoundWord = part, Annotation = part };
            }
        }

        internal static List<Breakdown> GetAlgorithmBreakdown(List<Breakdown> noAlg)
        {
            List<Breakdown> algList = new List<Breakdown>();
            foreach (Breakdown bd in noAlg)
            {
                algList.Add(new Breakdown { FoundWord = bd.FoundWord, Annotation = bd.Annotation });
            }

            for (int i = 0; i < algList.Count; i++)
            {
                foreach (Rule rule in rules)
                {
                    BreakdownService.ApplyRule(rule, algList, i);
                }
            }

            return algList;
        }
    }
}
