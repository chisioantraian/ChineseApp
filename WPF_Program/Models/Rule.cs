
namespace ChineseAppWPF.Models
{
    public abstract class Rule{ }

    public class NextTagRule : Rule
    {
        public string NextTag { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }
    }

    public class PrevTagRule : Rule
    {
        public string PrevTag { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }
    }

    public class NextWordRule : Rule
    {
        public string NextWord { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }
    }

    public class BetweenTagsRule : Rule
    {
        public string LeftTag { get; set; }
        public string RightTag { get; set; }
        public string CurrentWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }
    }

    public class BetweenWordsRule : Rule
    {
        public string LeftWord { get; set; }
        public string RightWord { get; set; }
        public string CurrentTag { get; set; }
        public string DesiredTag { get; set; }
    }

    public class NoMoreVerbsRule : Rule
    {
        public string CurrentChar { get; set; }
        public string DesiredTag { get; set; }
    }
}
