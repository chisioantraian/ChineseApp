using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_program.Models
{
    class Word
    {
        public string? Traditional { get; set; }
        public string? Simplified { get; set; }
        public string? Pinyin { get; set; }
        public string? Definitions { get; set; }
        public int? Frequency { get; set; }
    }
}
