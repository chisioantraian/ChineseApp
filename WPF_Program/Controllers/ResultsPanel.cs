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
        private static string pinyin = "   ,. 0123456789/abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static void ShowEnglishResult()
        {
            string text = mainWindow.SearchBar.Text;
            ChineseService.GetEnglishResult(text).UpdateShownWords();
        }

        public static void ShowChineseResult()
        {
            ShowChineseResult("");
        }

        public static void ShowChineseResult(string value = "")
        {
            string text = mainWindow.SearchBar.Text;

            if (value != "")
                text = value;

            //TODO create Set with all possible pinyins
            bool isPinyin = false;
            foreach (char c in text)
            {
                if (pinyin.Contains(c))
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
                ChineseService.SearchBySimplified(text).UpdateShownWords();
            }
        }

        internal static void ShowComposeResult(char component) => Decomposition.GetCharactersWithComponent(component.ToString()).UpdateShownWords();

        internal static void ShowSomeRandomWords() => ChineseService.GetRandomWords().UpdateShownWords();

        internal static void ShowWordWithThisCharacter(char character) => ShowCharacterDecomposition(character);


        internal static void ShowCharsWithComponent_SidePanel(char character)
        {
            IEnumerable<Word> words = Decomposition.GetCharactersWithComponent(character.ToString());
            words.Update_ShownCharsWithComponent(character);
        }

        internal static void ShowWordsWithCharacter_SidePanel(char character)
        {
            ChineseService.SearchBySimplified(character.ToString()).Update_ShownWordsWithCharacters(character);
        }

        internal static Brush ComputeColor(string pron)
        {
            if (pron.Contains("1")) return Brushes.Red;
            if (pron.Contains("2")) return Brushes.LimeGreen;   //Green
            if (pron.Contains("3")) return Brushes.Blue;
            if (pron.Contains("4")) return Brushes.DarkMagenta; // Purple
            return Brushes.Gray;
        }


        internal static void Update_ShownCharsWithComponent(this IEnumerable<Word> filteredWords, char character)
        {
            ResultWord ResultedWordFromWord(Word word)
            {
                List<string> singlePron = word.Pinyin.Split(" ").ToList();
                List<SPPair> sPPairs = new List<SPPair>();

                /*for (int i = 0; i < singlePron.Count; i++)
                {
                    sPPairs.Add(new SPPair { ChineseCharacter = word.Simplified[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i] });
                }

                sPPairs.Add(new SPPair { ChineseCharacter = ' ', CharacterColor = Brushes.Black, Pinyin = "" });
                sPPairs.Add(new SPPair { ChineseCharacter = '〔', CharacterColor = Brushes.DarkSlateGray, Pinyin = "" });
                
                int j = 0;
                for (int i = singlePron.Count; i < word.Longer.Length &&  j < singlePron.Count; i++, j++)
                {
                    SPPair pair = new SPPair { ChineseCharacter = word.Longer[i], CharacterColor = ComputeColor(singlePron[j]), Pinyin = singlePron[j]};
                    sPPairs.Add(pair);
                }
                if (j != 0)
                {
                    sPPairs.Add(new SPPair { ChineseCharacter = '〕', CharacterColor = Brushes.DarkSlateGray, Pinyin = "" });
                }*/
                for (int i = 0; i < word.Simplified.Length && i < singlePron.Count; i++)
                {
                    sPPairs.Add(new SPPair { ChineseCharacter = word.Simplified[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i] });
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
                            sPPairs.Add(new SPPair { ChineseCharacter = word.Traditional[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i] });
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

            filteredWords = filteredWords.SortByFrequency();
            mainWindow.CharLeftPanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.RightPanelCounter.Text = $"Characters with component {character} : {filteredWords.Count()}";
        }



        internal static void Update_ShownWordsWithCharacters(this IEnumerable<Word> filteredWords, char character)
        {
            ResultWord ResultedWordFromWord(Word word)
            {
                List<string> singlePron = word.Pinyin.Split(" ").ToList();
                List<SPPair> sPPairs = new List<SPPair>();

                for (int i = 0; i < word.Simplified.Length && i < singlePron.Count; i++)
                {
                    sPPairs.Add(new SPPair { ChineseCharacter = word.Simplified[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i] });
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
                            sPPairs.Add(new SPPair { ChineseCharacter = word.Traditional[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i] });
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



            filteredWords = filteredWords.SortByFrequency();



            mainWindow.CharRightPanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.LeftPanelCounter.Text = $"Words with character {character} : {filteredWords.Count()}";
        }



        internal static void UpdateShownWords(this IEnumerable<Word> filteredWords, bool showSorted = true)
        {
            ResultWord ResultedWordFromWord(Word word)
            {
                List<string> singlePron = word.Pinyin.Split(" ").ToList();
                List<SPPair> sPPairs = new List<SPPair>();

                for (int i = 0; i < word.Simplified.Length && i < singlePron.Count; i++)
                {
                    sPPairs.Add(new SPPair { ChineseCharacter = word.Simplified[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i] });
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
                    case SortingMethod.Frequency:
                        filteredWords = filteredWords.SortByFrequency();
                        break;

                    case SortingMethod.Strokes:
                        filteredWords = filteredWords.SortByStrokesCount();
                        break;

                    case SortingMethod.Pinyin:
                        filteredWords = filteredWords.SortByPinyin();
                        break;

                    case SortingMethod.Exact:
                        filteredWords = filteredWords.SortByExactity(mainWindow.SearchBar.Text, selectedLanguage);
                        break;
                }
            }


            mainWindow.WordsList.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.WordsCount.Text = $"{filteredWords.Count()} words found";
            //
            previousWords = currentWords;
            currentWords = filteredWords;
            mainWindow.UndoButton.IsEnabled = true;

        }
    }
}
