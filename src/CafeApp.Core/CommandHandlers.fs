module CafeApp.CommandHandlers

open System

open Chessie.ErrorHandling

open Domain
open States
open Events
open Commands
open Errors

let (|NonOrderedDrink|_|) order drink =
    if order.Drinks |> List.contains drink then
        None
    else
        Some drink

let (|ServeDrinkCompletesOrder|_|) order drink =
    if isServingDrinkCompletesOrder order drink then
        Some drink
    else
        None

let (|NonOrderedFood|_|) order food =
    if order.Foods |> List.contains food then
        None
    else
        Some food

let handleOpenTab tab = function
    | ClosedTab _ -> [ TabOpened tab ] |> ok
    | _ -> TabAlreadyOpened |> fail

let handlePlaceOrder order = function
    | OpenedTab _ ->
        if List.isEmpty order.Foods && List.isEmpty order.Drinks then
            fail CannotPlaceEmptyOrder
        else
            [ OrderPlaced order ] |> ok
    | ClosedTab _ -> fail CannotOrderWithClosedTab
    | _ -> fail OrderAlreadyPlaced

let handleServeDrink drink tabId = function
    | PlacedOrder order ->
        let event = DrinkServed (drink, tabId)
        match drink with
        | NonOrderedDrink order _ ->
            CannotServeNonOrderedDrink drink |> fail
        | ServeDrinkCompletesOrder order _ ->
            let payment = { Tab = order.Tab; Amount = orderAmount order }
            event :: [ OrderServed (order, payment) ] |> ok
        | _ -> [ event ] |> ok
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | OpenedTab _ -> CannotServeForNonPlacedOrder |> fail
    | ClosedTab _ -> CannotServeWithClosedTab |> fail
    | _ -> failwith "Todo"

let handlePrepareFood food tabId = function
    | PlacedOrder order ->
        match food with
        | NonOrderedFood order _ ->
            CannotPrepareNonOrderedFood food |> fail
        | _ ->
            [ FoodPrepared (food, tabId) ] |> ok
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | OpenedTab _ -> CannotPrepareForNonPlacedOrder |> fail
    | ClosedTab _ -> CannotPrepareWithClosedTab |> fail
    | _ -> failwith "Todo" 

let execute state command =
    match command with
    | OpenTab tab -> handleOpenTab tab state
    | PlaceOrder order -> handlePlaceOrder order state
    | ServeDrink (drink, tabId) -> handleServeDrink drink tabId state
    | PrepareFood (food, tabId) -> handlePrepareFood food tabId state
    | _ -> failwith "Todo"

let evolve state command =
    match execute state command with
    | Ok (events, _) ->
        let newState = events |> List.fold apply state
        (newState, events) |> ok
    | Bad err -> Bad err
