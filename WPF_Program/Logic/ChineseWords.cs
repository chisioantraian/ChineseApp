using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp2.Logic;

using static MyTypes;

namespace WPF_program.Logic
{
    /// <summary>
    /// Represents an interface to all the info about chinese characters, obtained from different files
    /// </summary>
    internal class ChineseWords
    {
        private readonly List<Word> words;
        private readonly List<DetailedWord> detailedWords;


        public ChineseWords()
        {
            words = Chinese.ChineseService.getAllWords();
            detailedWords = Chinese.ChineseService.getAllDetailedWords().ToList();
        }

        /// <summary>
        /// Sort a list of words by the words frequency in the language
        /// </summary>
        /// <param name="filteredWords">The list to be sorted</param>
        /// <returns>The list sorted</returns>
        private List<Word> GetSortedByFrequency(List<Word> filteredWords)
        {
            return Chinese.ChineseService.getSortedByFrequency(filteredWords).ToList();
        }

        /// <summary>
        /// Get the list of words, where the definition contains the paramater
        /// </summary>
        /// <param name="text">The string to be looked after</param>
        /// <returns>The results list</returns>
        internal List<Word> EnglishResult(string text)
        {
            return Chinese.ChineseService.getEnglishResult(text).ToList();
            List<Word> filteredWords =
                    words
                    .Where(w => w.Definitions.Contains(text))
                    .ToList();
            return GetSortedByFrequency(filteredWords);
        }

        /// <summary>
        /// Get the list of words, where the simplified form contains the paramater
        /// </summary>
        /// <param name="text">The simplified char to be looked after</param>
        /// <returns>The results list</returns>
        internal List<Word> SearchBySimplified(string text)
        {
            var filteredWords = words
                .Where(w => w.Simplified.Contains(text))
                .ToList();

            return GetSortedByFrequency(filteredWords);
        }

        /// <summary>
        /// Get the list of words, where the pronounciation contains the paramater
        /// </summary>
        /// <param name="text">The string representing the pinyin</param>
        /// <returns>The results list</returns>
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

        /// <summary>
        /// Get a list of random words
        /// </summary>
        /// <returns>The computed list</returns>
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

        /// <returns>The list of all words in the application</returns>
        internal List<Word> GetAll()
        {
            return words;
        }

        /// <returns>The list of all (detailed)words in the application. For the moment there are multiple formats..</returns>
        internal List<DetailedWord> GetAllDetailed()
        {
            return detailedWords;
        }

    }
}
