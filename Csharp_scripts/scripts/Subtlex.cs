using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace ConsoleApp1_csharp.scripts
{
    public class Subtlex
    {
        const string inputPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\SUBTLEX.utf8";
        const string strokesPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\ucs-strokes.txt";
        //static string outputPath = @"C:\Users\chisi\Desktop\work\ConsoleApp1_csharp\ConsoleApp1_csharp\output.txt";
        //static StreamWriter outputFile = new StreamWriter(outputPath);
        //var inputFile = new StreamReader(inputPath);

        class Word
        {
            public string Simplified { get; set; }
            public string Length { get; set; }
            public string Pinyin { get; set; }
            public string PinyinInput { get; set; }
            public string WCount { get; set; } //maybe useless
            public string WMillion { get; set; }
            public string Log10W { get; set; }
            public string W_CD { get; set; }
            public string W_CD_Percent { get; set; }
            public string Log10CD { get; set; }
            public string DominantPos { get; set; }
            public string DominantPosFreq { get; set; }
            public string AllPos { get; set; }
            public string AllPosFreq { get; set; }
            public string Definition { get; set; }
            public string StrokesCount { get; set; }
        };

        List<Word> words = new List<Word>();

        public void Run()
        {

            AddSubtlexInfo(words);
            //AddStrokesInfo(words);

            //Process(words);
        }

        private void AddSubtlexInfo(List<Word> words)
        {
            foreach (var line in File.ReadAllLines(inputPath).Skip(1))
            {
                AddWord(line, words);
            }
        }

        private void AddWord(string line, List<Word> words)
        {
            List<string> tokens = line.Split('\t').ToList();
            Word toBeInserted = new Word()
            {
                Simplified = tokens[0],
                Length = tokens[1],
                Pinyin = tokens[2],
                PinyinInput = tokens[3],
                WCount = tokens[4],
                WMillion = tokens[5],
                Log10W = tokens[6],
                W_CD = tokens[7],
                W_CD_Percent = tokens[8],
                Log10CD = tokens[9],
                DominantPos = tokens[10],
                DominantPosFreq = tokens[11],
                AllPos = tokens[12],
                AllPosFreq = tokens[13],
                Definition = tokens[14]
            };

            words.Add(toBeInserted);
        }

        private void AddStrokesInfo(List<Word> words)
        {
            foreach (Word w in words.Take(100))
            {
                foreach (var line in File.ReadAllLines(strokesPath))
                {
                    List<string> tokens = line.Split('\t').ToList();
                    string character = tokens[1];
                    string strokeCount = tokens[2];

                    if (strokeCount.Contains(","))
                    {
                        strokeCount = strokeCount.Split(",")[1];
                    }

                    if (w.Simplified == character)
                    {
                        w.StrokesCount = strokeCount;
                    }
                }
            }
        }

        private void Process(List<Word> words)
        {
            var test = words.Take(100).ToList()
                            .Where(w => w.Length == "1")
                            .OrderBy(w => Int32.Parse(w.StrokesCount)).ToList();
            foreach (var w in test)
            {
                Console.WriteLine($"{w.Simplified} , {w.StrokesCount}");
            }
        }

        public void CreateDatabase()
        {
            string ctQuery = @"CREATE TABLE IF NOT EXISTS [Words] (
                             [ID] NVARCHAR(20),
                             [Pinyin] NVARCHAR(120),
                             [Definition] NVARCHAR(500),
                             [Simplified] NVARCHAR(30),
                             [StrokeCount] NVARCHAR(5)
                             )";
            SQLiteConnection.CreateFile("dataCHN.db3");
            using (var con = new SQLiteConnection("data source=dataCHN.db3"))
            using (var com = new SQLiteCommand(con))
            {
                con.Open();

                com.CommandText = ctQuery;
                com.ExecuteNonQuery();

                foreach (var w in words)
                {
                    Console.WriteLine($"{w.Simplified}");
                    com.CommandText = $@"INSERT INTO Words (Pinyin, Definition, Simplified, StrokeCount)
                                         VALUES ('{w.Pinyin}', '{w.Definition}', '{w.Simplified}', '{w.StrokesCount}')";
                    com.ExecuteNonQuery();
                }

                con.Close();
            }
        }

    }
}
