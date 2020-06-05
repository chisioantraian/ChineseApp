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

        private static Sentence ComputeSentenceBreakdown(string sentenceText)
        {
            List<Breakdown> noAlgorithmBreakdown = GetNoAlgorithmBreakdown(sentenceText).ToList();
            List<Breakdown> algorithmBreakdown = GetAlgorithmBreakdown(noAlgorithmBreakdown);

            return new Sentence
            {
                Text = sentenceText,
                NoAlgorithm = noAlgorithmBreakdown,
                Algorithm = algorithmBreakdown
            };
        }

        private static List<Breakdown> GetNoAlgorithmBreakdown(string sentence)
        {
            /*List<Breakdown> noAlgList = new List<Breakdown>();
            List<string> wordParts = ChineseService.GetSimplifiedWordsFromSentence(sentence).ToList();
            foreach (string part in wordParts)
            {
                if (allDetailedWords.ContainsKey(part))
                    noAlgList.Add(new Breakdown { FoundWord = part, Annotation = allDetailedWords[part].DominantPos });
                else
                    noAlgList.Add(new Breakdown { FoundWord = part, Annotation = part });
            }
            return noAlgList;*/
            return ChineseService.GetSimplifiedWordsFromSentence(sentence)
                                    .Select(BreakdownFromPart)
                                    .ToList();
        }

        private static Breakdown BreakdownFromPart(string part)
        {
            if (allDetailedWords.ContainsKey(part))
                return new Breakdown { FoundWord = part, Annotation = allDetailedWords[part].DominantPos };
            else
                return new Breakdown { FoundWord = part, Annotation = part };
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
