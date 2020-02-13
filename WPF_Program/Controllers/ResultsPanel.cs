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
            List<Word> filteredWords = ChineseService.SearchBySimplified(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void ShowEnglishResult()
        {
            List<Word> filteredWords = ChineseService.GetEnglishResult(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void ShowPronounciationResult()
        {
            List<Word> filteredWords = ChineseService.SearchByPinyin(mainWindow.SearchBar.Text).ToList();
            UpdateShownWords(filteredWords);
        }

        internal static void UpdateShownWords(List<Word> filteredWords)
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
            List<Word> randomWords = ChineseService.GetRandomWords();
            UpdateShownWords(randomWords);
        }
    }
}
