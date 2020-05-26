using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Logic
{
    public static class ChineseService
    {
        private const string detailedPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\allDetailedWords.utf8";
        private const string wordsPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\allWords.utf8";
        private const string strokesPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\ucs-strokes.txt";

        private const string savedWordsPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\savedWords";
        private const string savedDetailedPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\savedDetailedWords";
        private const string savedAllWordsSetPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\savedAllWordsSet";
        private const string savedCharacterSetPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\savedCharacterSet";
        private const string savedOnlyTraditionalSetPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\savedOnlyTraditionalSet";

        private static List<Word> allWords = new List<Word>();
        private static Dictionary<string, DetailedWord> allDetailedWords = new Dictionary<string, DetailedWord>();
        private static HashSet<string> wordsSet = new HashSet<string>();                  // words
        private static HashSet<char> characterSet = new HashSet<char>();                  // chars
        private static HashSet<string> onlyTraditionalSet = new HashSet<string>();        // words
        private static Dictionary<char, int> strokesDict = new Dictionary<char, int>();

        public static void InitializeData()
        {
            //BuildAllWords();
            BuildAllWords_FromSerialized();

            //BuildAllDetailedWords();
            BuildAllDetailedWords_FromSerialized();

            //BuildAllWordsSet();
            BuildAllWordsSet_FromSerialized();

            //BuildCharacterSet();
            BuildCharacterSet_FromSerialized();

            //BuildOnlyTraditionalSet();
            BuildOnlyTraditionalSet_FromSerialized();

            BuildStrokesDict();
            //BuildStrokesDict_FromSerialized();            
        }

        public static List<Word> GetAllWords() => allWords;

        public static Dictionary<string, DetailedWord> GetAllDetailedWords() => allDetailedWords;

        internal static void SaveWordsToFile()
        {
            //using (Stream stream = File.Open(savedWordsPath, FileMode.Create))
            //{
            //    var bformatter = new BinaryFormatter();
            //    bformatter.Serialize(stream, allWords);
            //}

            //using (Stream stream = File.Open(savedDetailedPath, FileMode.Create))
            //{
            //    var bformatter = new BinaryFormatter();
            //    bformatter.Serialize(stream, allDetailedWords);
            //}

            //using (Stream stream = File.Open(savedAllWordsSetPath, FileMode.Create))
            //{
            //    var bformatter = new BinaryFormatter();
            //    bformatter.Serialize(stream, wordsSet);
            //}

            //using (Stream stream = File.Open(savedCharacterSetPath, FileMode.Create))
            //{
            //    var bformatter = new BinaryFormatter();
            //    bformatter.Serialize(stream, characterSet);
            //}

            //using (Stream stream = File.Open(savedOnlyTraditionalSetPath, FileMode.Create))
            //{
            //    var bformatter = new BinaryFormatter();
            //    bformatter.Serialize(stream, onlyTraditionalSet);
            //}

            //using (Stream stream = File.Open(savedStrokesDictPath, FileMode.Create))
            //{
            //    var bformatter = new BinaryFormatter();
            //    bformatter.Serialize(stream, strokesDict);
            //}
        }

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
                    Frequency = int.Parse(tokens[4]),
                };
            }
        }

        private static void BuildAllWords_FromSerialized()
        {
            using (Stream stream = File.Open(savedWordsPath, FileMode.Open))
            {
                var bformatter = new BinaryFormatter();
                allWords = (List<Word>)bformatter.Deserialize(stream);
            }
        }

        private static void BuildAllDetailedWords_FromSerialized()
        {
            using (Stream stream = File.Open(savedDetailedPath, FileMode.Open))
            {
                var bformatter = new BinaryFormatter();
                allDetailedWords = (Dictionary<string, DetailedWord>)bformatter.Deserialize(stream);
            }
        }

        private static void BuildAllWordsSet_FromSerialized()
        {
            using (Stream stream = File.Open(savedAllWordsSetPath, FileMode.Open))
            {
                var bformatter = new BinaryFormatter();
                wordsSet = (HashSet<string>)bformatter.Deserialize(stream);
            }
        }

        private static void BuildCharacterSet_FromSerialized()
        {
            using (Stream stream = File.Open(savedCharacterSetPath, FileMode.Open))
            {
                var bformatter = new BinaryFormatter();
                characterSet = (HashSet<char>)bformatter.Deserialize(stream);
            }
        }

        private static void BuildOnlyTraditionalSet_FromSerialized()
        {
            using (Stream stream = File.Open(savedOnlyTraditionalSetPath, FileMode.Open))
            {
                var bformatter = new BinaryFormatter();
                onlyTraditionalSet = (HashSet<string>)bformatter.Deserialize(stream);
            }
        }

        private static void BuildStrokesDict_FromSerialized()
        {
            using (Stream stream = File.Open(savedOnlyTraditionalSetPath, FileMode.Open))
            {
                var bformatter = new BinaryFormatter();
                strokesDict = (Dictionary<char, int>)bformatter.Deserialize(stream);
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
                wordsSet.Add(word.Simplified);
            }
        }

        private static void BuildCharacterSet()
        {
            foreach (var word in allWords)
            {
                if (word.Simplified.Length == 1)
                {
                    characterSet.Add(word.Simplified[0]);
                    characterSet.Add(word.Traditional[0]);
                }
            }
        }

        private static void BuildOnlyTraditionalSet()
        {
            foreach (var word in allWords)
            {
                if (word.Traditional != word.Simplified)
                {
                    onlyTraditionalSet.Add(word.Traditional);
                }
            }
        }



        private static void BuildStrokesDict()
        {
            foreach (string line in File.ReadAllLines(strokesPath))
            {
                if (string.IsNullOrEmpty(line))
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
            return words.OrderByDescending(w => w.Frequency);
        }

        public static IEnumerable<Word> SortByStrokesCount(this IEnumerable<Word> words)
        {
            return words.OrderBy(w => GetStrokeCount(w));
        }

        public static IEnumerable<Word> SortByPinyin(this IEnumerable<Word> words)
        {
            return words.OrderBy(w => w.Pinyin);
        }

        public static IEnumerable<Word> SortByExactity(this IEnumerable<Word> words, string text, SelectedLanguage language)
        {
            IEnumerable<Word> exactWords;
            IEnumerable<Word> restOfWords;

            if (language == SelectedLanguage.English)
            {
                exactWords = words.Where(w => w.Definitions.ContainsInsensitive('/' + text + '/'));
            }
            else
            {
                exactWords = words.Where(w => w.Simplified == text); //TODO add traditional
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

        //TODO modify to use traditional
        //TODO add new filed to file, with strokecount
        private static int GetStrokeCount(Word w)
        {
            int count = 0;

            foreach (char c in w.Simplified)
            {
                if (strokesDict.ContainsKey(c))
                    count += strokesDict[c];
            }
            
            return count;
        }

        public static bool ContainsInsensitive(this string source, string value)
        {
            return source?.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private static bool WordFitsCriteria(Word w, string text)
        {
            char[] sep = new char[] { ' ', '(', '/' };
            List<string> tokens = w.Definitions.Split(sep).ToList();
            return tokens.Any(t => t.StartsWith(text));
        }

        public static IEnumerable<Word> GetEnglishResult(string text)
        {
            return allWords.AsParallel()
                           .Where(w => w.Definitions.ContainsInsensitive(text));
            //return allWords.AsParallel()
            //               .Where(w => WordFitsCriteria(w, text))
            //               .ToList();
        }

        public static IEnumerable<Word> SearchBySimplified(string text)
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



        //TODO improve algorithm
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
            for (int i=0; i<20; i++)
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
                       where w.Simplified == simp // || w.Traditional == simp ?
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
                List<Word> intermediate = new List<Word>();
                for (int j = i+1; j <= simplified.Length; j++)
                {
                    string possible = simplified[i..j];
                    if (WordExists(possible))
                    {
                        //result.AddRange(GetWordsWithSimplified(possible));
                        GetWordsWithSimplified(possible).ForEach(w => intermediate.Add(w));
                    }
                }
                //intermediate.Reverse();
                intermediate.ForEach(w => result.Add(w));
                //result.AddRange(intermediate);
            }
            result.Reverse();
            return result.OrderBy(w => w.Simplified.Length).Reverse().ToList();
        }
    }
}
