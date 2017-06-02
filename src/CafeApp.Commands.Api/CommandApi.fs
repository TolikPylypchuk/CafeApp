module CafeApp.Commands.Api.CommandApi

open System.Text

open Chessie.ErrorHandling

open CafeApp.Core.CommandHandlers
open CafeApp.Persistence.Queries

open CommandHandler
open OpenTab
open PlaceOrder
open ServeDrink

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
    | _ -> "Invalid command" |> err |> fail |> async.Return
