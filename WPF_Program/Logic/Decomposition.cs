using System.Collections.Generic;
using System.IO;
using System.Linq;

using ChineseAppWPF.Models;

namespace ChineseAppWPF.Logic
{
    public static class Decomposition
    {
        private const string decompPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\cjk-decomp.txt";
        private static Dictionary<char, List<char>> basicDict = new Dictionary<char, List<char>>();

        internal static Dictionary<char, List<char>> GetBasicDict() => basicDict;

        public static void BuildDecompositionDict()
        {
            File.ReadAllLines(decompPath)
                .Skip(10640)
                .ToList()
                .ForEach(AnalyzeLine);
            static void AnalyzeLine(string line)
            {
                char character = line.Split(':')[0][0];
                string afterParan = line.Split('(')[1];
                List<char> components = new List<char>();

                if (afterParan.Contains(','))
                {
                    components.Add(afterParan.Split(',')[0][0]);
                    components.Add(afterParan.Split(',')[1]
                                             .Split(')')[0][0]);
                }
                else
                {
                    components.Add(afterParan.Split(')')[0][0]);
                }
                if (!basicDict.ContainsKey(character))
                    basicDict.Add(character, components);
            }
        }


        public static List<string> DecomposeCharToRadicals(char topChar)
        {
            if (!basicDict.ContainsKey(topChar))
                return new List<string>();

            if (Kangxi.CheckIfKangxiRadical(topChar))
                return new List<string> { topChar.ToString() };

            List<string> result = new List<string> { topChar.ToString() };
            Queue<char> chars = new Queue<char>();

            foreach (char c in basicDict[topChar])
            {
                chars.Enqueue(c);
            }

            while (chars.Count > 0)
            {
                char firstChar = chars.Dequeue();
                result.Add(firstChar.ToString());

                if (Kangxi.CheckIfKangxiRadical(firstChar))
                {
                }
                else if (basicDict.ContainsKey(firstChar))
                {
                    foreach (char c in basicDict[firstChar])
                    {
                        chars.Enqueue(c);
                    }
                }
                else
                {
                    //decompositionText.Append(" Stroke / Unencoded\n");
                }
            }
            return result;
        }

        public static IEnumerable<Word> GetCharactersWithComponent(string text)
        {
            char ch = text[0];
            var simplifiedComponentsFound = basicDict.Where(dTuple => dTuple.Value.Contains(ch))
                                                     .ToDictionary(dTuple => dTuple.Key);
            bool ComputedSimplifiedIsFound(Word w) => w.Simplified.Length == 1 &&
                                                      simplifiedComponentsFound.ContainsKey(w.Simplified[0]);
            return ChineseService.GetAllWords()
                                 .AsParallel()
                                 .Where(ComputedSimplifiedIsFound);
        }
    }
}
