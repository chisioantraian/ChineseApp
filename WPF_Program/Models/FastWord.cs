using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ChineseAppWPF.Models
{
    public class FastWord
    {
        public string Traditional { get; set; }
        public string Simplified { get; set; }
        public string Pinyin { get; set; }
        public string Definitions { get; set; }
        public int Frequency { get; set; }

        public StackPanel elem { get; set; }
    }
}
