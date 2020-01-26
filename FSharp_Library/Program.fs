module Program

open MySql.Data.MySqlClient
open System.Collections.Generic
open MyTypes

[<EntryPoint>]
let main args =
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

    words |> Seq.iter (fun el -> (printfn "%s" el.Traditional))
    connection.Close()

    0
