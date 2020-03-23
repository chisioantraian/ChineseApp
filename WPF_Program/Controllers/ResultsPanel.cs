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

        internal static void ShowWordWithThisCharacter(char character) => ShowCharacterDecomposition(character);

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

            mainWindow.WordsList.ItemsSource = filteredWords.Select(ResultedWordFromWord);
        }
    }
}
