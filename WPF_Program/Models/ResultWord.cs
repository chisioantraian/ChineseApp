using System.Collections.Generic;
using System.Windows.Media;

namespace ChineseAppWPF.Models
{
    public class ResultWord
    {
        public IEnumerable<SPPair> SimplifiedPinyinPairs { get; set; }
        public string Definitions { get; set; }

        public Brush culoare { get; set; } = Brushes.DarkRed;
        //public string culoare { get; set; }
    }
}
