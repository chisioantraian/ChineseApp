using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
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

        internal static Brush ComputeColor(string pron)
        {
            if (pron.Contains("1"))
                return Brushes.Red;
            if (pron.Contains("2"))
                return Brushes.LimeGreen; //Green
            if (pron.Contains("3"))
                return Brushes.Blue;
            if (pron.Contains("4"))
                return Brushes.DarkMagenta; // Purple
            return Brushes.Gray;
        }

        /*
         * Change in file; will obtain directly
         * (marks are not visible)
         */
        internal static string PronounciationWithToneMarker(string pron)
        {
            return pron switch
            {
                "ai4" => "ài",
                "ao4" => "ào",
                _ => pron
            };
        }

        internal static void UpdateShownWords(this IEnumerable<Word> filteredWords, string writingSystem, string sortingMethod, bool showSorted = true)
        {
            static SPPair makeSPP(char chn, string pron) 
            {
                return new SPPair
                {
                    ChineseCharacter = chn,
                    CharacterColor = ComputeColor(pron),
                    Pinyin = pron //PronounciationWithToneMarker(pron),
                };
            }

            ResultWord ResultedWordFromWord(Word word)
            {
                //IEnumerable<char> singleChar = writingSystem == "Simplified" ? word.Simplified : word.Traditional;
                List<string> singlePron = word.Pinyin.Split(" ").ToList();
                //IEnumerable<SPPair> sPPairs = singleChar.Zip(singlePron, makeSPP);

                if (singlePron.Count != word.Simplified.Length)
                    Console.WriteLine($"{word.Simplified} {word.Pinyin}");

                List<SPPair> sPPairs = new List<SPPair>();
                for (int i = 0; i < word.Simplified.Length && i < singlePron.Count; i++)
                {
                    sPPairs.Add(new SPPair
                    {
                        ChineseCharacter = word.Simplified[i],
                        CharacterColor = ComputeColor(singlePron[i]),
                        Pinyin = singlePron[i]
                    });
                }

                bool addBrackets = false;
                for (int i = 0; i < word.Simplified.Length && i < singlePron.Count; i++)
                {
                    if (word.Simplified[i] != word.Traditional[i])
                    {
                        addBrackets = true;
                        break;
                    }
                }

                if (addBrackets)
                {
                    sPPairs.Add(new SPPair { ChineseCharacter = ' ', CharacterColor = Brushes.Black, Pinyin = "" });
                    sPPairs.Add(new SPPair { ChineseCharacter = '〔', CharacterColor = Brushes.DarkSlateGray, Pinyin = "" });
                    for (int i = 0; i < word.Traditional.Length && i < singlePron.Count; i++)
                    {
                        if (word.Simplified[i] == word.Traditional[i])
                        {
                            sPPairs.Add(new SPPair { ChineseCharacter = '-', CharacterColor = Brushes.DarkSlateGray, Pinyin = "" });
                        }
                        else
                        {
                            sPPairs.Add(new SPPair { ChineseCharacter = word.Traditional[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i]});
                        }
                    }
                    sPPairs.Add(new SPPair { ChineseCharacter = '〕', CharacterColor = Brushes.DarkSlateGray, Pinyin = "" });
                }

               

                return new ResultWord
                {
                    SimplifiedPinyinPairs = sPPairs,
                    Definitions = word.Definitions,
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
