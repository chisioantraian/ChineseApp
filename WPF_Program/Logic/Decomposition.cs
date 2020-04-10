using System.Collections.Generic;
using System.IO;
using System.Linq;

using ChineseAppWPF.Models;

namespace ChineseAppWPF.Logic
{
    public static class Decomposition
    {
        private const string decompPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\cjk-decomp.txt";
        
        //private static Dictionary<char, List<char>> basicDict = new Dictionary<char, List<char>>();
        private static Dictionary<string, List<string>> basicDict = new Dictionary<string, List<string>>();

        internal static Dictionary<string, List<string>> GetBasicDict() => basicDict;

        public static void BuildDecompositionDict()
        {
            File.ReadAllLines(decompPath)
                //.Skip(10640)
                .ToList()
                .ForEach(AnalyzeLine);
            static void AnalyzeLine(string line)
            {
                //char character = line.Split(':')[0][0];
                string toBeDecomposed = line.Split(':')[0];

                string afterParan = line.Split('(')[1];
                List<string> components = new List<string>();

                if (afterParan.Contains(','))
                {
                    //components.Add(afterParan.Split(',')[0][0]);
                    components.Add(afterParan.Split(',')[0]);
                    //components.Add(afterParan.Split(',')[1]
                    //                         .Split(')')[0][0]);
                    components.Add(afterParan.Split(',')[1]
                                             .Split(')')[0]);
                }
                else
                {
                    components.Add(afterParan.Split(')')[0]);
                }
                if (!basicDict.ContainsKey(toBeDecomposed))
                    basicDict.Add(toBeDecomposed, components);
            }
        }

        public static IEnumerable<Word> GetCharactersWithComponent(string component)
        {
            char ch = component[0];
            var simplifiedComponentsFound = basicDict.Where(dTuple => dTuple.Value.Contains(ch.ToString()))
                                                     .ToDictionary(dTuple => dTuple.Key);
            bool ComputedSimplifiedIsFound(Word w) => w.Simplified.Length == 1 &&
                                                      simplifiedComponentsFound.ContainsKey(w.Simplified[0].ToString());
            return ChineseService.GetAllWords()
                                 .AsParallel()
                                 .Where(ComputedSimplifiedIsFound);
        }
    }
}
