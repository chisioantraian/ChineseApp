using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WpfApp2.Logic;
using WpfApp2.Models;

namespace ConsoleApp1_csharp.Logic
{
    internal static class Main2
    {
        const string inputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\cjk-decomp.txt";
        static string outputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\output.txt";
        static List<string> sentences = SentenceExamples.Examples();
        static List<Word> words = ChineseService.GetWordsFromDatabase();
        static StreamWriter outputFile = new StreamWriter(outputPath);

        enum POS
        {
            ADJECTIVE,
            ADVERB,
            CONJUNCTION,
            PREPOSITION,
            PRONOUN,
            NOUN,
            VERB,
        }


        static Dictionary<string, string> dict = new Dictionary<string, string>
        {
            /*{"我", POS.PRONOUN },
            {"我们", POS.PRONOUN },
            {"你", POS.PRONOUN },
            {"你门", POS.PRONOUN },
            {"他", POS.PRONOUN },
            {"她", POS.PRONOUN },
            {"他们", POS.PRONOUN },
            {"她们", POS.PRONOUN },
            {"咱们", POS.PRONOUN },*/

            { "快乐", "ADJECTIVE" },

            {"马上", "ADVERB" },

            {"昨天", "COMPLEMENT_OF_TIME" },

            {"如果", "CONJUNCTION" },
            {"还是", "CONJUNCTION" },
            {"但是", "CONJUNCTION" },

            {"天气", "NOUN" },
            {"桌子", "NOUN" },
            {"咖啡", "NOUN" },
            {"茶", "NOUN" },
            {"中国", "NOUN" },
            {"狗", "NOUN" },
            {"弟弟", "NOUN" },
            {"叔叔", "NOUN" },
            {"图书馆", "NOUN" },
            {"照相机", "NOUN" },


            {"我", "PRONOUN" },
            {"我们", "PRONOUN" },
            {"你", "PRONOUN" },
            {"你门", "PRONOUN" },
            {"他", "PRONOUN" },
            {"她", "PRONOUN" },
            {"它", "PRONOUN" },
            {"他们", "PRONOUN" },
            {"她们", "PRONOUN" },
            {"咱们", "PRONOUN" },

        };



        public static void StartProgram()
        {

            //StreamWriter outputFile = new StreamWriter(outputPath);

            foreach (string sentence in sentences)
            {
                var tokens = SplitIntoWords(sentence);
                ShowPosTags(tokens);
            }

            outputFile.Close();
        }

        static List<string> SplitIntoWords(string sentence)
        {
            List<string> result = new List<string>();
            string curr = "";

            foreach (char c in sentence)
            {
                if (WordExists(curr + c))
                {
                    curr += c;
                }
                else
                {
                    outputFile.Write(curr + ", ");
                    result.Add(curr);
                    curr = c.ToString();
                }
            }
            if (curr != "")
            {
                outputFile.Write(curr);
                result.Add(curr);
            }
            outputFile.Write('\n');
            return result;
        }

        static bool WordExists(string simpl)
        {
            return words.Any(w => w.Simplified == simpl);
        }

        static void ShowPosTags(List<string> tokens)
        {

            foreach (string tok in tokens)
            {
                if (dict.ContainsKey(tok))
                {
                    var tag = dict[tok];
                    outputFile.Write($"{tok} :  {tag}\n");
                }
            }
            outputFile.Write("\n");
        }

        static void SplitIntoWords_2(string sentence, StreamWriter outputFile)
        {
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


            //outputFile.Write(sentence + " : ");
            foreach (Word w in result)
            {
                outputFile.Write(w.Simplified + ", ");
            }
            outputFile.Write('\n');
        }

        static List<Word> GetResultedWord(string simpl)
        {
            return new List<Word>((IEnumerable<Word>)words.First(w => w.Simplified == simpl));
        }

    }
}
