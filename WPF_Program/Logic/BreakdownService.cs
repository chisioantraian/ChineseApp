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
            return new Breakdown { Part = token[0], Description = token[1] };
        }

        internal static List<Breakdown> GetTupleListFrom(string line)
        {
            return line.Split('\t')
                       .Select(BreakdownFromString)
                       .ToList();
        }

        // todo remove duplicate code
        public static void ApplyRule(Rule _rule, List<Breakdown> bd, int i)
        {
            switch (_rule)
            {
                case PrevTagRule rule:
                    if (i > 0 && rule.PrevTag == bd[i - 1].Description && rule.CurrentWord == bd[i].Part && rule.CurrentTag == bd[i].Description)
                        bd[i].Description = rule.DesiredTag;
                    break;

                case NextTagRule rule:
                    if (i < bd.Count - 1 && rule.NextTag == bd[i + 1].Description && rule.CurrentWord == bd[i].Part && rule.CurrentTag == bd[i].Description)
                        bd[i].Description = rule.DesiredTag;
                    break;

                case NextWordRule rule:
                    if (i < bd.Count - 1 && rule.NextWord == bd[i + 1].Part && rule.CurrentWord == bd[i].Part && rule.CurrentTag == bd[i].Description)
                        bd[i].Description = rule.DesiredTag;
                    break;

                case BetweenTagsRule rule:
                    if (i > 0 && i < bd.Count - 1 &&
                        rule.LeftTag == bd[i - 1].Description && rule.RightTag == bd[i + 1].Description &&
                        rule.CurrentWord == bd[i].Part && rule.CurrentTag == bd[i].Description)
                        bd[i].Description = rule.DesiredTag;
                    break;

                case BetweenWordsRule rule:
                    if ((i > 0) && (i < bd.Count - 1) &&
                        (rule.LeftWord == bd[i - 1].Part) && 
                        (rule.RightWord == bd[i + 1].Part) && 
                        (rule.CurrentTag == bd[i].Description)) //.Part
                    {
                        bd[i].Description = rule.DesiredTag;
                    }
                    break;
            }
        }
    }
}
