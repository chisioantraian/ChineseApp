using System.Collections.Generic;
using System.Windows.Controls;

namespace ChineseAppWPF.Models
{
    public class ResultWord
    {
        public IEnumerable<SPPair> SimplifiedPinyinPairs { get; set; }
        public TextBlock DefinitionBlock {get;set;}
    }
}
