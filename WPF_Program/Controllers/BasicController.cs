using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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

        public static void SetWindow(MainWindow window)
        {
            mainWindow = window;
            allDetailedWords = ChineseService.GetAllDetailedWords();
        }

        public static void ShowResult()
        {
            if (string.IsNullOrEmpty(mainWindow.SearchBar.Text))
            {
                return;
            }
            ComboBoxItem typeItem = (ComboBoxItem)mainWindow.InputComboBox.SelectedItem;
            switch (typeItem.Content.ToString())
            {
                case "Chinese": ShowChineseResult(); break;
                case "English": ShowEnglishResult(); break;
                case "Pronounciation": ShowPronounciationResult(); break;
                case "Compose": ShowComposeResult(); break;
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