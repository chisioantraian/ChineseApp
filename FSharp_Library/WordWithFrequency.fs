namespace Chinese

open System.Collections.Generic
open System.IO
open System

open MyTypes
open Chinese.ChineseService



module WordWithFrequency =
    let freqInput = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\wordfreq.utf8"
    let outputFile = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\allWords.utf8"

    let buildFreqDict() =
        let freqDict = new Dictionary<string, int>()
        let addFreqFromLine (line:string) =
            let token = line.Split('\t')
            freqDict.Add(token.[0], token.[1] |> int)
        File.ReadAllLines(freqInput) |> Seq.iter addFreqFromLine
        freqDict

    let buildFile() =
        let allWords = getAllWords()
        let freqDict = buildFreqDict()
        let resultedList = new List<Word>()
        
        for word in allWords do
            let frequency = if freqDict.ContainsKey(word.Simplified) then
                                freqDict.[word.Simplified]
                            else 0
            let wordAvecFreq = {
                Traditional = word.Traditional;
                Simplified = word.Simplified;
                Pinyin = word.Pinyin;
                Definitions = word.Definitions;
                Frequency = frequency
            }
            resultedList.Add(wordAvecFreq)

        use writer = new StreamWriter(outputFile)
        for w in resultedList do
            printfn "%s : %d" w.Simplified w.Frequency
            writer.WriteLine(w.Traditional + "\t" + w.Simplified + "\t" + w.Pinyin + "\t" + w.Definitions + "\t" + w.Frequency.ToString())
              

