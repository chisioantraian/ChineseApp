using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static MyTypes;

namespace WpfApp2.Logic
{
    /// <summary>
    /// Get all the necessary info from the disk
    /// </summary>
    public static partial class ChineseService
    {
        private const string strokesPath = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\ucs-strokes.txt";

        /// <summary>
        /// Get all the words from the mysqlDB
        /// </summary>
        /// <returns>The list of words resulted</returns>
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
                Word word = new Word(0, reader.GetString("traditional"), reader.GetString("simplified"),
                    reader.GetString("pronounciation"), reader.GetString("definitions"));
                words.Add(word);
            }
            return words;
        }

    }
}
