
using System.Collections.Generic;
using System.Windows.Documents;

namespace ChineseAppWPF.Models
{
    public abstract class Rule
    {
        public abstract void ApplyRule(List<Breakdown> bd, int i);
    }

    public class NextTagRule : Rule
    {
        public string NextTag { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }

        public NextTagRule(string[] tokens)
        {
            NextTag = tokens[1]; 
            CurrentWord = tokens[2]; 
            CurrentTag = tokens[3]; 
            DesiredTag = tokens[4];
        }

        public override void ApplyRule(List<Breakdown> bd, int i)
        {
            if (i < bd.Count - 1 &&
                NextTag == bd[i + 1].Annotation &&
                CurrentWord == bd[i].FoundWord &&
                CurrentTag == bd[i].Annotation)
            {
                bd[i].Annotation = DesiredTag;
            }
        }
    }

    public class PrevTagRule : Rule
    {
        public string PrevTag { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }

        public PrevTagRule(string[] tokens)
        {
            PrevTag = tokens[1]; 
            CurrentWord = tokens[2]; 
            CurrentTag = tokens[3]; 
            DesiredTag = tokens[4];
        }

        public override void ApplyRule(List<Breakdown> bd, int i)
        {
            if (i > 0 &&
                PrevTag == bd[i - 1].Annotation && 
                CurrentWord == bd[i].FoundWord && 
                CurrentTag == bd[i].Annotation)
            {
                bd[i].Annotation = DesiredTag;
            }
        }
    }

    public class NextWordRule : Rule
    {
        public string NextWord { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }

        public NextWordRule(string[] tokens)
        {
            NextWord = tokens[1];
            CurrentWord = tokens[2];
            CurrentTag = tokens[3];
            DesiredTag = tokens[4];
        }

        public override void ApplyRule(List<Breakdown> bd, int i)
        {
            if (i < bd.Count - 1 &&
                NextWord == bd[i + 1].FoundWord &&
                CurrentWord == bd[i].FoundWord &&
                CurrentTag == bd[i].Annotation)
            {
                bd[i].Annotation = DesiredTag;
            }
        }
    }

    public class BetweenTagsRule : Rule
    {
        public string LeftTag { get; set; }
        public string RightTag { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }

        public BetweenTagsRule(string[] tokens)
        {
            LeftTag = tokens[1];
            RightTag = tokens[2];
            CurrentWord = tokens[3];
            CurrentTag = tokens[4];
            DesiredTag = tokens[5];
        }

        public override void ApplyRule(List<Breakdown> bd, int i)
        {
            if ((i > 0) && (i < bd.Count - 1) &&
                LeftTag == bd[i - 1].Annotation &&
                RightTag == bd[i + 1].Annotation &&
                CurrentWord == bd[i].FoundWord &&
                CurrentTag == bd[i].Annotation)
            {
                bd[i].Annotation = DesiredTag;
            }
        }
    }

    public class BetweenWordsRule : Rule
    {
        public string LeftWord { get; set; }
        public string RightWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }

        public BetweenWordsRule(string[] tokens)
        {
            LeftWord = tokens[1]; 
            RightWord = tokens[2]; 
            CurrentTag = tokens[3]; 
            DesiredTag = tokens[4];
        }

        public override void ApplyRule(List<Breakdown> bd, int i)
        {
            if ((i > 0) && (i < bd.Count - 1) &&
                 LeftWord == bd[i - 1].FoundWord &&
                 RightWord == bd[i + 1].FoundWord &&
                 CurrentTag == bd[i].Annotation)
            {
                bd[i].Annotation = DesiredTag;
            }
        }
    }

    public class NoMoreVerbsRule : Rule
    {
        public string CurrentChar { get; set; }
        public string DesiredTag { get; set; }

        public NoMoreVerbsRule(string[] tokens)
        {
            CurrentChar = tokens[1];
            DesiredTag = tokens[2];
        }
        public override void ApplyRule(List<Breakdown> bd, int i)
        {
            bool thereAreVerbs = false;
            for (int j = i + 1; j < bd.Count; j++)
            {
                if (bd[j].Annotation == "v")
                {
                    thereAreVerbs = true;
                    break;
                }
            }
            if ((CurrentChar == bd[i].FoundWord) && thereAreVerbs)
            {
                bd[i].Annotation = DesiredTag;
            }
        }
    }
}
