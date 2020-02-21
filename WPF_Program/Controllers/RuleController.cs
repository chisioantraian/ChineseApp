using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace ChineseAppWPF.Controllers
{
    public class Rule
    {
        public string Current { get; set; }
        public string Tag1 { get; set; }
        public string Tag2 { get; set; }
        public string Cond { get; set; }
        public string Tag3 { get; set; }
    }

    public static partial class Controller
    {
        private static string rulesPath = @"C:\Users\chisi\Desktop\work\ChineseApp\WPF_Program\Data\rules_1.utf8";

        public static List<Rule> rules = null;

        public static void InitializeRules()
        {
            Console.WriteLine("Initialize rules");
            rules = File.ReadAllLines(rulesPath)
                        .Select(GetRuleFromLine)
                        .ToList();

            foreach (Rule rule in rules)
            {
                Console.WriteLine($"{rule.Current}, {rule.Tag1}, {rule.Tag2}, {rule.Cond}, {rule.Tag3}");
            }
            
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
