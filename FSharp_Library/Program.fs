module Program

open MySql.Data.MySqlClient
open System.Collections.Generic
open MyTypes
open Chinese.WordWithFrequency

[<EntryPoint>]
let main args =

    buildFile()
    0
