using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace ConsoleApp1_csharp.scripts
{
    public class Subtlex
    {
        private const string inputPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\SUBTLEX.utf8";
        private const string strokesPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\ucs-strokes.txt";

        private class DetailedWord
        {
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
            public string? _Traditional { get; set; }
        };

        private class Word
        {
            public string? Simplified { get; set; }
            public string? Traditional { get; set; }
            public string? Pronounciation { get; set; }
            public string? Definitions { get; set; }
        }

        private class ResultedWord
        {
            public string? Traditional { get; set; }
            public string? DominantPos { get; set; }
        }

        private readonly List<DetailedWord> detailedWords = new List<DetailedWord>();
        private readonly List<Word> words = GetWordsFromDatabase();

        public void Run()
        {
            AddSubtlexInfo(detailedWords);
            Console.WriteLine($"detailed size: {detailedWords.Count}");
           Console.WriteLine($"simple size: {words.Count}");

            CombineLists();

            //AddStrokesInfo(detailedWords);
            //Process(detailedWords);
        }

        private void CombineLists()
        {

        }



        private static List<Word> GetWordsFromDatabase()
        {
            List<Word> words = new List<Word>();
            const string connString = "SERVER=localhost;" +
                    "DATABASE=chinese;" +
                    "USER=root;" +
                    "PASSWORD=password;";

            const string sql = "SELECT * FROM words";

            using var connection = new MySqlConnection(connString);
            using var cmdSel = new MySqlCommand(sql, connection);

            MySqlDataReader reader;

            connection.Open();
            reader = cmdSel.ExecuteReader();

            while (reader.Read())
            {
                Word word = new Word
                {
                    Simplified = reader.GetString("simplified"),
                    Traditional = reader.GetString("traditional"),
                    Pronounciation = reader.GetString("pronounciation"),
                    Definitions = reader.GetString("definitions")
                };
                words.Add(word);
            }
            return words;
        }



        private void AddSubtlexInfo(List<DetailedWord> detailedWords)
        {
            foreach (var line in File.ReadAllLines(inputPath).Skip(1))
            {
                AddWord(line, detailedWords);
            }
        }

        private void AddWord(string line, List<DetailedWord> detailedWords)
        {
            List<string> tokens = line.Split('\t').ToList();
            DetailedWord toBeInserted = new DetailedWord()
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

            detailedWords.Add(toBeInserted);
        }



        private void AddStrokesInfo(List<DetailedWord> detailedWords)
        {
            foreach (DetailedWord w in detailedWords.Take(100))
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


        private void Process(List<DetailedWord> detailedWords)
        {
            var test = detailedWords.Take(100)
                            .AsEnumerable()
                            .Where(w => w.Length == "1")
                            .OrderBy(w => Int32.Parse(w?.StrokesCount ?? "0"))
                            .ToList();
            foreach (var w in test)
            {
                Console.WriteLine($"{w.Simplified} , {w.StrokesCount}");
            }
        }


        public void CreateDatabase()
        {
            // CREATE TABLE IF NOT EXISTS 
            const string ctQuery =
                  @"CREATE TABLE [Words] ( 
                    [ID] NVARCHAR(20),
                    [Pinyin] NVARCHAR(120),
                    [Definition] NVARCHAR(500),
                    [Simplified] NVARCHAR(30),
                    [StrokeCount] NVARCHAR(10)
                    )";
            SQLiteConnection.CreateFile("dataCHN.db3");
            using var con = new SQLiteConnection("data source=dataCHN.db3"); //TODO using simple vs nested
            using var com = new SQLiteCommand(con);
            con.Open();

            com.CommandText = ctQuery;
            com.ExecuteNonQuery();

            foreach (var w in detailedWords)
            {
                const string query = @"INSERT INTO WORDS (Pinyin, Definition, Simplified, StrokeCount)
                                     VALUES (@_pinyin, @_definition, @_simplified, @_strokecount);";

                com.CommandText = query;
                com.Parameters.AddWithValue("@_pinyin", w.Pinyin);
                com.Parameters.AddWithValue("@_definition", w.Definition);
                com.Parameters.AddWithValue("@_simplified", w.Simplified);
                com.Parameters.AddWithValue("@_strokecount", w.StrokesCount);

                com.ExecuteNonQuery();
            }

            con.Close();
        }



    }
}
