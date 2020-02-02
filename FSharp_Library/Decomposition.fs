namespace Chinese

open System.Collections.Generic;
open System.IO;

open MyTypes
open Kangxi

module Decomposition =
    let inputPath =  @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\cjk-decomp.txt"
    let basicDict = new Dictionary<char, List<char>>()

    let analyzeLine (line:string) =
        let character = line.Split(':').[0].[0]
        let afterParan = line.Split('(').[1]
        let components = new List<char>()

        if afterParan.Contains(',') then
            components.Add <| afterParan.Split(',').[0].[0]
            components.Add <| afterParan.Split(',').[1]
                                        .Split(')').[0].[0]
        else
            components.Add <| afterParan.Split(')').[0].[0]

        if not (basicDict.ContainsKey(character)) then
            basicDict.Add(character, components)

    let getDecompositionRules() =
        inputPath |> File.ReadAllLines
                  |> Seq.skip 10640
                  |> Seq.iter analyzeLine
        basicDict
    
    let getCharacterDecomposition() =
        let basicDict = getDecompositionRules()
        let resultDict = new Dictionary<char, List<char>>()
        let kangxiRadicals = Kangxi.getRadicals()

        for dictKey in basicDict.Keys do
            let mutable foundRadical = false
            for kg in kangxiRadicals do
                if kg.Symbol = dictKey then
                    foundRadical <- true //todo break alternative
            if not foundRadical then
                resultDict.Add(dictKey, basicDict.[dictKey])

        resultDict

    //TODO rename variables
    let decomposeCharToRadicals (ch:char) =
        let mutable decompositionText = ""
        let chars = new Queue<char>()
        for c in basicDict.[ch] do
            decompositionText <- decompositionText + (" " + c.ToString());
            chars.Enqueue(c)
        while chars.Count > 0 do
            let firstCh = chars.Dequeue()
            decompositionText <- decompositionText + (firstCh.ToString() + " : ")
            if basicDict.ContainsKey(firstCh) then
                for c in basicDict.[firstCh] do
                     decompositionText <- decompositionText + (" " + c.ToString())
                     chars.Enqueue(c)
                decompositionText <- decompositionText + "\n"
        decompositionText


    let getCharactersWithComponent (text:string) = 
        let ch = text.[0]
        let simplifiedComponentsFound = new List<char>()
        for decompositionTuple in basicDict do
            let componentList = decompositionTuple.Value
            if componentList.Contains(ch) then
                printfn "dt : %c" decompositionTuple.Key
                simplifiedComponentsFound.Add(decompositionTuple.Key)
        printfn "scf size = %d" simplifiedComponentsFound.Count

        let filteredWords = new List<Word>()
        for word in ChineseService.allWords do
            if word.Simplified.Length = 1 then 
                if simplifiedComponentsFound.Contains(word.Simplified.[0]) then
                    filteredWords.Add(word)
        filteredWords
