using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChineseAppWPF.Logic
{
    public static class RuleService
    {
        private const string rulesPath = @"C:\Users\chisi\source\repos\chisioantraian\ChineseApp\WPF_Program\Data\rules_1.utf8";

        private static readonly List<Rule> rules = BuildRules();

        public static List<Rule> GetRules() => rules;

        private static List<Rule> BuildRules()
        {
            return File.ReadAllLines(rulesPath)
                       .Select(GetRuleFromLine)
                       .ToList();


        }

        private static Rule GetRuleFromLine(string line)
        {
            string[] tokens = line.Split('\t');

            return tokens[0] switch
            {
                "prevTag" => new PrevTagRule(tokens), //{ PrevTag = tokens[1], CurrentWord = tokens[2], CurrentTag = tokens[3], DesiredTag = tokens[4] },
                "nextTag" => new NextTagRule(tokens),// { NextTag = tokens[1], CurrentWord = tokens[2], CurrentTag = tokens[3], DesiredTag = tokens[4] },
                "nextWord" => new NextWordRule(tokens),// { NextWord = tokens[1], CurrentWord = tokens[2], CurrentTag = tokens[3], DesiredTag = tokens[4] },
                "betweenTags" => new BetweenTagsRule(tokens),// { LeftTag = tokens[1], RightTag = tokens[2], CurrentWord = tokens[3], CurrentTag = tokens[4], DesiredTag = tokens[5] },
                "betweenWords" => new BetweenWordsRule(tokens),// { LeftWord = tokens[1], RightWord = tokens[2], CurrentTag = tokens[3], DesiredTag = tokens[4] },
                "noMoreVerbs" => new NoMoreVerbsRule(tokens),// { CurrentChar = tokens[1], DesiredTag = tokens[2] },
                _ => null
            };
        }
    }
}
