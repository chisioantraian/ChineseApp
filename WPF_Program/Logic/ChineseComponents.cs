using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpfApp2.Models;
//using static WpfApp2.Logic.Kangxi;

namespace WpfApp2.Logic
{
    public static partial class ChineseService
    {
        //TODO use relative paths
        private const string inputPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\cjk-decomp.txt";

        //TODO use singleton
        //TODO comment
        public static Dictionary<char, List<char>> GetCharacterDecomposition()
        {
            Dictionary<char, List<char>> basicDict = GetDecompositionRules();
            Dictionary<char, List<char>> resultDict = new Dictionary<char, List<char>>();
            List<MyTypes.KangxiRadical> kangxiRadicals = Chinese.Kangxi.getRadicals().ToList();//Kangxi.GetRadicals();

            foreach (char dictKey in basicDict.Keys)
            {
                bool foundRadical = false;
                foreach (MyTypes.KangxiRadical kg in kangxiRadicals)
                {
                    if (kg.Symbol == dictKey)
                    {
                        foundRadical = true;
                        break;
                    }
                }

                if (! foundRadical)
                {
                    resultDict.Add(dictKey, basicDict[dictKey]);
                }
                else
                {
                    resultDict.Add(dictKey, null);
                }
            }

            return resultDict;
        }

        private static void ProcessRule(Rule rule, Dictionary<char, List<char>> dict, List<Rule> decompositionRules)
        {
            var listComponents = new List<char>();
            if (rule.ComponentA != null)
                listComponents.Add((char)rule.ComponentA);
            if (rule.ComponentB != null)
                listComponents.Add((char)rule.ComponentB);

            var result = new List<char>();

            while (listComponents.Count > 0)
            {
                char frontChar = listComponents[0];
                listComponents.RemoveAt(0);

                foreach (var radical in decompositionRules)
                {
                    if (radical.ToBeDecomposed == frontChar)
                    {
                        result.Add(radical.ToBeDecomposed);
                        if (radical.CompositionType == "Kangxi")
                            break;
                        if (radical.ComponentA != null)
                            listComponents.Insert(0, (char)radical.ComponentA);
                        if (radical.ComponentB != null)
                            listComponents.Insert(0, (char)radical.ComponentB);
                    }
                }
            }

            if (! dict.ContainsKey(rule.ToBeDecomposed))
                dict.Add(rule.ToBeDecomposed, result);
        }

        private static Dictionary<char, List<char>> GetDecompositionRules()
        {
            Dictionary<char, List<char>> basicDict = new Dictionary<char, List<char>>();

            // components -> radicals decomposition list
            foreach (string line in File.ReadLines(inputPath).Skip(10640))
            {
                AnalyzeLine(line, basicDict);
            }
            return basicDict;
        }

        private static Rule Annalyze(string line)
        {
            char characterToBeDecomposed = line.Split(':')[0][0];
            string compositionType = line.Split(':')[1].Split('(')[0];
            string afterParan = line.Split('(')[1];
            char componentA;
            char? componentB = null;

            if (afterParan.Contains(","))
            {
                componentA = afterParan.Split(',')[0][0];
                componentB = afterParan
                    .Split(',')[1]
                    .Split(')')[0][0];
            }
            else
            {
                componentA = afterParan.Split(')')[0][0];
            }

            return new Rule(characterToBeDecomposed, compositionType, componentA, componentB);
        }

        private static void AnalyzeLine(string line, Dictionary<char, List<char>> dict)
        {
            char character = line.Split(':')[0][0];
            //string compositionType = line.Split(':')[1].Split('(')[0]
            string afterParan = line.Split('(')[1];
            char componentA;
            char? componentB = null;
            var components = new List<char>();

            if (afterParan.Contains(","))
            {
                componentA = afterParan.Split(',')[0][0];
                componentB = afterParan
                    .Split(',')[1]
                    .Split(')')[0][0];
                components.Add(componentA);
                components.Add((char)componentB);
            }
            else
            {
                componentA = afterParan.Split(')')[0][0];
                components.Add(componentA);
            }
            if (! dict.ContainsKey(character))
                dict.Add(character, components);
        }
    }
}
