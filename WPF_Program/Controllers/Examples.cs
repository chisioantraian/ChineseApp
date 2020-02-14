using System.Windows.Controls;
using ChineseAppWPF.Logic;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        public static void InitializeSentenceExamples()
        {
            foreach (string sentence in SentenceExamples.Examples())
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
