using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1_csharp.Logic
{
    public class Splitter
    {



        /*
        const string inputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\cjk-decomp.txt";
        static string outputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\output.txt";
        static List<string> sentences = SentenceExamples.Examples();
        static List<Word> words = ChineseService.GetWordsFromDatabase();
        //static StreamWriter outputFile = new StreamWriter(outputPath);

        enum POS
        {
            PRONOUN,
            VERB,
        }


        static Dictionary<char, POS> dict = new Dictionary<char, POS>
        {
            {'我', POS.PRONOUN}
        };

        public static void Main(string[] args)
        {
            string sentence = sentences[4];
            string constructedWord = String.Empty;
            var resultedWord = new List<Word>();
            var toInsert = new List<Word>();
            var result = new List<Word>();

            foreach (char curr in sentence)
            {
                resultedWord = GetResultedWord(constructedWord + curr);
                if (resultedWord.Count > 0)
                {
                    toInsert = resultedWord;
                    constructedWord += curr;
                }
                else
                {
                    if (toInsert.Count > 0)
                        toInsert.ForEach(w => result.Add(w));
                    toInsert = GetResultedWord(curr.ToString());
                    constructedWord = curr.ToString();
                }
            }
            if (toInsert.Count > 0)
                toInsert.ForEach(w => result.Add(w));

            using (StreamWriter outputFile = new StreamWriter(outputPath))
            {
                result.ForEach(r => outputFile.WriteLine(r.Simplified));

            }
        }

        static List<Word> GetResultedWord(string simpl)
        {
            return words.Where(w => w.Simplified == simpl).ToList();
        }*/
    }
}
