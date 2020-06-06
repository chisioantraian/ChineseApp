using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ChineseAppWPF.UI
{
    public static partial class UiLayer
    {
        private static MainWindow mainWindow;
        private static Dictionary<string, DetailedWord> allDetailedWords;
        private static Dictionary<string, List<string>> basicDict;
        private static IEnumerable<Word> currentWords = new List<Word>();

        private static SelectedLanguage selectedLanguage = SelectedLanguage.English;
        private static SortingMethod sortingMethod = SortingMethod.Frequency;

        private delegate void ShowLanguageResult();
        private static ShowLanguageResult showLanguageResult;

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

    }
}