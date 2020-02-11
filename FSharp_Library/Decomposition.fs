namespace Chinese

open System.Collections.Generic;
open System.IO;

open MyTypes
open Kangxi

module Decomposition =
    let inputPath =  @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\cjk-decomp.txt"
    let basicDict = new Dictionary<char, List<char>>()
    //let allWords = ChineseService.allWords
    //let kangxiRadicals = Kangxi.getRadicals() |> Seq.toList

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


    let getDefinition(character:char) =

        let mutable result = " - "
        for w in ChineseService.allWordsAvecFrequency do
            if w.Simplified = character.ToString() then
                result <- w.Definitions
        result

    //TODO rename variables, refactor
    let decomposeCharToRadicals (topChar:char) =
        if not (basicDict.ContainsKey(topChar)) then
            "cannot find decomposition"
        else
            let mutable decompositionText = ""
            let chars = new Queue<char>()
            if Kangxi.checkIfKangxiRadical(topChar) then
                decompositionText <- topChar.ToString() + " - KANGXI RADICAL\n"
            else
                for c in basicDict.[topChar] do
                    decompositionText <- decompositionText + (" " + c.ToString())
                    chars.Enqueue(c)
                decompositionText <- decompositionText + "\n"
            //decompositionText <- decompositionText + "   *** " + getDefinition(topChar) + "\n"
            while chars.Count > 0 do
                let firstChar = chars.Dequeue()
                decompositionText <- decompositionText + (firstChar.ToString() + " : ")
                if Kangxi.checkIfKangxiRadical(firstChar) then
                    decompositionText <- decompositionText + " - KANGXI RADICAL\n"
                    //decompositionText <- decompositionText + "   *** " + getDefinition(firstChar) + "\n"
                elif basicDict.ContainsKey(firstChar) then
                        for c in basicDict.[firstChar] do
                             decompositionText <- decompositionText + (" " + c.ToString())
                             chars.Enqueue(c)
                        decompositionText <- decompositionText + "\n"
                        //decompositionText <- decompositionText + "   *** " + getDefinition(firstChar) + "\n"
                else
                    decompositionText <- decompositionText + " STROKE / unencoded\n"
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

        let filteredWords = new List<WordAvecFrequency>()
        for word in ChineseService.allWordsAvecFrequency do
            if word.Simplified.Length = 1 then 
                if simplifiedComponentsFound.Contains(word.Simplified.[0]) then
                    filteredWords.Add(word)
        filteredWords
