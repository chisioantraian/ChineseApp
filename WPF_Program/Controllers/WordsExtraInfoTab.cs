using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static void ShowCharacterDecomposition(char characterToBeDecomposed)
        {
            //string decompositionText = Decomposition.DecomposeCharToRadicals(characterToBeDecomposed);
            //mainWindow.DecompositionBlock.Text = $"{decompositionText} ";

            //mainWindow.DecompositionWordList.Items.Clear();

            List<string> chars = Decomposition.DecomposeCharToRadicals(characterToBeDecomposed);
            List<Word> words = ChineseService.GetAllWordsFrom(chars).ToList();

            //foreach (Word w in words)
            //{
            //    ListBoxItem item = new ListBoxItem { Content = $"{w.Simplified} , {w.Pinyin}", FontSize = 20 };
            //    mainWindow.DecompositionWordList.Items.Add(item);
            //}

            UpdateShownWords_Left(words);

        }

        internal static void UpdateShownWords_Left(this IEnumerable<Word> filteredWords)
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

            mainWindow.DecompositionWordList.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            //mainWindow.WordsList.ItemsSource = filteredWords.Select(ResultedWordFromWord);
        }
    }
}
