using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WPF_program.Logic;
using WPF_program.Models;

namespace WPF_program.Controllers
{
    public class SPPair
    {
        public char ChineseCharacter { get; set; }
        public string Pinyin { get; set; } 
    }
    public class ResultWord
    {
        public List<SPPair> sPPairs { get; set; }// = new List<SPPair>();
        public string Definitions { get; set; }
    }

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

        internal static void UpdateShownWords(this List<Word> filteredWords)
        {
            SPPair makeSPP(char chn, string pron) => new SPPair { ChineseCharacter = chn, Pinyin = pron };
            List<ResultWord> resultedWords = new List<ResultWord>();
            foreach (var word in filteredWords)
            {
                List<SPPair> sPPairs = new List<SPPair>();
                List<char> singleChar = word.Simplified.ToList();
                List<string> singlePron = word.Pinyin.Split(" ").ToList();
                //for (int i = 0; i < singleChar.Count && i < singlePron.Count; i++)
                //{
                //    SPPair sPair = new SPPair { ChineseCharacter = singleChar[i], Pinyin = singlePron[i] };
                //    sPPairs.Add(sPair);
                //}
                sPPairs = singleChar.Zip(singlePron, makeSPP).ToList();

                var rWord = new ResultWord
                {
                    sPPairs = sPPairs,
                    Definitions = word.Definitions
                };
                resultedWords.Add(rWord);
            }

            mainWindow.WordsList.ItemsSource = resultedWords;
        }

        internal static void ShowWordWithThisCharacter(char character)
        {
            ShowCharacterDecomposition(character);
        }
        
        internal static void ShowSomeRandomWords()
        {
            List<Word> randomWords = ChineseService.GetRandomWords();
            UpdateShownWords(randomWords);
        }
    }
}
