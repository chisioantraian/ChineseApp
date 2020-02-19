using System.Windows.Controls;
using System.Linq;
using ChineseAppWPF.Logic;
using System.Collections.Generic;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        public static void InitializeSentenceExamples()
        {
            //foreach (string sentence in SentenceExamples.Examples())
            foreach (string sentence in sentences.Select(sent => sent.Text))
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = sentence,
                };
                item.MouseLeftButtonUp += (s, e) =>
                {
                    mainWindow.TestSentenceInputBox.Text = sentence;
                    ShowGrammarAnalysis();
                };
                mainWindow.ExamplesList.Items.Add(item);
            }
        }
    }
}
