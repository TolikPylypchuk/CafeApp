module CafeApp.CommandHandlers

open System

open Chessie.ErrorHandling

open Domain
open States
open Events
open Commands
open Errors

let handleOpenTab tab = function
    | ClosedTab _ -> [ TabOpened tab ] |> ok
    | _ -> TabAlreadyOpened |> fail

let execute state command =
    match command with
    | OpenTab tab -> handleOpenTab tab state
    | _ -> failwith "Todo"

let evolve state command =
    match execute state command with
    | Ok (events, _) ->
        let newState = List.fold apply state events
        (newState, events) |> ok
    | Bad err -> Bad err
