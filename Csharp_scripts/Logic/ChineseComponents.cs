using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static WpfApp2.Logic.Kangxi;

namespace WpfApp2.Logic
{
    public partial class ChineseService
    {
        private const string inputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\cjk-decomp.txt";

        public class MyTuple
        {
            private string characterToBeDecomposed;
            private string componentA;
            private string componentB;

            public MyTuple(string characterToBeDecomposed, string compositionType, string componentA, string componentB)
            {
                this.characterToBeDecomposed = characterToBeDecomposed;
                CompositionType = compositionType;
                this.componentA = componentA;
                this.componentB = componentB;
            }

            public char ToBeDecomposed { get; set; }
            public string CompositionType { get; set; }
            public char? ComponentA { get; set; }
            public char? ComponentB { get; set; }
        }

        //TODO use singleton
        public static Dictionary<char, List<char>> GetCharacterDecomposition()
        {
            var dict = new Dictionary<char, List<char>>();
            List<KangxiRadical> kangxiRadicals = Kangxi.GetRadicals();
            List<MyTuple> decompositionRules = new List<MyTuple>();

            foreach (string line in File.ReadLines(inputPath).Skip(10640))
            {
                //Console.WriteLine("test " + line);
                //AnalyzeLine(line, dict);
                var tupleFound = Annalyze(line);

                foreach (KangxiRadical radical in kangxiRadicals)
                {
                    if (radical.Symbol == tupleFound.ToBeDecomposed)
                    {
                        tupleFound.CompositionType = "Kangxi";
                        tupleFound.ComponentA = null;
                        tupleFound.ComponentB = null;
                        break;
                    }
                }
                decompositionRules.Add(tupleFound);
            }
            //Console.WriteLine("After read lines & setting kangxi");

            foreach (MyTuple rule in decompositionRules)
            {
                Console.WriteLine(rule.CompositionType);
                var listComponents = new List<char> { rule.ToBeDecomposed };
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
                {
                    dict.Add(rule.ToBeDecomposed, result);
                }
            }

            return dict;
        }

        private static MyTuple Annalyze(string line)
        {
            string characterToBeDecomposed = line.Split(':')[0];
            string compositionType = line.Split(':')[1].Split('(')[0];
            string afterParan = line.Split('(')[1];
            string componentA;
            string? componentB = String.Empty;

            if (afterParan.Contains(","))
            {
                componentA = afterParan.Split(',')[0];
                componentB = afterParan
                    .Split(',')[1]
                    .Split(')')[0];
            }
            else
            {
                componentA = afterParan.Split(')')[0];
            }

            return new MyTuple(characterToBeDecomposed, compositionType, componentA, componentB);
        }

        private static void AnalyzeLine(string line, Dictionary<string, List<string>> dict)
        {
            string character = line.Split(':')[0];
            //string compositionType = line.Split(':')[1].Split('(')[0]
            string afterParan = line.Split('(')[1];
            string componentA;
            string? componentB = String.Empty;
            var components = new List<string>();

            if (afterParan.Contains(","))
            {
                componentA = afterParan.Split(',')[0];
                componentB = afterParan
                    .Split(',')[1]
                    .Split(')')[0];
                components.Add(componentA);
                components.Add(componentB);
            }
            else
            {
                componentA = afterParan.Split(')')[0];
                components.Add(componentA);
            }

            dict.Add(character, components);
        }
    }
}
