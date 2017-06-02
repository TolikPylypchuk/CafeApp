module CafeApp.Commands.Api.PlaceOrder

open FSharp.Data

open CafeApp.Commands.Api.CommandHandler
open CafeApp.Core.Commands
open CafeApp.Core.Domain
open CafeApp.Persistence.Queries

[<Literal>]
let PlaceOrderJson = """{
    "placeOrder": {
        "tabId": "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
        "foodMenuNumbers": [ 8, 9 ],
        "drinkMenuNumbers": [ 10, 11 ]
    }
}"""

type PlaceOrderReq = JsonProvider<PlaceOrderJson>

let (|PlaceOrderRequest|_|) payload =
    try
        PlaceOrderReq.Parse(payload).PlaceOrder |> Some
    with
    | ex -> None

let validatePlaceOrder queries (c: PlaceOrderReq.PlaceOrder) = async {
    let! table = queries.Table.GetTableByTabId c.TabId
    match table with
    | Some table ->
        let! foods = queries.Food.GetFoodsByMenuNumbers c.FoodMenuNumbers
        let! drinks = queries.Drink.GetDrinksByMenuNumbers c.DrinkMenuNumbers

        let isEmptyOrder foods drinks =
            foods |> List.isEmpty && drinks |> List.isEmpty

        match foods, drinks with
        | Choice1Of2 foods, Choice1Of2 drinks ->
            if isEmptyOrder foods drinks then
                let msg = "The order should contain at least 1 food or drink"
                return Choice2Of2 msg
            else
                let tab = { Id = c.TabId; TableNumber = table.Number }
                return Choice1Of2 (tab, foods, drinks)
        | Choice2Of2 fkeys, Choice2Of2 dkeys ->
            let msg =
                sprintf "Invalid food keys: %A, invalid drinks keys: %A" fkeys dkeys
            return Choice2Of2 msg
        | Choice2Of2 keys, _ ->
            let msg = sprintf "Invalid food keys: %A" keys
            return Choice2Of2 msg
        | _, Choice2Of2 keys ->
            let msg = sprintf "Invalid drink keys: %A" keys
            return Choice2Of2 msg
    | _ -> return Choice2Of2 "Invalid tab id"
}

let toPlaceOrderCommand (tab, foods, drinks) =
    {
        Tab = tab
        Drinks = drinks
        Foods = foods
    } |> PlaceOrder

let placeOrderCommander queries =
    {
        Validate = validatePlaceOrder queries
        ToCommand = toPlaceOrderCommand
    }
