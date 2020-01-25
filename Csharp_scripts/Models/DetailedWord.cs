using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp_scripts.Models
{
    /// <summary>
    /// Represents a chinese word, but in a different format from the Word class
    /// </summary>
    public class DetailedWord
    {
        public string Rank { get; set; }
        public string? Simplified { get; set; }
        public string? Length { get; set; }
        public string? Pinyin { get; set; }
        public string? PinyinInput { get; set; }
        public string? WCount { get; set; } //maybe useless
        public string? WMillion { get; set; }
        public string? Log10W { get; set; }
        public string? W_CD { get; set; }
        public string? W_CD_Percent { get; set; }
        public string? Log10CD { get; set; }
        public string? DominantPos { get; set; }
        public string? DominantPosFreq { get; set; }
        public string? AllPos { get; set; }
        public string? AllPosFreq { get; set; }
        public string? Definition { get; set; }
        public string? StrokesCount { get; set; }
    };
}
