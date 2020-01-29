using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace WPF_program.Controllers
{
    public static partial class Controller
    {
        // Add some sentences to the app, which will be used as examples
        public static void InitializeSentenceExamples()
        {
            foreach (string sentence in Chinese.SentenceExamples.getAllSentences())
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = sentence,
                };
                item.MouseLeftButtonUp += (s, e) => ShowDecomposed(sentence);
                mainWindow.ExamplesList.Items.Add(item);
            }
        }


    }
}
