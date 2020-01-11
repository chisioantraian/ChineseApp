using CSharp_scripts.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpfApp2.Models;

namespace WpfApp2.Logic
{
    public static partial class ChineseService
    {
        private const string detailedInputPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\SUBTLEX.utf8";
        private const string strokesPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\ucs-strokes.txt";
        private static int rank = 0;

        public static List<Word> GetWordsFromDatabase()
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

        public static List<DetailedWord> GetDetailedWords()
        {
            List<DetailedWord> detailedWords = new List<DetailedWord>();
            foreach (var line in File.ReadAllLines(detailedInputPath).Skip(1))
            {
                AddWord(line, detailedWords);
            }
            return detailedWords;
        }

        private static void AddWord(string line, List<DetailedWord> detailedWords)
        {
            List<string> tokens = line.Split('\t').ToList();
            DetailedWord toBeInserted = new DetailedWord()
            {
                Rank = rank.ToString(),
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
            rank++;

            detailedWords.Add(toBeInserted);
        }
    }
}
