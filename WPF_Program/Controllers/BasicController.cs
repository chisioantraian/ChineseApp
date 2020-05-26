using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static SelectedLanguage selectedLanguage = SelectedLanguage.English;
        private static SortingMethod sortingMethod = SortingMethod.Frequency;

        private delegate void ShowLanguageResult();
        private static ShowLanguageResult showLanguageResult;// = ShowEnglishResult;

        public static void SetWindow(MainWindow window)
        {
            mainWindow = window;
            allDetailedWords = ChineseService.GetAllDetailedWords();
            basicDict = Decomposition.GetBasicDict();
            showLanguageResult = ShowEnglishResult;
        }

        public static void ChangeToEnglishInput()
        {
            mainWindow.SearchBar.Text = "";
            selectedLanguage = SelectedLanguage.English;
            mainWindow.SearchBarPlaceholder.Text = "Enter your english word, then press enter";
            showLanguageResult = ShowEnglishResult;
        }

        public static void ChangeToChineseInput()
        {
            mainWindow.SearchBar.Text = "";
            selectedLanguage = SelectedLanguage.Chinese;
            mainWindow.SearchBarPlaceholder.Text = "Enter your characters/pinyin, then press enter";
            showLanguageResult = ShowChineseResult;
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
            showLanguageResult();
        }

        public static void SortByFrequency()
        {
            sortingMethod = SortingMethod.Frequency;
            currentWords.UpdateShownWords();
        }

        public static void SortByNumberOfStrokes()
        {
            sortingMethod = SortingMethod.Strokes;
            currentWords.UpdateShownWords();
        }

        public static void SortByPinyin()
        {
            sortingMethod = SortingMethod.Pinyin;
            currentWords.UpdateShownWords();
        }

        public static void SortByExact()
        {
            sortingMethod = SortingMethod.Exact;
            currentWords.UpdateShownWords();
        }

        public static IEnumerable<Breakdown> GetNoAlgBreakdown(string sentence)
        {
            //Console.WriteLine("Begin GetNoAlgBreakdown");
            List<string> wordParts = ChineseService.GetSimplifiedWordsFromSentence(sentence).ToList();
            //Console.WriteLine("After GetSimplifiedWordsFromSentence");
            foreach (string part in wordParts)
            {
                if (allDetailedWords.ContainsKey(part))
                    yield return new Breakdown { Part = part, Description = allDetailedWords[part].DominantPos };
                else
                    yield return new Breakdown { Part = part, Description = part }; // = "-"
            }
            //Console.WriteLine("End GetNoAlgBreakdown");
        }

        internal static List<Breakdown> GetAlgBreakdown(List<Breakdown> noAlg)
        {
            //Console.WriteLine("Begin GetAlgBreakdown");
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
            //Console.WriteLine("End GetAlgBreakdown");

            return algList;
        }
    }
}