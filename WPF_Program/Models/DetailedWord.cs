using System;

namespace ChineseAppWPF.Models
{
    [Serializable]
    public class DetailedWord
    {
        public string Simplified { get; set; }
        public string Length { get; set; }
        public string Pinyin { get; set; }
        public string PinyinInput { get; set; }
        public string WCount { get; set; }
        public string WMillion { get; set; }
        public string Log10W { get; set; }
        public string Wcd { get; set; }
        public string WcdPercent { get; set; }
        public string Log10CD { get; set; }
        public string DominantPos { get; set; }
        public string DominantPosFreq { get; set; }
        public string AllPos { get; set; }
        public string AllPosFreq { get; set; }
        public string Definition { get; set; }
        public string StrokesCount { get; set; }
    };
}
