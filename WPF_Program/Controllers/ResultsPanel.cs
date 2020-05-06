using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static string english = "   ,. 0123456789/abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        internal static void ShowEnglishResult()
        {
            string text = mainWindow.SearchBar.Text;
            ChineseService.GetEnglishResult(text).UpdateShownWords();
        }

        /*internal static void ShowChineseResult(string text = "")
        {
            if (text == "")
                text = mainWindow.SearchBar.Text;
            ShowChineseResult(text, writingState, sortingState);
        }*/

        internal static void ShowChineseResult(string value = "")
        {
            string text = mainWindow.SearchBar.Text;

            if (value != "")
                text = value;

            bool isPinyin = false;
            foreach (char c in text)
            {
                if (english.Contains(c))
                {
                    isPinyin = true;
                    break;
                }
            }

            if (isPinyin)
            {
                ChineseService.SearchByPinyin(text).UpdateShownWords();
            }
            else
            {
                ChineseService.SearchBySimplified(text, writingState).UpdateShownWords();
            }
        }

        internal static void ShowComposeResult(string value) => Decomposition.GetCharactersWithComponent(value, writingState).UpdateShownWords();

        internal static void ShowSomeRandomWords() => ChineseService.GetRandomWords().UpdateShownWords();

        internal static void ShowWordWithThisCharacter(char character) => ShowCharacterDecomposition(character.ToString(), writingState);


        internal static void UpdateShownWords(this IEnumerable<Word> filteredWords, bool showSorted = true)
        {
            filteredWords.UpdateShownWords(writingState, sortingState, showSorted);
        }

        internal static void UpdateShownWords(this IEnumerable<Word> filteredWords, string writingSystem, string sortingMethod, bool showSorted = true)
        {
            static SPPair makeSPP(char chn, string pron) => new SPPair { ChineseCharacter = chn, Pinyin = pron };

            ResultWord ResultedWordFromWord(Word word)
            {
                IEnumerable<char> singleChar = writingSystem == "Simplified" ? word.Simplified : word.Traditional;
                IEnumerable<string> singlePron = word.Pinyin.Split(" ");
                IEnumerable<SPPair> sPPairs = singleChar.Zip(singlePron, makeSPP);

                return new ResultWord
                {
                    SimplifiedPinyinPairs = sPPairs,
                    Definitions = word.Definitions
                };
            }

            if (showSorted)
            {
                switch (sortingMethod)
                {
                    case "Frequency":
                        filteredWords = filteredWords.SortByFrequency();
                        break;

                    case "Strokes":
                        filteredWords = filteredWords.SortByStrokesCount(writingState);
                        break;

                    case "Pinyin":
                        filteredWords = filteredWords.SortByPinyin();
                        break;

                    case "Exact":
                        ComboBoxItem typeItem = (ComboBoxItem)mainWindow.InputComboBox.SelectedItem;
                        string language = typeItem.Content.ToString();
                        filteredWords = filteredWords.SortByExactity(mainWindow.SearchBar.Text, language);
                        break;
                }
            }


            mainWindow.WordsList.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.WordsCount.Text = $"{filteredWords.Count()} words found";
            //
            previousWords = currentWords; // ?
            currentWords = filteredWords;
            mainWindow.UndoButton.IsEnabled = true;

        }
    }
}
