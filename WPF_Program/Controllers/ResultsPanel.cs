using System.Collections.Generic;
using System.Linq;

using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        internal static void ShowChineseResult() => ChineseService.SearchBySimplified(mainWindow.SearchBar.Text).UpdateShownWords();

        internal static void ShowEnglishResult() => ChineseService.GetEnglishResult(mainWindow.SearchBar.Text).UpdateShownWords();

        internal static void ShowPronounciationResult() => ChineseService.SearchByPinyin(mainWindow.SearchBar.Text).UpdateShownWords();

        internal static void ShowComposeResult() => Decomposition.GetCharactersWithComponent(mainWindow.SearchBar.Text).UpdateShownWords();

        internal static void ShowSomeRandomWords() => ChineseService.GetRandomWords().UpdateShownWords();

        internal static void ShowWordWithThisCharacter(char character) => ShowCharacterDecomposition(character.ToString());

        internal static void UpdateShownWords(this IEnumerable<Word> filteredWords)
        {
            static SPPair makeSPP(char chn, string pron) => new SPPair { ChineseCharacter = chn, Pinyin = pron };

            static ResultWord ResultedWordFromWord(Word word)
            {
                IEnumerable<char> singleChar = word.Simplified;
                IEnumerable<string> singlePron = word.Pinyin.Split(" ");
                IEnumerable<SPPair> sPPairs = singleChar.Zip(singlePron, makeSPP);

                return new ResultWord
                {
                    SimplifiedPinyinPairs = sPPairs,
                    Definitions = word.Definitions
                };
            }

            
            switch (sortingState)
            {
                case "Frequency":
                    filteredWords = filteredWords.SortByFrequency();
                    break;

                case "Strokes":
                    filteredWords = filteredWords.SortByStrokesCount();
                    break;

                case "Pinyin":
                    filteredWords = filteredWords.SortByPinyin();
                    break;
            }

            mainWindow.WordsList.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.WordsCount.Text = $"{filteredWords.Count()} words found";
            //
            currentWords = filteredWords;
        }
    }
}
