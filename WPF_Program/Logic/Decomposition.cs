using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ChineseAppWPF.Models;

namespace ChineseAppWPF.Logic
{
    public static class Decomposition
    {
        private const string decompPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\cjk-decomp.txt";
        private readonly static Dictionary<char, List<char>> basicDict = new Dictionary<char, List<char>>();

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

        //todo delete
        public static string DecomposeCharToRadicals(char topChar)
        {
            if (!basicDict.ContainsKey(topChar))
                return "cannot find decomposition";
            if (Kangxi.CheckIfKangxiRadical(topChar))
                return topChar.ToString() + " - Kangxi Radical\n";

            StringBuilder decompositionText = new StringBuilder();
            Queue<char> chars = new Queue<char>();

            foreach (char c in basicDict[topChar])
            {
                decompositionText.Append(c.ToString());
                chars.Enqueue(c);
            }
            decompositionText.Append("\n");

            while (chars.Count > 0)
            {
                char firstChar = chars.Dequeue();
                decompositionText.Append(firstChar.ToString()).Append(" : ");
                if (Kangxi.CheckIfKangxiRadical(firstChar))
                {
                    decompositionText.Append(" - Kangxi Radical\n");
                }
                else if (basicDict.ContainsKey(firstChar))
                {
                    foreach (char c in basicDict[firstChar])
                    {
                        decompositionText.Append(" ").Append(c.ToString());
                        chars.Enqueue(c);
                    }
                    decompositionText.Append("\n");
                }
                else
                {
                    decompositionText.Append(" Stroke / Unencoded\n");
                }
            }
            return decompositionText.ToString();
        }

        public static List<string> DecomposeCharToRadicals_2(char topChar)
        {
            if (!basicDict.ContainsKey(topChar))
                //return "cannot find decomposition";
                return new List<string>();

            if (Kangxi.CheckIfKangxiRadical(topChar))
                //return topChar.ToString() + " - Kangxi Radical\n";
                return new List<string> { topChar.ToString() };

            List<string> result = new List<string> { topChar.ToString() };
            //StringBuilder decompositionText = new StringBuilder();
            Queue<char> chars = new Queue<char>();

            foreach (char c in basicDict[topChar])
            {
                //decompositionText.Append(c.ToString());
                chars.Enqueue(c);
            }
            //decompositionText.Append("\n");

            while (chars.Count > 0)
            {
                char firstChar = chars.Dequeue();
                //decompositionText.Append(firstChar.ToString()).Append(" : ");
                result.Add(firstChar.ToString());
                if (Kangxi.CheckIfKangxiRadical(firstChar))
                {
                    //decompositionText.Append(" - Kangxi Radical\n");
                    ;
                }
                else if (basicDict.ContainsKey(firstChar))
                {
                    foreach (char c in basicDict[firstChar])
                    {
                        //decompositionText.Append(" ").Append(c.ToString());
                        chars.Enqueue(c);
                    }
                    //decompositionText.Append("\n");
                }
                else
                {
                    //decompositionText.Append(" Stroke / Unencoded\n");
                    ;
                }
            }
            //return decompositionText.ToString();
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
