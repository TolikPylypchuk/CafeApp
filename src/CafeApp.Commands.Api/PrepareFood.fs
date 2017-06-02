module CafeApp.Commands.Api.PrepareFood

open FSharp.Data

open CafeApp.Commands.Api.CommandHandler
open CafeApp.Core.CommandHandlers
open CafeApp.Core.Commands

[<Literal>]
let PrepareFoodJson = """{
    "prepareFood": {
        "tabId": "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "menuNumber": 10
    }
}"""

type PrepareFoodReq = JsonProvider<PrepareFoodJson>

let (|PrepareFoodRequest|_|) payload =
    try
        let req = PrepareFoodReq.Parse(payload).PrepareFood
        (req.TabId, req.MenuNumber) |> Some
    with
    | ex -> None

let validateFood getTableByTabId getFoodByMenuNumber (tabId, foodMenuNumber) = async {
    let! table = getTableByTabId tabId
    match table with
    | Some _ ->
        let! food = getFoodByMenuNumber foodMenuNumber
        match food with
        | Some food ->
            return Choice1Of2 (food, tabId)
        | None ->
            return Choice2Of2 "Invalid food menu number"
    | None ->
        return Choice2Of2 "Invalid tab id"
}

let prepareFoodCommander getTableByTabId getFoodByMenuNumber =
    {
        Validate = validateFood getTableByTabId getFoodByMenuNumber
        ToCommand = PrepareFood
    }
