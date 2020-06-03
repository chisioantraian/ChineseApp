using ChineseAppWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseAppWPF.Logic
{
    public static class BreakdownService
    {
        private static Breakdown BreakdownFromString(string simplPOS)
        {
            string[] token = simplPOS.Split("_");
            return new Breakdown { FoundWord = token[0], Annotation = token[1] };
        }

        internal static List<Breakdown> GetTupleListFrom(string line)
        {
            return line.Split('\t')
                       .Select(BreakdownFromString)
                       .ToList();
        }

        // todo remove duplicate code
        public static void ApplyRule(Rule ruleToApply, List<Breakdown> bd, int i)
        {
            switch (ruleToApply)
            {
                case PrevTagRule rule:
                    if (i > 0 && rule.PrevTag == bd[i - 1].Annotation && rule.CurrentWord == bd[i].FoundWord && rule.CurrentTag == bd[i].Annotation)
                    {
                        bd[i].Annotation = rule.DesiredTag;
                    }
                    break;

                case NextTagRule rule:
                    if (i < bd.Count - 1 && rule.NextTag == bd[i + 1].Annotation && rule.CurrentWord == bd[i].FoundWord && rule.CurrentTag == bd[i].Annotation)
                    {
                        bd[i].Annotation = rule.DesiredTag;
                    }
                    break;

                case NextWordRule rule:
                    if (i < bd.Count - 1 && rule.NextWord == bd[i + 1].FoundWord && rule.CurrentWord == bd[i].FoundWord && rule.CurrentTag == bd[i].Annotation)
                    {
                        bd[i].Annotation = rule.DesiredTag;
                    }
                    break;

                case BetweenTagsRule rule:
                    if (i > 0 && i < bd.Count - 1 &&
                        rule.LeftTag == bd[i - 1].Annotation && rule.RightTag == bd[i + 1].Annotation &&
                        rule.CurrentWord == bd[i].FoundWord && rule.CurrentTag == bd[i].Annotation)
                    {
                        bd[i].Annotation = rule.DesiredTag;
                    }
                    break;

                case BetweenWordsRule rule:
                    if ((i > 0) && (i < bd.Count - 1) &&
                        (rule.LeftWord == bd[i - 1].FoundWord) &&
                        (rule.RightWord == bd[i + 1].FoundWord) &&
                        (rule.CurrentTag == bd[i].Annotation))
                    {
                        bd[i].Annotation = rule.DesiredTag;
                    }
                    break;

                case NoMoreVerbsRule rule:
                    bool thereAreVerbs = false;
                    for (int j = i + 1; j < bd.Count; j++)
                    {
                        if (bd[j].Annotation == "v")
                        {
                            thereAreVerbs = true;
                            break;
                        }
                    }
                    if ((rule.CurrentChar == bd[i].FoundWord) && thereAreVerbs)
                    {
                        bd[i].Annotation = rule.DesiredTag;
                    }
                    break;
            }
        }
    }
}
