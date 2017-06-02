module CafeApp.Commands.Api.CloseTab

open FSharp.Data

open CafeApp.Commands.Api.CommandHandler
open CafeApp.Core.CommandHandlers
open CafeApp.Core.Commands
open CafeApp.Core.Domain
open CafeApp.Persistence.ReadModels

[<Literal>]
let CloseTabJson = """{
    "closeTab": {
        "tabId": "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "amount": 10.1
    }
}"""

type CloseTabReq = JsonProvider<CloseTabJson>

let (|CloseTabRequest|_|) payload =
    try
        let req = CloseTabReq.Parse(payload).CloseTab
        (req.TabId, req.Amount) |> Some
    with
    | ex -> None

let validateCloseTab getTableByTabId (tabId, amount) = async {
    let! table = getTableByTabId tabId
    match table with
    | Some table ->
        let tab = { Id = tabId; TableNumber = table.Number }
        return Choice1Of2 { Tab = tab; Amount = amount }
    | None ->
        return Choice2Of2 "Invalid tab id"
}

let closeTabCommander getTableByTabId =
    {
        Validate = validateCloseTab getTableByTabId
        ToCommand = CloseTab
    }
