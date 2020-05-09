using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Windows.Controls;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static MainWindow mainWindow;
        private static Dictionary<string, DetailedWord> allDetailedWords;
        private const string testsPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\testSentences.utf8";
        private static List<Sentence> sentences = new List<Sentence>();
        private static readonly List<Sentence> wrongSentences = new List<Sentence>();
        private static Dictionary<string, List<string>> basicDict;

        private static IEnumerable<Word> currentWords = new List<Word>();
        private static IEnumerable<Word> previousWords = new List<Word>();

        private static string sortingState = "Frequency";
        private static string writingState = "Simplified";

        public static void SetWindow(MainWindow window)
        {
            mainWindow = window;
            allDetailedWords = ChineseService.GetAllDetailedWords();
            basicDict = Decomposition.GetBasicDict();
        }

        public static void ShowResult()
        {
            if (mainWindow == null)
                return;
            if (string.IsNullOrEmpty(mainWindow.SearchBar.Text))
            {
                currentWords = new List<Word>(); 
                currentWords.UpdateShownWords();
                return;
            }
            ComboBoxItem typeItem = (ComboBoxItem)mainWindow.InputComboBox.SelectedItem;
            switch (typeItem.Content.ToString())
            {
                case "English":
                    var t = new Thread(ShowEnglishThread);
                    t.Start();
                    //ShowEnglishResult(); 
                    break;
                case "Chinese": ShowChineseResult(); break;
            }
        }

        public static void ShowEnglishThread()
        {
            mainWindow.Dispatcher.BeginInvoke(new Action(() => ShowEnglishResult()));
        }

        public static void Undo()
        {
            currentWords = previousWords;
            currentWords.UpdateShownWords();

            mainWindow.UndoButton.IsEnabled = false;
        }

        private static void constructResultedWords(List<Word> result, List<string> arr, int start, int end)
        {
            if (end == arr.Count)
                return;

            else if (start > end)
                constructResultedWords(result, arr, 0, end + 1);

            else
            {
                string s = "";
                for (int i = start; i <= end; i++)
                {
                    Console.Write(arr[i]);
                    s += arr[i];//Console.Write(arr[i]);
                }
                Console.WriteLine();
                if (ChineseService.WordExists(s))
                {
                    //IEnumerable<Word> wds = ChineseService.SearchBySimplified(s);
                    IEnumerable<Word> wds = ChineseService.GetAllWordsFrom(new List<string> { s }, writingState);
                    foreach (Word w in wds)
                        result.Add(w);
                }
                //Console.WriteLine();
                constructResultedWords(result, arr, start + 1, end);
            }
            return;
        }

        public static void ShowWordsFromThisChars(List<string> chars)
        {
            List<Word> result = new List<Word>();

            constructResultedWords(result, chars, 0, 0);
            result.UpdateShownWords(false);
        }


        public static void SortResult()
        {
            if (mainWindow == null)
                return;

            ComboBoxItem sortItem = (ComboBoxItem)mainWindow.SortingComboBox.SelectedItem;
            string selectedSorting = sortItem.Content.ToString();
            
            if (sortingState != selectedSorting)
            {
                sortingState = selectedSorting;
                currentWords.UpdateShownWords();
            }
        }

        public static void ChangeWritingSystem()
        {
            if (mainWindow == null)
                return;

            ComboBoxItem item = (ComboBoxItem)mainWindow.WritingSystemComboBox.SelectedItem;
            string selectedSystem = item.Content.ToString();

            if (writingState != selectedSystem)
            {
                writingState = selectedSystem;
                currentWords.UpdateShownWords();
            }
        }

        public static IEnumerable<Breakdown> GetNoAlgBreakdown(string sentence)
        {
            IEnumerable<string> wordParts = ChineseService.GetSimplifiedWordsFromSentence(sentence);
            foreach (string part in wordParts)
            {
                if (allDetailedWords.ContainsKey(part))
                    yield return new Breakdown { Part = part, Description = allDetailedWords[part].DominantPos };
                else
                    yield return new Breakdown { Part = part, Description = part }; // = "-"
            }
        }

        internal static List<Breakdown> GetAlgBreakdown(List<Breakdown> noAlg)
        {
            List<Breakdown> algList = new List<Breakdown>();
            foreach (Breakdown bd in noAlg)
            {
                algList.Add(new Breakdown { Part = bd.Part, Description = bd.Description });
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