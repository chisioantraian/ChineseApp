using System.Windows.Controls;
using System.Linq;
using ChineseAppWPF.Logic;
using System.Collections.Generic;
using ChineseAppWPF.Models;
using System;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        public static void InitializeSentenceExamples()
        {
            //foreach (string sentence in SentenceExamples.Examples())
            foreach (Sentence sentence in wrongSentences)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = sentence.Text,
                };
                item.MouseLeftButtonUp += (s, e) =>
                {
                    mainWindow.TestSentenceInputBox.Text = sentence.Text;
                    foreach (Breakdown sen in sentence.Correct)
                    {
                        Console.WriteLine(sen.Part + " " + sen.Description);
                    }
                    ShowGrammarAnalysis();
                };
                mainWindow.ExamplesList.Items.Add(item);
            }
        }
    }
}
