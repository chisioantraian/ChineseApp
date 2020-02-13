
namespace Chinese

open System
open System.IO
open MySql.Data.MySqlClient
open System.Collections.Generic

//open MyTypes

//module ChineseService =

    //let filePath = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\SUBTLEX.utf8"
    //let wordsPath = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\allWords.utf8"

    //let allDetailedWords = new Dictionary<string, DetailedWord>()


    (*let getDetailedWordFromLine (line:string) =
        let tokens = line.Split('\t')
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

    //delete first row file
    let buildAllDetailedWords() =
        for line in File.ReadAllLines(filePath) do
            let detailedWord = getDetailedWordFromLine(line)
            if not (allDetailedWords.ContainsKey(detailedWord.Simplified)) then
                allDetailedWords.Add(detailedWord.Simplified, detailedWord)*)

    
    //let allWords =
    //    let buildWordFromLine (line:string) =
    //        let token = line.Split('\t')
    //        {
    //            Traditional = token.[0];
    //            Simplified = token.[1];
    //            Pinyin = token.[2];
    //            Definitions = token.[3];
    //            Frequency = token.[4] |> int
    //        }
    //    File.ReadAllLines(wordsPath) |> Seq.map buildWordFromLine

    //let getAllDetailedWords() =
    //    buildAllDetailedWords()
    //    allDetailedWords

    //let getAllWords() =
    //    allWords

    //let getSortedByFrequency filteredWords =
    //    filteredWords |> Seq.sortBy (fun w -> w.Frequency) |> Seq.rev
    
    //let getEnglishResult (text:string) =
    //    allWords
    //   |> Seq.filter (fun w -> w.Definitions.Contains(text))
    //   |> getSortedByFrequency

    //let searchBySimplified (text:string) =
    //    allWords
    //    |> Seq.filter (fun w -> w.Simplified.Contains(text))
    //    |> getSortedByFrequency

    //TODO too C#-like
    (*let searchByPinyin (text:string) =
        let prons = text.Split(' ')

        let checkIfPinyinMatches (word:Word) =
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
        allWords
        |> Seq.filter checkIfPinyinMatches
        |> getSortedByFrequency*)

    //let getRandomWords() =
    //    let random = new Random()
    //    let result = new List<Word>()
        //for i in 0 .. 20 do
        //    let index = random.Next(allWordsAvecFrequency.Count)
        //    result.Add (allWordsAvecFrequency.[index])
    //    result

    //let getResultedWord (simpl:string) = 
    //    allWords
    //    |> Seq.filter (fun w -> w.Simplified = simpl)

    //todo clarify
    let getWordsFromSentence (sentence:string) =
        (*let mutable constructedWord = ""
        let mutable toInsert = new List<Word>()
        let mutable result = new List<Word>()
        for curr in sentence do
            let resultedWord = getResultedWord(constructedWord + curr.ToString()) |> Seq.toList //getResultedWord(constructedWord + curr)
            if resultedWord.Length > 0 then
                toInsert <- new List<Word>(resultedWord)
                constructedWord <- constructedWord + curr.ToString()
            else
                if toInsert.Count > 0 then
                    result.Add(toInsert.[0])
                toInsert <- new List<Word>(getResultedWord(curr.ToString())) //(getResultedWord(curr.ToString()))
                constructedWord <- curr.ToString()
        if toInsert.Count > 0 then
            result.Add(toInsert.[0]) //toInsert.ForEach(w => result.Add(w));
        result*)

        
        let mutable simpList = new List<string>()
        let mutable constructedWord = ""
        let mutable toInsert = ""
        let mutable resultList = new List<Word>()
        for curr in sentence do
            let wordToCheck = constructedWord + curr.ToString()
            if allDetailedWords.ContainsKey(wordToCheck) then
                toInsert <- allDetailedWords.[wordToCheck].Simplified
                constructedWord <- wordToCheck
            else
                if toInsert <> "" then
                    simpList.Add(toInsert)
                    toInsert <- ""
                toInsert <- curr.ToString()
        if toInsert <> "" then
            simpList.Add(toInsert)

        for simp in simpList do
            let words = allWords |> Seq.filter (fun w -> w.Simplified = simp)
            for word in words do
                resultList.Add(word)
        resultList
        
       




        