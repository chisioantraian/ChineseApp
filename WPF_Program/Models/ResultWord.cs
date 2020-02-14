using System.Collections.Generic;

namespace ChineseAppWPF.Models
{
    public class ResultWord
    {
        public IEnumerable<SPPair> SimplifiedPinyinPairs { get; set; }
        public string Definitions { get; set; }
    }
}
