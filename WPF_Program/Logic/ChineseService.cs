using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChineseAppWPF.Logic
{
    public static partial class ChineseService
    {
        private static readonly List<Word> allWords = BuildAllWords();
        private static readonly Dictionary<string, DetailedWord> allDetailedWords = BuildAllDetailedWords();
        private static readonly HashSet<string> wordsSet = BuildAllWordsSet();
        private static readonly HashSet<char> characterSet = BuildCharacterSet();
        private static readonly HashSet<string> onlyTraditionalSet = BuildOnlyTraditionalSet();
        private static readonly Dictionary<char, int> strokesDict = BuildStrokesDict();

        public static List<Word> GetAllWords() => allWords;

        public static Dictionary<string, DetailedWord> GetAllDetailedWords() => allDetailedWords;


        public static bool ContainsInsensitive(this string source, string value)
        {
            return source?.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static IEnumerable<Word> GetEnglishResult(string text)
        {
            return allWords.AsParallel()
                           .Where(w => w.Definitions.ContainsInsensitive(text));
        }

        public static bool ContainsRegexPattern(string text)
        {
            return text.Contains('^') ||
                   text.Contains('*') ||
                   text.Contains('.') ||
                   text.Contains('$') ||
                   text.Contains('+') ||
                   text.Contains('?');
        }

        public static IEnumerable<Word> SearchBySimplified(string text)
        {
            if (ContainsRegexPattern(text))
            {
                Regex rgx = new Regex(@text);
                ChineseSystem writingSystem = GetWritingSystem(text);

                if (writingSystem == ChineseSystem.Simplified)
                {
                    return allWords.AsParallel()
                                   .Where(w => rgx.IsMatch(w.Simplified));
                }
                else
                {
                    return allWords.AsParallel()
                                   .Where(w => rgx.IsMatch(w.Traditional));
                }
            }
            else
            {
                ChineseSystem writingSystem = GetWritingSystem(text);

                if (writingSystem == ChineseSystem.Simplified)
                {
                    return allWords.AsParallel()
                                   .Where(w => w.Simplified.Contains(text));
                }
                else
                {
                    return allWords.AsParallel()
                                   .Where(w => w.Traditional.Contains(text));
                }
            }
        }

        public static IEnumerable<Word> SearchByPinyin(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<Word>();
            string[] prons = text.Split(' ');
            bool CheckIfPinyinMatches(Word word)
            {
                string[] wordProns = word.Pinyin.Split(' ');
                if (prons.Length != wordProns.Length)
                    return false;
                for (int i = 0; i <= prons.Length - 1; i++)
                {
                    if (!wordProns[i].StartsWith(prons[i]))
                        return false;
                    if (wordProns[i].Length != (prons[i].Length + 1))
                        return false;
                }
                return true;
            }
            return allWords.AsParallel()
                           .Where(CheckIfPinyinMatches);
        }

        public static IEnumerable<Word> GetRandomWords()
        {
            Random random = new Random();
            List<Word> result = new List<Word>();
            for (int i = 0; i < 20; i++)
            {
                int index = random.Next(allWords.Count);
                result.Add(allWords[index]);
            }
            return result;
        }

        internal static bool IsCharacter(char character)
        {
            return characterSet.Contains(character);
        }

        internal static bool IsPunctuation(string word)
        {
            return word == "," || word == "，" ||
                   word == "." || word == "。" ||
                   word == "?" || word == "？" ||
                   word == "!" || word == "！";
        }

        internal static bool WordExists(string simplOfWord)
        {
            if (IsPunctuation(simplOfWord))
                return true;
            return wordsSet.Contains(simplOfWord);
        }

        public static IEnumerable<Word> GetAllWordsFrom(char character)
        {
            string ch = character.ToString();
            if (GetWritingSystem(ch) == ChineseSystem.Simplified)
            {
                return allWords.Where(w => w.Simplified == ch);
            }
            else
            {
                return allWords.Where(w => w.Traditional == ch);
            }
        }

        public static IEnumerable<Word> GetAllWordsFrom(IEnumerable<string> simpList)
        {
            ChineseSystem writingSystem = GetWritingSystem(simpList);

            if (writingSystem == ChineseSystem.Simplified)
            {
                return from simp in simpList
                       from w in allWords
                       where w.Simplified == simp
                       select w;
            }
            else
            {
                return from simp in simpList
                       from w in allWords
                       where w.Traditional == simp
                       select w;
            }
        }

        public static ChineseSystem GetWritingSystem(string text)
        {
            if (onlyTraditionalSet.Contains(text))
                return ChineseSystem.Traditional;
            return ChineseSystem.Simplified;
        }

        public static ChineseSystem GetWritingSystem(IEnumerable<string> simpList)
        {
            foreach (string s in simpList)
            {
                if (GetWritingSystem(s) == ChineseSystem.Traditional)
                {
                    return ChineseSystem.Traditional;
                }
            }
            return ChineseSystem.Simplified;
        }

        public static IEnumerable<string> GetSimplifiedWordsFromSentence(string sentence)
        {
            int i = 0;
            int j = sentence.Length;
            List<string> result = new List<string>();

            while (i < j)
            {
                string possible = sentence[i..j];
                if (WordExists(possible))
                {
                    result.Add(possible);
                    i = j;
                    j = sentence.Length;
                }
                else
                {
                    j--;
                }
            }
            return result;
        }

        public static List<Word> GetWordsWithSimplified(string simplified)
        {
            return allWords.Where(w => w.Simplified == simplified).ToList();
        }

        public static List<Word> SearchWordsInside(string simplified)
        {
            List<Word> result = new List<Word>();

            for (int i = 0; i < simplified.Length; i++)
            {
                for (int j = i + 1; j <= simplified.Length; j++)
                {
                    string possible = simplified[i..j];
                    if (WordExists(possible))
                    {
                        result.AddRange(GetWordsWithSimplified(possible));
                    }
                }
            }
            result.Reverse();
            return result.OrderBy(w => w.Simplified.Length).Reverse().ToList();
        }
    }
}