using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ChineseAppWPF.Logic;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private const string pinyin = "   ,. 0123456789/abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static void ShowEnglishResult()
        {
            string text = mainWindow.SearchBar.Text;
            ChineseService.GetEnglishResult(text).UpdateShownWords();
        }

        public static void ShowChineseResult()
        {
            if (mainWindow.SearchBar.Text != "")
                ShowChineseResult(mainWindow.SearchBar.Text);
            else
                ShowChineseResult("");
        }

        public static void ShowChineseResult(string value = "")
        {
            string text = mainWindow.SearchBar.Text;

            if (string.IsNullOrEmpty(value))
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

        internal static TextBlock ComputeDefinitionBlock(string definition)
        {

            string text = mainWindow.SearchBar.Text;
            TextBlock result = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0,2,0,0)
            };

            text = text.Replace("(", @"\(");
            text = text.Replace(")", @"\)");
            
            string pattern = @$"({text})";
            string[] substrings = Regex.Split(definition, pattern, RegexOptions.IgnoreCase);

            foreach (string match in substrings)
            {
                if (match.ToLower() == text.ToLower())
                {
                    result.Inlines.Add(new Run(match)
                    {
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        //Foreground = Brushes.Chocolate,
                        Background = Brushes.BurlyWood//.LightSteelBlue
                    });
                }
                else
                {
                    result.Inlines.Add(new Run(match)
                    {
                        FontSize = 16,
                        Foreground = Brushes.Black,
                    });
                }
            }
            return result;
        }

        internal static void Update_ShownCharsWithComponent(this IEnumerable<Word> filteredWords, char character)
        {
            static ResultWord ResultedWordFromWord(Word word)
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
                    DefinitionBlock = new TextBlock 
                    {
                        Text = word.Definitions,
                        Foreground = Brushes.Black,
                        TextWrapping = TextWrapping.Wrap, 
                    }
                };
            }

            filteredWords = filteredWords.SortByFrequency();
            mainWindow.CharsExtraPanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.CharsExtraCounter.Text = $"Characters with component {character}";
        }

        internal static void Update_ShownWordsWithCharacters(this IEnumerable<Word> filteredWords, char character)
        {
            static ResultWord ResultedWordFromWord(Word word)
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
                    DefinitionBlock = new TextBlock
                    {
                        Text = word.Definitions,
                        Foreground = Brushes.Black,
                        TextWrapping = TextWrapping.Wrap,
                    }
                };
            }

            filteredWords = filteredWords.SortByFrequency();

            mainWindow.WordsExtraPanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.WordsExtraCounter.Text = $"Words with character {character}";
        }

        internal static void UpdateShownWords(this IEnumerable<Word> filteredWords, bool showSorted = true)
        {
            static ResultWord ResultedWordFromWord(Word word)
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
                    //Definitions = word.Definitions,
                    //DefinitionBlocks = new List<TextBlock> { new TextBlock { Text = word.Definitions } }
                    DefinitionBlock = ComputeDefinitionBlock(word.Definitions)
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
            //
            currentWords = filteredWords;
        }
    }
}
