using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChineseAppWPF.Models;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private const string rulesPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\rules_1.utf8";

        private static List<Rule> rules = null;

        public static void InitializeRules()
        {
            rules = File.ReadAllLines(rulesPath)
                        .Select(GetRuleFromLine)
                        .ToList();

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
                    "noMoreVerbs" => new NoMoreVerbsRule { CurrentChar=tokens[1], DesiredTag=tokens[2]},
                    _ => null
                };
            }
        }
    }
}
