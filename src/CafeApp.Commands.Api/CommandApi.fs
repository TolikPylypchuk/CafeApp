module CafeApp.Commands.Api.CommandApi

open System.Text

open Chessie.ErrorHandling

open CafeApp.Core.CommandHandlers
open CafeApp.Persistence.Queries

open CommandHandler
open OpenTab
open PlaceOrder

let handleCommandRequest queries eventStore = function
    | OpenTabRequest tab ->
        queries.Table.GetTableByTableNumber
        |> openTabCommander
        |> handleCommand eventStore tab
    | PlaceOrderRequest order ->
        placeOrderCommander queries
        |> handleCommand eventStore order
    | _ -> "Invalid command" |> err |> fail |> async.Return
