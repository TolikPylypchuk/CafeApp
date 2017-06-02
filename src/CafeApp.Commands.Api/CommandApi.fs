module CafeApp.Commands.Api.CommandApi

open System.Text

open Chessie.ErrorHandling

open CafeApp.Core.CommandHandlers
open CafeApp.Persistence.Queries

open CommandHandler
open CloseTab
open OpenTab
open PlaceOrder
open PrepareFood
open ServeDrink
open ServeFood

let handleCommandRequest queries eventStore = function
    | OpenTabRequest tab ->
        queries.Table.GetTableByTableNumber
        |> openTabCommander
        |> handleCommand eventStore tab
    | PlaceOrderRequest order ->
        placeOrderCommander queries
        |> handleCommand eventStore order
    | ServeDrinkRequest (tabId, drinkMenuNumber) ->
        queries.Drink.GetDrinkByMenuNumber
        |> serveDrinkCommander queries.Table.GetTableByTabId
        |> handleCommand eventStore (tabId, drinkMenuNumber)
    | PrepareFoodRequest (tabId, foodMenuNumber) ->
        queries.Food.GetFoodByMenuNumber
        |> prepareFoodCommander queries.Table.GetTableByTabId
        |> handleCommand eventStore (tabId, foodMenuNumber)
    | ServeFoodRequest (tabId, foodMenuNumber) ->
        queries.Food.GetFoodByMenuNumber
        |> serveFoodCommander queries.Table.GetTableByTabId
        |> handleCommand eventStore (tabId, foodMenuNumber)
    | CloseTabRequest (tabId, amount) ->
        closeTabCommander queries.Table.GetTableByTabId
        |> handleCommand eventStore (tabId, amount)
    | _ -> "Invalid command" |> err |> fail |> async.Return
