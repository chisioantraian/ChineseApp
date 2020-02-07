using System.Collections.Generic;
using WpfApp2.Models;

namespace WpfApp2.Logic
{
    public static class ChineseService
    {
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

    }
}
