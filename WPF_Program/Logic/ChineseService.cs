using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_program.Models;

namespace WPF_program.Logic
{
    public static class WordListExtension
    {
        public static List<Word> SortByFrequency(this IEnumerable<Word> words)
        {
            return words.OrderBy(w => w.Frequency)
                        .Reverse()
                        .ToList();
        }
    }

    public static partial class ChineseService
    {
        const string detailedPath = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\SUBTLEX.utf8";
        const string wordsPath = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\allWords.utf8";

        static List<Word> allWords = new List<Word>();
        static Dictionary<string, DetailedWord> allDetailedWords = new Dictionary<string, DetailedWord>();

        public static void InitializeData()
        {
            BuildAllWords();
            BuildAllDetailedWords();
            Decomposition.BuildDecompositionDict();
        }

        public static List<Word> GetAllWords() => allWords;
        public static Dictionary<string, DetailedWord> GetAllDetailedWords() => allDetailedWords;

        private static void BuildAllWords()
        {
            foreach (string line in File.ReadAllLines(wordsPath))
            {
                var word = getWordFrom(line);
                allWords.Add(word);
            }
            Word getWordFrom(string line)
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
            foreach (string line in File.ReadAllLines(detailedPath))
            {
                var dw = getDetailedWordFrom(line);
                if (!allDetailedWords.ContainsKey(dw.Simplified))
                    allDetailedWords.Add(dw.Simplified, dw);
            }
            DetailedWord getDetailedWordFrom(string line)
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
                    W_CD = tokens[7],
                    W_CD_Percent = tokens[8],
                    Log10CD = tokens[9],
                    DominantPos = tokens[10],
                    DominantPosFreq = tokens[11],
                    AllPos = tokens[12],
                    AllPosFreq = tokens[13],
                    Definition = tokens[14]
                };
            }
        }



        public static List<Word> GetEnglishResult(string text)
        {
            return allWords.Where(w => w.Definitions.Contains(text))
                           .SortByFrequency();
        }

        public static List<Word> SearchBySimplified(string text)
        {
            return allWords.Where(w => w.Simplified.Contains(text))
                           .SortByFrequency();
        }

        public static List<Word> SearchByPinyin(string text)
        {
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
            return allWords.Where(CheckIfPinyinMatches)
                           .SortByFrequency();
        }

        public static List<Word> GetRandomWords()
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

        private static bool WordExists(string simplOfWord)
        {
            return allWords.Any(w => w.Simplified == simplOfWord);
        }

        public static List<Word> GetAllWordsFrom(List<string> simpList)
        {
            List<Word> resultList = new List<Word>();
            foreach (string simp in simpList)
            {
                IEnumerable<Word> words = allWords.Where(w => w.Simplified == simp);
                foreach (Word w in words)
                    resultList.Add(w);
            }
            return resultList;
        }

        //other vesrion?
        public static List<string> GetSimplifiedWordsFromSentence(string sentence)
        {
            List<string> simpList = new List<string>();
            string constructedWord = "";
            string toInsert = "";
            foreach (char curr in sentence)
            {
                Console.WriteLine($"curr = {curr}");
                string wordToCheck = constructedWord + curr.ToString();
                //if (allDetailedWords.ContainsKey(wordToCheck))
                if (WordExists(wordToCheck))
                {
                    toInsert = allDetailedWords[wordToCheck].Simplified;
                    constructedWord = wordToCheck;
                }
                else
                {
                    if (toInsert != "")
                    {
                        simpList.Add(toInsert);
                        toInsert = "";
                    }
                    //if (allDetailedWords.ContainsKey(curr.ToString()))
                    if (WordExists(curr.ToString()))
                        toInsert = curr.ToString();
                    else
                        toInsert = "";
                    constructedWord = curr.ToString(); // ???
                }
            }
            if (toInsert != "")
            {
                simpList.Add(toInsert);
            }
            Console.Write("Added: ");
            //foreach (string simp in simpList)
            //{
            //    Console.Write($"{simp} - ");
                //IEnumerable<Word> words = allWords.Where(w => w.Simplified == simp);
                //Console.Write("added: ");
                //foreach (Word word in words)
                //{
                    //Console.Write($"{word.Simplified} - ");
                //    resultList.Add(word);
                //}
                //Console.WriteLine("");
            //}
            //Console.WriteLine("");
            return simpList;
        }

    }
}
