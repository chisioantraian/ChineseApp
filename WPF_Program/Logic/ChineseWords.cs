using CSharp_scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp2.Logic;
using WpfApp2.Models;

namespace WPF_program.Logic
{
    internal class ChineseWords
    {
        private readonly List<Word> words;
        private readonly List<DetailedWord> detailedWords;

        public ChineseWords()
        {
            words = ChineseService.GetWordsFromDatabase();
            detailedWords = ChineseService.GetDetailedWords();
        }

        private List<Word> GetSortedByFrequency(List<Word> filteredWords)
        {
            foreach (Word w in filteredWords)
            {
                foreach (DetailedWord dw in detailedWords)
                {
                    if (w.Simplified == dw.Simplified)
                    {
                        w.Rank = Int32.Parse(dw.Rank);
                        break;
                    }
                }
            }
            return filteredWords.OrderBy(w => w.Rank).ToList();
        }

        internal List<Word> EnglishResult(string text)
        {
            List<Word> filteredWords =
                    words
                    .Where(w => w.Definitions.Contains(text))
                    .ToList();
            return GetSortedByFrequency(filteredWords);
        }

        internal List<Word> SearchBySimplified(string text)
        {
            var filteredWords = words
                .Where(w => w.Simplified.Contains(text))
                .ToList();

            return GetSortedByFrequency(filteredWords);
        }

        internal List<Word> SearchByPinyin(string text)
        {
            string[] prons = text.Split(' ');
            List<Word> filteredWords = new List<Word>();
            foreach (var word in words)
            {
                string[] wordProns = word.Pronounciation.Split(' ');
                if (prons.Length != wordProns.Length)
                    continue;
                bool toInsert = true;
                for (int i = 0; i < prons.Length; i++)
                {
                    if (!wordProns[i].StartsWith(prons[i]))
                        toInsert = false;
                    if (wordProns[i].Length != (prons[i].Length + 1))
                        toInsert = false;
                }
                if (toInsert)
                    filteredWords.Add(word);
            }

            return GetSortedByFrequency(filteredWords);
        }

        internal List<Word> GetRandomWords()
        {
            Random random = new Random();
            List<Word> result = new List<Word>();
            for (int i = 0; i < 5; i++)
            {
                int index = random.Next(words.Count);
                result.Add(words[index]);
            }
            return result;
        }

        internal List<Word> GetAll()
        {
            return words;
        }

        internal List<DetailedWord> GetAllDetailed()
        {
            return detailedWords;
        }

    }
}
