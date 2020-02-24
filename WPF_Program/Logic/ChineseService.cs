using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ChineseAppWPF.Models;

namespace ChineseAppWPF.Logic
{
    public static class ChineseService
    {
        private const string detailedPath = @"C:\Users\chisi\Desktop\work\ChineseApp\WPF_Program\Data\allDetailedWords.utf8";
        private const string wordsPath = @"C:\Users\chisi\Desktop\work\ChineseApp\WPF_Program\Data\allWords.utf8";

        private static List<Word> allWords = new List<Word>();
        private static Dictionary<string, DetailedWord> allDetailedWords = new Dictionary<string, DetailedWord>();

        public static void InitializeData()
        {
            BuildAllWords();
            BuildAllDetailedWords();
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

        public static List<Word> SortByFrequency(this IEnumerable<Word> words) =>
            words.OrderBy(w => w.Frequency)
                 .Reverse()
                 .ToList();

        public static List<Word> GetEnglishResult(string text) =>
            allWords.AsParallel()
                    .Where(w => w.Definitions.Contains(text))
                    .SortByFrequency();

        public static List<Word> SearchBySimplified(string text)
        {
            return allWords.AsParallel()
                           .Where(w => w.Simplified.Contains(text))
                           .SortByFrequency();
        }

        public static List<Word> SearchByPinyin(string text)
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
                           .Where(CheckIfPinyinMatches)
                           .SortByFrequency();
        }

        public static List<Word> GetRandomWords()
        {
            Random random = new Random();
            List<Word> result = new List<Word>();
            for (int i=0; i<200; i++)
            {
                int index = random.Next(allWords.Count);
                result.Add(allWords[index]);
            }
            return result.SortByFrequency();
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
            return allWords.Any(w => w.Simplified == simplOfWord);
        }

        public static List<Word> GetAllWordsFrom(List<string> simpList)
        {
            //return simpList.SelectMany(simp => allWords.Where(w => w.Simplified == simp))
            //               .ToList();
            return (from simp in simpList
                   from w in allWords
                   where w.Simplified == simp
                   select w).ToList();
        }

        /// <summary>
        /// Simplified + punctuation
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static List<string> GetSimplifiedWordsFromSentence(string sentence)
        {
            //Console.WriteLine($"GetSimplifiedWordsFromSentence: {sentence}");
            List<string> simpList = new List<string>();
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
                        simpList.Add(toInsert);
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
                simpList.Add(toInsert);
            }
            return simpList;
        }
    }
}
