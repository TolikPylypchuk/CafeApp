module CafeApp.Core.Tests.CafeAppTestsDSL

open Chessie.ErrorHandling

open Xunit

open CafeApp.Core.CommandHandlers
open CafeApp.Core.Errors
open CafeApp.Core.Events
open CafeApp.Core.States

let Given (state: State) = state

let When command state = (command, state)

let ThenStateShouldBe expectedState (command, state) =
    match evolve state command with
    | Ok ((actualState, events), _) ->
        Assert.Equal (expectedState, actualState)
        events |> Some
    | Bad errs ->
        Assert.True
            (false, sprintf "Expected: %A, but actual: %A" expectedState errs.Head)
        None

let WithEvents (expectedEvents: Event list) (actualEvents: Event list option) =
    match actualEvents with
    | Some actualEvents ->
        Assert.Equal<Event list>(expectedEvents, actualEvents)
    | None -> Assert.Empty expectedEvents

let ShouldFailWith (expectedError: Error) (command, state) =
    match evolve state command with
    | Bad errs -> Assert.Equal (expectedError, errs.Head)
    | Ok (r, _) ->
        Assert.True
            (false, sprintf "Expected: %A, but actual: %A" expectedError r)
