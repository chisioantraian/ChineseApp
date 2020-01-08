using MySql.Data.MySqlClient;
using System.Collections.Generic;
using WpfApp2.Models;

namespace WpfApp2.Logic
{
    public partial class ChineseService
    {
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
    }
}
