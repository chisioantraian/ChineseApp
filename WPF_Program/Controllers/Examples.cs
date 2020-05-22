using System;
using System.Windows.Controls;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        public static void InitializeSentenceExamples()
        {
            foreach (Sentence sentence in wrongSentences)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = sentence.Text,
                };
                item.MouseLeftButtonUp += (s, e) =>
                {
                    mainWindow.TestSentenceInputBox.Text = sentence.Text;
                    AnalyseSentence_TestTab();
                };
                mainWindow.ExamplesList.Items.Add(item);
            }
        }
    }
}
