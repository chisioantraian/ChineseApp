using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static string rulesPath = @"C:\Users\chisi\Desktop\work\ChineseApp\WPF_Program\Data\rules_1.utf8";

        public static List<Rule> rules = null;

        public static void InitializeRules()
        {
            rules = File.ReadAllLines(rulesPath)
                        .Select(GetRuleFromLine)
                        .ToList();

            Console.WriteLine("Finished reading rules from file");

            static Rule GetRuleFromLine(string line)
            {
                string[] tokens = line.Split('\t');
                return new Rule
                {
                    Current = tokens[0],
                    Tag1 = tokens[1],
                    Tag2 = tokens[2],
                    Cond = tokens[3],
                    Tag3 = tokens[4]
                };
            }
        }
    }
}
