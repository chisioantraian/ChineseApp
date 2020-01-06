using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1_csharp.Logic
{
    public class ReadWrite
    {
        public static void Main2(string[] args)
        {
            string inputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\cjk-decomp.txt";
            string outputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\output.txt";
            var dict = new Dictionary<string, List<string>>();

            foreach (string line in File.ReadLines(inputPath).Skip(10640))
            {
                AnalyzeLine(line, dict);
            }

            PrintWordsToFile(outputPath, dict);
        }

        private static void AnalyzeLine(string line, Dictionary<string, List<string>> dict)
        {
            string character = line.Split(':')[0];
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

        private static void PrintWordsToFile(string outputPath, Dictionary<string, List<string>> dict)
        {
            using var outputFile = new StreamWriter(outputPath); //TODO
            foreach (var word in dict)
            {
                outputFile.Write(word.Key + " :-: ");
                foreach (var component in word.Value)
                {
                    outputFile.Write(component + ' ');
                }
                outputFile.Write('\n');
            }
        }
    }
}

