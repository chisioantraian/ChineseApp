using Chinese;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using static MyTypes;

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
            List<WordAvecFrequency> filteredWords = ChineseService.searchBySimplified(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void ShowEnglishResult()
        {
            List<WordAvecFrequency> filteredWords = ChineseService.getEnglishResult(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void ShowPronounciationResult()
        {
            List<WordAvecFrequency> filteredWords = ChineseService.searchByPinyin(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void UpdateShownWords(List<WordAvecFrequency> filteredWords)
        {
            //mainWindow.ResultCountBlock.Text = $"{filteredWords.Count} words found";
            //mainWindow.WordsList.Items.Add(mainWindow.ResultCountBlock);
            List<ResultWord> resultedWords = new List<ResultWord>();
            foreach (var word in filteredWords)
            {
                List<SPPair> sPPairs = new List<SPPair>();
                List<char> singleChar = word.Simplified.ToList();
                List<string> singlePron = word.Pinyin.Split(" ").ToList();
                for (int i = 0; i < singleChar.Count && i < singlePron.Count; i++)
                {
                    SPPair sPair = new SPPair { ChineseCharacter = singleChar[i], Pinyin = singlePron[i] };
                    sPPairs.Add(sPair);
                }

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
            List<WordAvecFrequency> randomWords = ChineseService.getRandomWords();
            UpdateShownWords(randomWords);
        }
    }
}
