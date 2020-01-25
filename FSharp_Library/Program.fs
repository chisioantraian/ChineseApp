// Learn more about F# at http://fsharp.org

namespace NumeTestare

open System
open System.IO
open System.Collections.Generic

type DetailedWordF = {
    Simplified: string
    Length: string
    Pinyin: string
    PinyinInput: string
    WCount: string
    WMillion: string
    Log10W: string
    W_CD: string
    W_CD_percent: string
    Log10CD: string
    DominantPos: string
    DominantPosFreq: string
    AllPos: string
    AllPosFreq: string
    Definition: string
}

//[<EntryPoint>]
//let main argv =
module Testare =

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

    let cautaTateCuvintele() =
        File.ReadAllLines(filePath) |> Seq.map getWordFromLine 


