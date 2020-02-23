using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ChineseAppWPF.Models
{
    public class Sentence
    {
        public string Text { get; set; }
        public List<Breakdown> Correct { get; set; } = null;
        public List<Breakdown> NoAlgorithm { get; set; } = null;
        public List<Breakdown> Algorithm { get; set; } = null;

    }
}
