using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_program.Models;

namespace WPF_program.Logic
{
    public static partial class Decomposition
    {
        const string decompPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\cjk-decomp.txt";
        static Dictionary<char, List<char>> basicDict = new Dictionary<char, List<char>>();

        public static void BuildDecompositionDict()
        {
            File.ReadAllLines(decompPath)
                .Skip(10640)
                .ToList()
                .ForEach(AnalyzeLine);
            void AnalyzeLine(string line)
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
                    components.Add(afterParan.Split(')')[0][0]);
                if (!basicDict.ContainsKey(character))
                    basicDict.Add(character, components);
            }
        }

        public static Dictionary<char, List<char>> GetCharacterDecomposition()
        {
            var resultDict = new Dictionary<char, List<char>>();
            var kangxiRadicals = Kangxi.GetRadicals();

            foreach (char character in basicDict.Keys)
            {
                bool foundRadical = false;
                foreach (KangxiRadical radical in kangxiRadicals)
                    if (radical.Symbol == character)
                        foundRadical = true;
                if (!foundRadical)
                    resultDict.Add(character, basicDict[character]);
            }
            return resultDict;
        }

        public static string DecomposeCharToRadicals(char topChar)
        {
            if (!basicDict.ContainsKey(topChar))
                return "cannot find decomposition";
            string decompositionText = "";
            Queue<char> chars = new Queue<char>();
            if (Kangxi.CheckIfKangxiRadical(topChar))
                decompositionText = topChar.ToString() + " - Kangxi Radical\n";
            else
            {
                foreach (char c in basicDict[topChar])
                {
                    decompositionText += " " + c.ToString();
                    chars.Enqueue(c);
                }
                decompositionText += "\n";
            }
            while (chars.Count > 0)
            {
                char firstChar = chars.Dequeue();
                decompositionText += firstChar.ToString() + " : ";
                if (Kangxi.CheckIfKangxiRadical(firstChar))
                    decompositionText += " - Kangxi Radical\n";
                else if (basicDict.ContainsKey(firstChar))
                {
                    foreach (char c in basicDict[firstChar])
                    {
                        decompositionText += " " + c.ToString();
                        chars.Enqueue(c);
                    }
                    decompositionText += "\n";
                }
                else
                    decompositionText += " Stroke / Unencoded\n";
            }

            return decompositionText;
        }

        public static List<Word> GetCharactersWithComponent(string text)
        {
            char ch = text[0];
            List<char> simplifiedComponentsFound = new List<char>();
            foreach (var decompositionTuple in basicDict)
            {
                List<char> componentList = decompositionTuple.Value;
                if (componentList.Contains(ch))
                    simplifiedComponentsFound.Add(decompositionTuple.Key);
            }
            List<Word> filteredWords = new List<Word>();
            foreach (Word word in ChineseService.GetAllWords())
            {
                if (word.Simplified.Length == 1)
                    if (simplifiedComponentsFound.Contains(word.Simplified[0]))
                        filteredWords.Add(word);
            }
            return filteredWords;
        }

    }
}
