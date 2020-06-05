using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChineseAppWPF.Logic
{
    public static partial class ChineseService
    {
        private const string wordsPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\allWords.utf8";
        private const string detailedPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\allDetailedWords.utf8";
        private const string strokesPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\ucs-strokes.txt";

        internal static List<Word> BuildAllWords()
        {
            return File.ReadAllLines(wordsPath)
                       .AsParallel()
                       .Select(GetWordFromLine)
                       .ToList();

            static Word GetWordFromLine(string line)
            {
                string[] tokens = line.Split('\t');

                return new Word
                {
                    Traditional = tokens[0],
                    Simplified = tokens[1],
                    Pinyin = tokens[2],
                    Definitions = tokens[3],
                    Frequency = int.Parse(tokens[4]),
                };
            }
        }

        internal static Dictionary<string, DetailedWord> BuildAllDetailedWords()
        {
            return File.ReadAllLines(detailedPath)
                       .AsParallel()
                       .Select(GetDetailedWordFromLine)
                       .ToDictionary(w => w.Simplified);

            static DetailedWord GetDetailedWordFromLine(string line)
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

        internal static Dictionary<char, int> BuildStrokesDict()
        {
            Dictionary<char, int> dict = new Dictionary<char, int>();
            foreach (string line in File.ReadAllLines(strokesPath))
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                string[] token = line.Split("\t");
                char character = token[1][0];
                int count;
                if (token[2].Contains(","))
                {
                    count = int.Parse(token[2].Split(",")[0]);
                }
                else
                {
                    count = int.Parse(token[2]);
                }
                if (!dict.ContainsKey(character))
                    dict.Add(character, count);
            }
            return dict;
        }

        internal static HashSet<string> BuildAllWordsSet()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (Word word in allWords)
            {
                set.Add(word.Traditional);
                set.Add(word.Simplified);
            }
            return set;
        }

        private static HashSet<char> BuildCharacterSet()
        {
            HashSet<char> set = new HashSet<char>();
            foreach (var word in allWords)
            {
                if (word.Simplified.Length == 1)
                {
                    set.Add(word.Simplified[0]);
                    set.Add(word.Traditional[0]);
                }
            }
            return set;
        }

        private static HashSet<string> BuildOnlyTraditionalSet()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (var word in allWords)
            {
                if (word.Traditional != word.Simplified)
                {
                    set.Add(word.Traditional);
                }
            }
            return set;
        }
    }
}
