module CafeApp.Commands.Api.ServeDrink

open FSharp.Data

open CafeApp.Core.CommandHandlers
open CafeApp.Core.Commands
open CafeApp.Commands.Api.CommandHandler

[<Literal>]
let ServeDrinkJson = """{
    "serveDrink": {
        "tabId": "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "menuNumber": 10
    }
}"""

type ServeDrinkReq = JsonProvider<ServeDrinkJson>

let (|ServeDrinkRequest|_|) payload =
    try
        let req = ServeDrinkReq.Parse(payload).ServeDrink
        (req.TabId, req.MenuNumber) |> Some
    with
    | ex -> None

let validateServeDrink getTableByTabId getDrinkByMenuNumber (tabId, drinkMenuNumber) = async {
    let! table = getTableByTabId tabId
    match table with
    | Some _ ->
        let! drink = getDrinkByMenuNumber drinkMenuNumber
        match drink with
        | Some drink ->
            return Choice1Of2 (drink, tabId)
        | None ->
            return Choice2Of2 "Invalid drink menu number"
    | None ->
        return Choice2Of2 "Invalid tab id"
}

let serveDrinkCommander getTableByTabId getDrinkByMenuNumber =
    let validate =
        getDrinkByMenuNumber
        |> validateServeDrink getTableByTabId
    {
        Validate = validate
        ToCommand = ServeDrink
    }
