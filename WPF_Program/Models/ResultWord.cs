using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChineseAppWPF.Models
{
    public class ResultWord
    {
        public IEnumerable<SPPair> SimplifiedPinyinPairs { get; set; }
        //public string Definitions { get; set; }
        public IEnumerable<TextBlock> DefinitionBlocks {get;set;}
    }
}
