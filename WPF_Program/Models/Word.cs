namespace WpfApp2.Models
{
    public class Word
    {
        public int Rank { get; set; } = 200_000;
        public string Simplified { get; set; }
        public string Traditional { get; set; }
        public string Pronounciation { get; set; }
        public string Definitions { get; set; }
    }
}
