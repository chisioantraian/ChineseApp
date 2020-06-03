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
            string text = mainWindow.SearchBar.Text;

            if (IsProbablyPinyin(text))
            {
                ChineseService.SearchByPinyin(text).UpdateShownWords();
            }
            else
            {
                ChineseService.SearchBySimplified(text).UpdateShownWords();
            }
        }

        internal static bool IsProbablyPinyin(string text)
        {
            return text.Any(c => pinyin.Contains(c));
        }

        internal static void ShowSomeRandomWords() => ChineseService.GetRandomWords().UpdateShownWords();

        internal static void ShowDecompositionTreeOfCharacter(char character) => ShowCharacterDecomposition(character);

        internal static void ShowCharsWithComponent_SidePanel(char character)
        {
            Decomposition.GetCharactersWithComponent(character.ToString()).Update_ShownCharsWithComponent(character);
        }

        internal static void ShowWordsWithCharacter_SidePanel(char character)
        {
            ChineseService.SearchBySimplified(character.ToString()).Update_ShownWordsWithCharacters(character);
        }

        internal static void ShowWordsContainingWord_SidePanel(string simplified)
        {
            ChineseService.SearchBySimplified(simplified).Update_ShownWordsContainingWord(simplified);
        }

        internal static void ShowWordsInside_SidePanel(string simplified)
        {
            ChineseService.SearchWordsInside(simplified).Update_ShownInsideWords(simplified);
        }

        internal static Brush ComputeColor(string pron)
        {
            if (pron.Contains("1")) return Brushes.Red;
            if (pron.Contains("2")) return Brushes.LimeGreen;
            if (pron.Contains("3")) return Brushes.Blue;
            if (pron.Contains("4")) return Brushes.DarkMagenta;
            return Brushes.Gray;
        }

        internal static TextBlock ComputeDefinitionBlock(string definition)
        {
            TextBlock block = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0,4,0,0) };
            string text = mainWindow.SearchBar.Text
                            .Replace("(", @"\(")
                            .Replace(")", @"\)");

            string pattern = @$"({text})";
            string[] substrings = Regex.Split(definition, pattern, RegexOptions.IgnoreCase);

            foreach (string match in substrings)
            {
                if (match.ToLower() == text.ToLower())
                {
                    block.Inlines.Add(new Run(match) { FontSize = 16, FontWeight = FontWeights.Bold, Foreground = Brushes.Orange });
                }
                else
                {
                    block.Inlines.Add(new Run(match) { FontSize = 16, Foreground = Brushes.Black, });
                }
            }
            return block;
        }
        

        internal static void Update_ShownCharsWithComponent(this IEnumerable<Word> filteredWords, char character)
        {
            filteredWords = filteredWords.SortByFrequency();
            mainWindow.CharsExtraPanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.CharsExtraCounter.Text = $"Characters with component {character}";
        }

        internal static void Update_ShownInsideWords(this IEnumerable<Word> filteredWords, string simplified)
        {
            mainWindow.WordsInsidePanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.WordsInsideCounter.Text = $"Words inside {simplified}";
        }

        internal static void Update_ShownWordsWithCharacters(this IEnumerable<Word> filteredWords, char character)
        {
            filteredWords = filteredWords.SortByFrequency();
            mainWindow.WordsExtraPanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.WordsExtraCounter.Text = $"Words with character {character}";
        }

        internal static void Update_ShownWordsContainingWord(this IEnumerable<Word> filteredWords, string simplified)
        {
            filteredWords = filteredWords.SortByFrequency();
            mainWindow.WordsExtraPanel.ItemsSource = filteredWords.Select(ResultedWordFromWord);
            mainWindow.WordsExtraCounter.Text = $"Words containing word {simplified}";
        }

        internal static void UpdateShownWords(this IEnumerable<Word> filteredWords, bool showSorted = true)
        {
            if (showSorted)
            {
                filteredWords = sortingMethod switch
                {
                    SortingMethod.Frequency => filteredWords.SortByFrequency(),
                    SortingMethod.Strokes => filteredWords.SortByStrokesCount(),
                    SortingMethod.Pinyin => filteredWords.SortByPinyin(),
                    SortingMethod.Exact => filteredWords.SortByExactity(mainWindow.SearchBar.Text, selectedLanguage)
                };
            }

            mainWindow.WordsList.ItemsSource = filteredWords.Select(ResultedWordFromWord_Main);
            currentWords = filteredWords;
        }

        private static ResultWord ResultedWordFromWord_Main(Word word)
        {
            ResultWord resultWord = ResultedWordFromWord(word);
            resultWord.DefinitionBlock = ComputeDefinitionBlock(word.Definitions);
            return resultWord;
        }

        private static bool AddBrackets(Word word, List<string> singlePron)
        {
            for (int i = 0; i < word.Simplified.Length && i < singlePron.Count; i++)
            {
                if (word.Simplified[i] != word.Traditional[i])
                {
                    return true;
                }
            }
            return false;
        }

        private static ResultWord ResultedWordFromWord(Word word)
        {
            List<string> singlePron = word.Pinyin.Split(" ").ToList();
            List<SPPair> sPPairs = new List<SPPair>();

            for (int i = 0; i < word.Simplified.Length && i < singlePron.Count; i++)
            {
                sPPairs.Add(new SPPair { ChineseCharacter = word.Simplified[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i], SimplifiedWord = word.Simplified });
            }

            if (AddBrackets(word, singlePron))
            {
                sPPairs.Add(new SPPair { ChineseCharacter = ' ', CharacterColor = Brushes.Black, Pinyin = "", SimplifiedWord = word.Simplified });
                sPPairs.Add(new SPPair { ChineseCharacter = '〔', CharacterColor = Brushes.DarkSlateGray, Pinyin = "", SimplifiedWord = word.Simplified });
                for (int i = 0; i < word.Traditional.Length && i < singlePron.Count; i++)
                {
                    if (word.Simplified[i] == word.Traditional[i])
                    {
                        sPPairs.Add(new SPPair { ChineseCharacter = '-', CharacterColor = Brushes.DarkSlateGray, Pinyin = "", SimplifiedWord = word.Simplified });
                    }
                    else
                    {
                        sPPairs.Add(new SPPair { ChineseCharacter = word.Traditional[i], CharacterColor = ComputeColor(singlePron[i]), Pinyin = singlePron[i], SimplifiedWord = word.Simplified });
                    }
                }
                sPPairs.Add(new SPPair { ChineseCharacter = '〕', CharacterColor = Brushes.DarkSlateGray, Pinyin = "", SimplifiedWord = word.Simplified });
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
    }
}
