
namespace WpfApp2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Chinese;
    using WPF_program.Controllers;
    using static MyTypes;


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("reorganized code3");
            Controller.setWindow(this);
            Controller.InitializeSentenceExamples();
            Controller.ShowSomeRandomWords();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => Controller.ShowEnglishResult();
        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();
        private void SearchBar_KeyUp(object sender, KeyEventArgs e) => Controller.ShowResult(e.Key);


        // Get a character's decomposition, till radicals/strokes level
        /*private string GetNiceDecomposed(char c)
        {
            string result = String.Empty;

            Queue<char> qu = new Queue<char>();
            qu.Enqueue(c);

            while (qu.Count > 0)
            {
                char curr = qu.Dequeue();
                if (dict.ContainsKey(curr))
                {
                    List<char> currList = dict[curr];
                    if (currList == null)
                    {
                        result += $"{curr} : Kangxi radical\n";
                    }
                    else if (currList.Count >= 2)
                    {
                        result += $"{curr} : {currList[0]}   {currList[1]}\n";
                        qu.Enqueue(currList[0]);
                        qu.Enqueue(currList[1]);
                    }
                    else if (currList.Count == 1)
                    {
                        result += $"{curr} : {currList[0]}\n";
                        qu.Enqueue(currList[0]);
                    }
                    else
                    {
                        result += $"{curr} : basic stroke\n";
                    }
                }
            }

            result += "\n definitions : \n";

            foreach (Word w in allWords)
            {
                if (w.Simplified.Length == 1 && w.Simplified[0] == c)
                {
                    string def = w.Definitions.Replace('/', '\n');
                    result += $"{c}: {def}";
                }
            }

            return result;
        }*/

    }
}
