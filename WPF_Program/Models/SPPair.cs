using System.Windows.Media;

namespace ChineseAppWPF.Models
{
    public class SPPair
    {
        public char ChineseCharacter { get; set; }
        public string Pinyin { get; set; }
        public Brush CharacterColor { get; set; }
        public string SimplifiedWord { get; set; }
    }
}
