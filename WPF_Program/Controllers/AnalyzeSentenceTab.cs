using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static int correctSentencesByAlg;
        private static int wrongNumberOfWordsAfterAlg;
        private static int wrongDecompositionFoundAfterAlg;

        internal static void AnalyseSentence()
        {
            string sentenceText = mainWindow.SentenceAnalysisInputBox.Text;
            Sentence st = ComputeSentenceBreakdown(sentenceText);

            mainWindow.SentenceAnalysisBox.Children.Clear();
            foreach (Breakdown b in st.Algorithm)
            {
                (SolidColorBrush, string) posTuple = PosInformation.GetPosInfo(b.Description);
                var wordBorder = UiFactory.BoxFactory.CreateAnalysisWordBox(posTuple, b.Part, mainWindow);
                mainWindow.SentenceAnalysisBox.Children.Add(wordBorder);
            }
        }

        private static Sentence ComputeSentenceBreakdown(string sentence)
        {
            List<Breakdown> noAlgBreakdown = GetNoAlgBreakdown(sentence).ToList();
            List<Breakdown> algBreakdown = GetAlgBreakdown(noAlgBreakdown);

            return new Sentence
            {
                Text = sentence,
                Correct = null,
                NoAlgorithm = noAlgBreakdown,
                Algorithm = algBreakdown
            };
        }
    }
}
