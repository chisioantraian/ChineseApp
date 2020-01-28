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
        /// Get the list of words, where the definition contains the paramater
        /// </summary>
        /// <param name="text">The string to be looked after</param>
        /// <returns>The results list</returns>
        internal List<Word> EnglishResult(string text)
        {
            return Chinese.ChineseService.getEnglishResult(text).ToList();
        }

        /// <summary>
        /// Get the list of words, where the simplified form contains the paramater
        /// </summary>
        /// <param name="text">The simplified char to be looked after</param>
        /// <returns>The results list</returns>
        internal List<Word> SearchBySimplified(string text)
        {
            return Chinese.ChineseService.searchBySimplified(text).ToList();
        }

        /// <summary>
        /// Get the list of words, where the pronounciation contains the paramater
        /// </summary>
        /// <param name="text">The string representing the pinyin</param>
        /// <returns>The results list</returns>
        internal List<Word> SearchByPinyin(string text)
        {
            return Chinese.ChineseService.searchByPinyin(text).ToList();
        }

        /// <summary>
        /// Get a list of random words
        /// </summary>
        /// <returns>The computed list</returns>
        internal List<Word> GetRandomWords()
        {
            return Chinese.ChineseService.getRandomWords().ToList();
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
