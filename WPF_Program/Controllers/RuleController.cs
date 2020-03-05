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

                return tokens[0] switch
                {
                    "prevTag" => new PrevTagRule { PrevTag=tokens[1], CurrentWord=tokens[2], CurrentTag=tokens[3], DesiredTag=tokens[4]},
                    "nextTag" => new NextTagRule { NextTag=tokens[1], CurrentWord=tokens[2], CurrentTag=tokens[3], DesiredTag=tokens[4]},
                    "nextWord" => new NextWordRule { NextWord=tokens[1], CurrentWord=tokens[2], CurrentTag=tokens[3], DesiredTag=tokens[4]},
                    "betweenTags" => new BetweenTagsRule { LeftTag=tokens[1], RightTag=tokens[2], CurrentWord=tokens[3], CurrentTag=tokens[4], DesiredTag=tokens[5]},
                    "betweenWords" => new BetweenWordsRule { LeftWord=tokens[1], RightWord=tokens[2], CurrentTag=tokens[3], DesiredTag=tokens[4]},
                    _ => null
                };
                /*return new Rule
                {
                    Current = tokens[0],
                    Tag1 = tokens[1],
                    Tag2 = tokens[2],
                    Cond = tokens[3],
                    Tag3 = tokens[4]
                };*/
            }
        }
    }
}
