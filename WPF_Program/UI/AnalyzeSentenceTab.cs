using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using ChineseAppWPF.UiFactory;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAppWPF.UI
{
    public static partial class UiLayer
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
            List<Breakdown> algList = CopyFromNoAlgorithm(noAlg);

            for (int i = 0; i < algList.Count; i++)
            {
                foreach (Rule rule in RuleService.GetRules())
                {
                    rule.ApplyRule(algList, i);
                }
            }

            return algList;
        }

        private static List<Breakdown> CopyFromNoAlgorithm(List<Breakdown> noAlg)
        {
            return noAlg.Select(bd => new Breakdown { FoundWord = bd.FoundWord, Annotation = bd.Annotation })
                        .ToList();
        }
    }
}
