using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseAppWPF.Logic
{
    public static partial class ChineseService
    {
        public static IEnumerable<Word> SortByFrequency(this IEnumerable<Word> words)
        {
            return words.OrderByDescending(w => w.Frequency);
        }

        public static IEnumerable<Word> SortByStrokesCount(this IEnumerable<Word> words)
        {
            return words.OrderBy(GetStrokeCount);
        }

        public static IEnumerable<Word> SortByPinyin(this IEnumerable<Word> words)
        {
            return words.OrderBy(w => w.Pinyin);
        }

        public static IEnumerable<Word> SortByExactity(this IEnumerable<Word> words, string text, SelectedLanguage language)
        {
            static IEnumerable<Word> GetExactWords(IEnumerable<Word> words, string text, SelectedLanguage language)
            {
                if (language == SelectedLanguage.English)
                {
                    return words.Where(w => w.Definitions.ContainsInsensitive('/' + text + '/'));
                }
                else
                {
                    return words.Where(w => w.Simplified == text); //TODO add traditional
                }
            }
            IEnumerable<Word> exactWords = GetExactWords(words, text, language);
            IEnumerable<Word> restOfWords = words.Except(exactWords);

            foreach (Word w in exactWords)
            {
                yield return w;
            }
            foreach (Word w in restOfWords.SortByFrequency())
            {
                yield return w;
            }
        }

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

    }
}
