using System.Windows.Controls;
using WPF_program.Logic;

namespace WPF_program.Controllers
{
    public static partial class Controller
    {
        // Add some sentences to the app, which will be used as examples
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
