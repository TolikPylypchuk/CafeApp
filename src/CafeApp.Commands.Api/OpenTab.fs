module CafeApp.Commands.Api.OpenTab

open System

open FSharp.Data

open CafeApp.Core.Commands
open CafeApp.Core.Domain
open CafeApp.Commands.Api.CommandHandler

[<Literal>]
let OpenTabJson = """{
    "openTab": {
        "tableNumber": 1
    }
}"""

type OpenTabReq = JsonProvider<OpenTabJson>

let (|OpenTabRequest|_|) payload =
    try
        let req = OpenTabReq.Parse(payload).OpenTab
        { Id = Guid.NewGuid(); TableNumber = req.TableNumber } |> Some
    with
    | ex -> None

let validateOpenTab getTableByTableNumber tab = async {
    let! result = getTableByTableNumber tab.TableNumber
    match result with
    | Some table ->
        return Choice1Of2 tab
    | None ->
        return Choice2Of2 "Invalid table number"
}

let openTabCommander getTableByTableNumber =
    {
        Validate = validateOpenTab getTableByTableNumber
        ToCommand = OpenTab
    }
