using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Logic
{
    public static class Decomposition
    {
        private const string decompPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\cjk-decomp.txt";
        
        private static Dictionary<string, List<string>> basicDict = new Dictionary<string, List<string>>();

        internal static Dictionary<string, List<string>> GetBasicDict() => basicDict;

        public static void BuildDecompositionDict()
        {
            File.ReadAllLines(decompPath)
                .ToList()
                .ForEach(AnalyzeLine);
            static void AnalyzeLine(string line)
            {
                string toBeDecomposed = line.Split(':')[0];

                string afterParan = line.Split('(')[1];
                List<string> components = new List<string>();

                if (afterParan.Contains(','))
                {
                    int count = Regex.Matches(afterParan, ",").Count;
                    if (count == 1)
                    {
                        components.Add(afterParan.Split(',')[0]);
                        components.Add(afterParan.Split(',')[1]
                                                 .Split(')')[0]);
                    }
                    else
                    {
                        components.Add(afterParan.Split(',')[0]);
                        components.Add(afterParan.Split(',')[1]);
                        components.Add(afterParan.Split(',')[2]
                                                 .Split(')')[0]);
                    }
                }
                else
                {
                    components.Add(afterParan.Split(')')[0]);
                }
                if (!basicDict.ContainsKey(toBeDecomposed))
                    basicDict.Add(toBeDecomposed, components);
            }
        }

        private static bool IsComponentInTree(string character, string component)
        {
            if (character == component)
                return true;

            if (basicDict.ContainsKey(character))
            {
                foreach (string ch in basicDict[character])
                {
                    if (IsComponentInTree(ch, component))
                        return true;
                }
            }

            return false;
        }

        public static IEnumerable<Word> GetCharactersWithComponent(string component, string writingState)
        {
            bool ComputedSimplifiedIsFound(Word w)
            {
                if (writingState == "Simplified")
                    return IsComponentInTree(w.Simplified, component);
                else
                    return IsComponentInTree(w.Traditional, component);
            }

            return ChineseService.GetAllWords()
                                 .AsParallel()
                                 .Where(ComputedSimplifiedIsFound)
                                 .SortByFrequency();
        }
    }
}
