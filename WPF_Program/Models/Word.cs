namespace WpfApp2.Models
{
    /// <summary>
    /// Represents a chinese word
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Imaginary rank in the list of the most used chars
        /// </summary>
        public int Rank { get; set; } = 200_000;
        
        /// <summary>
        /// The simplified form of the chinese word
        /// </summary>
        public string Simplified { get; set; }

        /// <summary>
        /// The traditional form of the chinese word
        /// </summary>
        public string Traditional { get; set; }

        /// <summary>
        /// The romanized form of the mandarin pronounciation
        /// </summary>
        public string Pronounciation { get; set; }

        /// <summary>
        /// The defition(s)
        /// </summary>
        public string Definitions { get; set; }
    }
}
