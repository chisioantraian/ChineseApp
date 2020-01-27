
namespace Chinese

open System
open System.IO
open MySql.Data.MySqlClient
open System.Collections.Generic

open MyTypes

module ChineseService =

    let filePath = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\SUBTLEX.utf8"

    let getWordFromLine (line:string) = 
        let tokens = line.Split '\t'
        {
            Simplified = tokens.[0];
            Length = tokens.[1];
            Pinyin = tokens.[2];
            PinyinInput = tokens.[3];
            WCount = tokens.[4];
            WMillion = tokens.[5];
            Log10W = tokens.[6];
            W_CD = tokens.[7];
            W_CD_percent = tokens.[8];
            Log10CD = tokens.[9];
            DominantPos = tokens.[10];
            DominantPosFreq = tokens.[11];
            AllPos = tokens.[12];
            AllPosFreq = tokens.[13];
            Definition = tokens.[14];
        }

    let allDetailedWords =
        File.ReadAllLines(filePath) |> Seq.map getWordFromLine 

    let allWords =
        let connString = "SERVER=localhost; DATABASE=chinese; USER=root; PASSWORD=password;"
        let qry = "SELECT * FROM words;"
        let words = new List<Word>()

        let connection = new MySqlConnection(connString)
        let commandSel = new MySqlCommand(qry, connection)

        connection.Open()
        let reader = commandSel.ExecuteReader()
        while reader.Read() do
            let word = {
                Rank = 0;
                Simplified = reader.GetString "simplified";
                Traditional = reader.GetString "traditional";
                Pronounciation = reader.GetString "pronounciation";
                Definitions = reader.GetString "definitions";
            }
            words.Add word
        connection.Close()
        words

    let getAllDetailedWords() =
        allDetailedWords

    let getAllWords() =
        allWords

    let getSortedByFrequency (filteredWords:List<Word>) =
        for w in filteredWords do
            let detailedWord = allDetailedWords
                                |> Seq.tryFind (fun dw -> w.Simplified = dw.Simplified)
            if detailedWord.IsSome then
                w.Rank <- Int32.Parse detailedWord.Value.WCount
        filteredWords |> Seq.sortBy (fun w -> w.Rank) |> Seq.rev


