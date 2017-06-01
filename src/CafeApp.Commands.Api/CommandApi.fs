module CafeApp.Commands.Api.CommandApi

open System.Text

open Chessie.ErrorHandling

open CafeApp.Core.CommandHandlers
open CafeApp.Persistence.Queries

open CommandHandler
open OpenTab

let handleCommandRequest validationQueries eventStore = function
    | OpenTabRequest tab ->
        validationQueries.Table.GetTableByTableNumber
        |> openTabCommander
        |> handleCommand eventStore tab
    | _ -> "Invalid command" |> err |> fail |> async.Return
