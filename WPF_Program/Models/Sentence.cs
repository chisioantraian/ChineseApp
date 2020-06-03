using System.Collections.Generic;

namespace ChineseAppWPF.Models
{
    public class Sentence
    {
        public string Text { get; set; }
        public List<Breakdown> Correct { get; set; }
        public List<Breakdown> NoAlgorithm { get; set; }
        public List<Breakdown> Algorithm { get; set; }
    }
}
