using System.Collections.Generic;
using System.Linq;

using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        internal static void ShowChineseResult()
        {
            ChineseService.SearchBySimplified(mainWindow.SearchBar.Text).UpdateShownWords();
        }

        internal static void ShowEnglishResult()
        {
            ChineseService.GetEnglishResult(mainWindow.SearchBar.Text).UpdateShownWords();
        }

        internal static void ShowPronounciationResult()
        {
            ChineseService.SearchByPinyin(mainWindow.SearchBar.Text).UpdateShownWords();
        }

        internal static void ShowSomeRandomWords()
        {
            ChineseService.GetRandomWords().UpdateShownWords();
        }

        internal static void ShowWordWithThisCharacter(char character)
        {
            ShowCharacterDecomposition(character);
        }

        internal static void UpdateShownWords(this List<Word> filteredWords)
        {
            static SPPair makeSPP(char chn, string pron) => new SPPair { ChineseCharacter = chn, Pinyin = pron };
            List<ResultWord> resultedWords = new List<ResultWord>();
            foreach (var word in filteredWords)
            {
                IEnumerable<char> singleChar = word.Simplified;
                IEnumerable<string> singlePron = word.Pinyin.Split(" ");

                IEnumerable<SPPair> sPPairs = singleChar.Zip(singlePron, makeSPP);

                var resultedWord = new ResultWord
                {
                    SimplifiedPinyinPairs = sPPairs,
                    Definitions = word.Definitions
                };
                resultedWords.Add(resultedWord);
            }

            mainWindow.WordsList.ItemsSource = resultedWords;
        }
    }
}
