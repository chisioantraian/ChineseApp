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
        private static ShowLanguageResult showLanguageResult = ShowEnglishResult;

        public static void SetWindow(MainWindow window)
        {
            mainWindow = window;
            allDetailedWords = ChineseService.GetAllDetailedWords();
            basicDict = Decomposition.GetBasicDict();
        }

        public static void ChangeLanguage()
        {
            if (selectedLanguage == SelectedLanguage.English)
            {
                selectedLanguage = SelectedLanguage.Chinese;
                showLanguageResult = ShowChineseResult;
                mainWindow.ChangeLanguageButton.Content = "Input: Chinese\nChange to English";
            }
            else
            {
                selectedLanguage = SelectedLanguage.English;
                showLanguageResult = ShowEnglishResult;
                mainWindow.ChangeLanguageButton.Content = "Input: English\nChange to Chinese";
            }
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

            //stopwatch.Restart();
            showLanguageResult();

            //stopwatch.Stop();

            //Console.WriteLine($"\nsearch time: {stopwatch.ElapsedMilliseconds} ms");
        }

        private static SortingMethod GetSorting(string text)
        {
            return text switch
            {
                "Frequency" => SortingMethod.Frequency,
                "Strokes" => SortingMethod.Strokes,
                "Pinyin" => SortingMethod.Pinyin,
                _ => SortingMethod.Exact
            };
        }

        public static void SortResult()
        {
            if (mainWindow == null)
                return;

            //TODO bind enum to combobox?
            //TODO or change combobox to button
            ComboBoxItem sortItem = (ComboBoxItem)mainWindow.SortingComboBox.SelectedItem;
            SortingMethod selectedSorting = GetSorting(sortItem.Content.ToString());

            if (sortingMethod != selectedSorting)
            {
                sortingMethod = selectedSorting;
                currentWords.UpdateShownWords();
            }
        }

        public static IEnumerable<Breakdown> GetNoAlgBreakdown(string sentence)
        {
            Console.WriteLine("Begin GetNoAlgBreakdown");

            List<string> wordParts = ChineseService.GetSimplifiedWordsFromSentence(sentence).ToList();
            Console.WriteLine("After GetSimplifiedWordsFromSentence");
            foreach (string part in wordParts)
            {
                if (allDetailedWords.ContainsKey(part))
                    yield return new Breakdown { Part = part, Description = allDetailedWords[part].DominantPos };
                else
                    yield return new Breakdown { Part = part, Description = part }; // = "-"
            }
            Console.WriteLine("End GetNoAlgBreakdown");
        }

        internal static List<Breakdown> GetAlgBreakdown(List<Breakdown> noAlg)
        {
            Console.WriteLine("Begin GetAlgBreakdown");
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
            Console.WriteLine("End GetAlgBreakdown");

            return algList;
        }
    }
}