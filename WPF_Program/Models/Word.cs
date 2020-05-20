using System;

namespace ChineseAppWPF.Models
{
    [Serializable]
    public class Word
    {
        public string Traditional { get; set; }
        public string Simplified { get; set; }
        public string Pinyin { get; set; }
        public string Definitions { get; set; }
        public int Frequency { get; set; }
    }
}
