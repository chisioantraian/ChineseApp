using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ChineseAppWPF.Models;

namespace ChineseAppWPF.Logic
{
    public static class ChineseService
    {
        private const string detailedPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\allDetailedWords.utf8";
        private const string wordsPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\allWords.utf8";
        private const string strokesPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\ucs-strokes.txt";

        private static List<Word> allWords = new List<Word>();
        private static Dictionary<string, DetailedWord> allDetailedWords = new Dictionary<string, DetailedWord>();
        private static HashSet<string> wordsSet = new HashSet<string>();
        private static Dictionary<char, int> strokesDict = new Dictionary<char, int>();

        public static void InitializeData()
        {
            BuildAllWords();
            BuildAllDetailedWords();
            BuildAllWordsSet();
            BuilStrokesDict();
        }

        public static List<Word> GetAllWords() => allWords;

        public static Dictionary<string, DetailedWord> GetAllDetailedWords() => allDetailedWords;

        private static void BuildAllWords()
        {
            allWords = File.ReadAllLines(wordsPath)
                           .AsParallel()
                           .Select(getWordFromLine)
                           .ToList();

            static Word getWordFromLine(string line)
            {
                string[] tokens = line.Split('\t');
                return new Word
                {
                    Traditional = tokens[0],
                    Simplified = tokens[1],
                    Pinyin = tokens[2],
                    Definitions = tokens[3],
                    Frequency = int.Parse(tokens[4])
                };
            }
        }

        private static void BuildAllDetailedWords()
        {
            allDetailedWords = File.ReadAllLines(detailedPath)
                                   .AsParallel()
                                   .Select(getDetailedWordFromLine)
                                   .ToDictionary(w => w.Simplified);

            static DetailedWord getDetailedWordFromLine(string line)
            {
                string[] tokens = line.Split('\t');
                return new DetailedWord
                {
                    Simplified = tokens[0],
                    Length = tokens[1],
                    Pinyin = tokens[2],
                    PinyinInput = tokens[3],
                    WCount = tokens[4],
                    WMillion = tokens[5],
                    Log10W = tokens[6],
                    Wcd = tokens[7],
                    WcdPercent = tokens[8],
                    Log10CD = tokens[9],
                    DominantPos = tokens[10],
                    DominantPosFreq = tokens[11],
                    AllPos = tokens[12],
                    AllPosFreq = tokens[13],
                    Definition = tokens[14]
                };
            }
        }

        private static void BuildAllWordsSet()
        {
            foreach (var word in allWords)
            {
                wordsSet.Add(word.Traditional);
                //if (word.Simplified == word.Traditional)
                wordsSet.Add(word.Simplified);
            }
        }

        private static void BuilStrokesDict()
        {
            foreach (string line in File.ReadAllLines(strokesPath))
            {
                if (line == "")
                    continue;
                string[] token = line.Split("	");
                char character = token[1][0];
                int count;
                if (token[2].Contains(","))
                {
                    count = int.Parse(token[2].Split(",")[0]); //TODO modify
                }
                else
                {
                    count = int.Parse(token[2]);
                }
                if (!strokesDict.ContainsKey(character))
                    strokesDict.Add(character, count);
            }
        }

        public static IEnumerable<Word> SortByFrequency(this IEnumerable<Word> words)
        {
            return words.OrderBy(w => w.Frequency).Reverse();
        }

        public static IEnumerable<Word> SortByStrokesCount(this IEnumerable<Word> words, string writingState)
        {
            return words.OrderBy(w => GetStrokeCount(w, writingState));
        }

        public static IEnumerable<Word> SortByPinyin(this IEnumerable<Word> words)
        {
            return words.OrderBy(w => w.Pinyin);
        }

        public static IEnumerable<Word> SortByExactity(this IEnumerable<Word> words, string text, string language)
        {
            IEnumerable<Word> exactWords;
            IEnumerable<Word> restOfWords;
            if (language == "English")
            {
                exactWords = words.Where(w => w.Definitions.ContainsInsensitive('/' + text + '/'));
            }
            else
            {
                exactWords = words.Where(w => w.Simplified == text);
            }
            restOfWords = words.Except(exactWords);

            foreach (Word w in exactWords)
            {
                yield return w;
            }
            foreach (Word w in restOfWords.SortByFrequency())
            {
                yield return w;
            }
        }

        private static int GetStrokeCount(Word w, string writingState) //modify to use traditional
        {
            int count = 0;
            if (writingState == "Simplified")
            {
                foreach (char c in w.Simplified)
                {
                    if (strokesDict.ContainsKey(c))
                        count += strokesDict[c];
                }
            }
            else
            {
                foreach (char c in w.Traditional)
                {
                    if (strokesDict.ContainsKey(c))
                        count += strokesDict[c];
                }
            }
            return count;
        }

        public static bool ContainsInsensitive(this string source, string value)
        {
            return source?.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        /*
         * Fast but not 100% correct. ( '/' can be used)
         * Or, slow and correct. (and no intermediary results)
         */
        public static IEnumerable<Word> GetEnglishResult(string text)
        {
            //return allWords.AsParallel()
            //   .Where(w => w.Definitions.ContainsInsensitive('/' + text + '/'));

            return allWords.AsParallel()
                           .Where(w => w.Definitions.ContainsInsensitive(text));

            //char[] delims = { ' ', '(', ')', '[', ']', ',', '.', '/'};
            //return allWords.AsParallel()
            //               .Where(w => w.Definitions.Split(delims).Any(tok => tok.ToLower() == text.ToLower()));

            //char[] delims = { ' ', '(', ')', '[', ']', ',', '.', '/'};
            //return allWords.AsParallel()
            //               .Where(w => w.Definitions.Split(delims).Any(tok => tok.ToLower().StartsWith(text.ToLower()) ));
        }

        public static IEnumerable<Word> SearchBySimplified(string text, string writingState)
        {
            if (writingState == "Simplified")
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
                for (int i = 0; i <= prons.Length-1; i++)
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
            for (int i=0; i<200; i++)
            {
                int index = random.Next(allWords.Count);
                result.Add(allWords[index]);
            }
            return result;
        }

        internal static bool IsCharacter(char character, string writingState)
        {
            if (writingState == "Simplified")
            {
                return allWords.Any(w => w.Simplified == character.ToString());
            }
            else
            {
                return allWords.Any(w => w.Traditional == character.ToString());
            }
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

        public static IEnumerable<Word> GetAllWordsFrom(IEnumerable<string> simpList, string writingState)
        {
            if (writingState == "Simplified")
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

        /// <summary>
        /// Simplified (first match) + punctuation.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetSimplifiedWordsFromSentence(string sentence)
        {
            string constructedWord = "";
            string toInsert = string.Empty;
            foreach (char curr in sentence)
            {
                string wordToCheck = constructedWord + curr.ToString();
                if (WordExists(wordToCheck))
                {
                    toInsert = wordToCheck;
                    constructedWord = wordToCheck;
                }
                else
                {
                    if (toInsert.Length != 0)
                    {
                        yield return toInsert;
                    }
                    if (WordExists(curr.ToString()))
                        toInsert = curr.ToString();
                    else
                        toInsert = string.Empty;
                    constructedWord = curr.ToString();
                }
            }
            if (toInsert.Length != 0)
            {
                yield return toInsert;
            }
        }
    }
}
