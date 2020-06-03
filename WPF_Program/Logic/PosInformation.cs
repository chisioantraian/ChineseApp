using System.Windows.Media;

namespace ChineseAppWPF.Logic
{
    internal static class PosInformation
    {
        internal static (SolidColorBrush, string) GetPosInfo(string description)
        {
            return description switch
            {
                "a" => (Brushes.Purple, "adjective"),
                "ad" => (Brushes.Purple, "adj as adv"),
                "ag" => (Brushes.Purple, "adj morpheme"),
                "an" => (Brushes.Purple, "adj w. nom fun"),
                "b" => (Brushes.Purple, "non-pred adje"),
                "c" => (Brushes.Pink, "conjunction"),
                "cc" => (Brushes.Pink, "conjunction"),
                "d" => (Brushes.LightBlue, "adverb"),
                "dg" => (Brushes.LightBlue, "adv morpheme"),
                "e" => (Brushes.DarkBlue, "interjection"),
                "f" => (Brushes.DarkBlue, "dir. locality"),
                "g" => (Brushes.DarkBlue, "morpheme"),
                "h" => (Brushes.DarkBlue, "prefix"),
                "i" => (Brushes.Cyan, "idiom"),
                "j" => (Brushes.DarkBlue, "interjection"),
                "k" => (Brushes.DarkBlue, "suffix"),
                "l" => (Brushes.DarkBlue, "fixed expr"),
                "m" => (Brushes.Orange, "numeral"),
                "mg" => (Brushes.Orange, "num morpheme"),
                "mq" => (Brushes.Orange, "num classifier"),
                "n" => (Brushes.Red, "noun"),
                "ng" => (Brushes.Red, "noun morpheme"),
                "nr" => (Brushes.Red, "pers name"),
                "ns" => (Brushes.Red, "place name"),
                "nt" => (Brushes.Red, "org name"),
                "nx" => (Brushes.Red, "nom. string"),
                "nz" => (Brushes.Red, "other prop. noun"),
                "o" => (Brushes.LightBlue, "onomatopoeia"),
                "p" => (Brushes.Aquamarine, "preposition"),
                "q" => (Brushes.DarkKhaki, "classifier"),
                "r" => (Brushes.DarkOrange, "pronoun"),
                "rg" => (Brushes.DarkOrange, "pron. morpheme"),
                "s" => (Brushes.Beige, "space word"),
                "t" => (Brushes.SandyBrown, "time word"),
                "tg" => (Brushes.SandyBrown, "time morpheme"),
                "u" => (Brushes.DarkBlue, "auxiliary"),
                "v" => (Brushes.LightGreen, "verb"),
                "vd" => (Brushes.LightGreen, "vb as adv"),
                "vg" => (Brushes.LightGreen, "vb morpheme"),
                "vn" => (Brushes.LightGreen, "vb nom fun"),
                "w" => (Brushes.DarkBlue, "symbol.."),
                "x" => (Brushes.Gold, "unclassififed"),
                "y" => (Brushes.Brown, "modal part."),
                "z" => (Brushes.Honeydew, "descriptive"),

                "engQmark" => (Brushes.BlueViolet, "english qMark"),
                "engDot" => (Brushes.BlueViolet, "english dot"),
                "engComma" => (Brushes.BlueViolet, "english comma"),
                "engExcl" => (Brushes.BlueViolet, "english exclamation"),
                "chnQmark" => (Brushes.BlueViolet, "chinese qMark"),
                "chnDot" => (Brushes.BlueViolet, "chinese dot"),
                "chnComma" => (Brushes.BlueViolet, "chinese comma"),
                "chnExcl" => (Brushes.BlueViolet, "chinese exclamation"),

                "?" => (Brushes.BlueViolet, "english qMark"),
                "." => (Brushes.BlueViolet, "english dot"),
                "," => (Brushes.BlueViolet, "english comma"),
                "!" => (Brushes.BlueViolet, "english exclamation"),
                "？" => (Brushes.BlueViolet, "chinese qMark"),
                "。" => (Brushes.BlueViolet, "chinese dot"),
                "，" => (Brushes.BlueViolet, "chinese comma"),
                "！" => (Brushes.BlueViolet, "chinese exclamation"),

                _ => (Brushes.Gray, "_")
            };
        }
    }
}
