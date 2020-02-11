
namespace Chinese

open System
open System.IO
open MySql.Data.MySqlClient
open System.Collections.Generic

open MyTypes

module ChineseService =

    let filePath = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\SUBTLEX.utf8"
    let wordsPath = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\allWords.utf8"

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

    (*let allWords =
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
    *)
    
    let allWordsAvecFrequency =
        let buildWordFromLine (line:string) =
            let token = line.Split('\t')
            {
                Traditional = token.[0];
                Simplified = token.[1];
                Pinyin = token.[2];
                Definitions = token.[3];
                Frequency = token.[4] |> int
            }
        File.ReadAllLines(wordsPath) |> Seq.map buildWordFromLine

    let getAllDetailedWords() =
        allDetailedWords

    let getAllWords() =
        allWordsAvecFrequency//allWords

    //TODO it's too slow. Move rank in allWords
    let getSortedByFrequency filteredWords = //(filteredWords:List<Word>) =
        (*for w in filteredWords do
            let detailedWord = allDetailedWords
                                |> Seq.tryFind (fun dw -> w.Simplified = dw.Simplified)
            if detailedWord.IsSome then
                w.Rank <- Int32.Parse detailedWord.Value.WCount
        filteredWords |> Seq.sortBy (fun w -> w.Rank) |> Seq.rev*)
        filteredWords |> Seq.sortBy (fun w -> w.Frequency) |> Seq.rev
    
    let getEnglishResult (text:string) =
        allWordsAvecFrequency
        |> Seq.filter (fun w -> w.Definitions.Contains(text))
        |> getSortedByFrequency

    let searchBySimplified (text:string) =
        allWordsAvecFrequency
        |> Seq.filter (fun w -> w.Simplified.Contains(text))
        |> getSortedByFrequency

    //TODO too C#-like
    let searchByPinyin (text:string) =
        let prons = text.Split(' ')

        let checkIfPinyinMatches (word:WordAvecFrequency) =
            let wordProns = word.Pinyin.Split(' ')
            if prons.Length <> wordProns.Length then
                false
            else
                let mutable toInsert = true
                for i in 0 .. prons.Length-1 do
                    if  not (wordProns.[i].StartsWith (prons.[i])) then
                        toInsert <- false
                    if wordProns.[i].Length <> (prons.[i].Length + 1) then
                        toInsert <- false
                toInsert
        allWordsAvecFrequency
        |> Seq.filter checkIfPinyinMatches
        |> getSortedByFrequency

    let getRandomWords() =
        let random = new Random()
        let result = new List<WordAvecFrequency>()
        //for i in 0 .. 20 do
        //    let index = random.Next(allWordsAvecFrequency.Count)
        //    result.Add (allWordsAvecFrequency.[index])
        result

    let getResultedWord (simpl:string) = 
        allWordsAvecFrequency
        |> Seq.filter (fun w -> w.Simplified = simpl)

    //todo clarify
    let getWordsFromSentence (sentence:string) =
        let mutable constructedWord = ""
        let mutable toInsert = new List<WordAvecFrequency>()
        let mutable result = new List<WordAvecFrequency>()
        for curr in sentence do
            let resultedWord = getResultedWord(constructedWord + curr.ToString()) |> Seq.toList //getResultedWord(constructedWord + curr)
            if resultedWord.Length > 0 then
                toInsert <- new List<WordAvecFrequency>(resultedWord)
                constructedWord <- constructedWord + curr.ToString()
            else
                if toInsert.Count > 0 then
                    result.Add(toInsert.[0])
                toInsert <- new List<WordAvecFrequency>(getResultedWord(curr.ToString())) //(getResultedWord(curr.ToString()))
                constructedWord <- curr.ToString()
        if toInsert.Count > 0 then
            result.Add(toInsert.[0]) //toInsert.ForEach(w => result.Add(w));
        result

        