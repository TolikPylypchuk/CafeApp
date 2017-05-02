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

let (|AlreadyServedDrink|_|) ipo drink =
    if ipo.ServedDrinks |> List.contains drink then
        Some drink
    else
        None

let (|NonOrderedFood|_|) order food =
    if order.Foods |> List.contains food then
        None
    else
        Some food

let (|AlreadyPreparedFood|_|) ipo food =
    if ipo.PreparedFoods |> List.contains food then
        Some food
    else
        None

let (|UnpreparedFood|_|) ipo food =
    if ipo.PreparedFoods |> List.contains food then
        None
    else
        Some food

let (|AlreadyServedFood|_|) ipo food =
    if ipo.ServedFoods |> List.contains food then
        Some food
    else
        None

let handleOpenTab tab = function
    | ClosedTab _ -> [ TabOpened tab ] |> ok
    | _ -> TabAlreadyOpened |> fail

let handlePlaceOrder order = function
    | OpenedTab _ ->
        if List.isEmpty order.Foods && List.isEmpty order.Drinks then
             CannotPlaceEmptyOrder |> fail
        else
            [ OrderPlaced order ] |> ok
    | ClosedTab _ -> CannotOrderWithClosedTab |> fail
    | _ -> OrderAlreadyPlaced |> fail

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
    | OrderInProgress ipo ->
        let order = ipo.PlacedOrder
        match drink with
        | NonOrderedDrink order _ ->
            CannotServeNonOrderedDrink drink |> fail
        | AlreadyServedDrink ipo _ ->
            CannotServeAlreadyServedDrink drink |> fail
        | _ -> [ DrinkServed (drink, order.Tab.Id) ] |> ok
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | OpenedTab _ -> CannotServeForNonPlacedOrder |> fail
    | ClosedTab _ -> CannotServeWithClosedTab |> fail

let handlePrepareFood food tabId = function
    | PlacedOrder order ->
        match food with
        | NonOrderedFood order _ ->
            CannotPrepareNonOrderedFood food |> fail
        | _ ->
            [ FoodPrepared (food, tabId) ] |> ok
    | OrderInProgress ipo ->
        let order = ipo.PlacedOrder
        match food with
        | NonOrderedFood order _ ->
            CannotPrepareNonOrderedFood food |> fail
        | AlreadyPreparedFood ipo _ ->
            CannotPrepareAlreadyPreparedFood food |> fail
        | _ -> [ FoodPrepared(food, tabId) ] |> ok
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | OpenedTab _ -> CannotPrepareForNonPlacedOrder |> fail
    | ClosedTab _ -> CannotPrepareWithClosedTab |> fail

let handleServeFood food tabId = function
    | OrderInProgress ipo ->
        let order = ipo.PlacedOrder
        match food with
        | NonOrderedFood order _ ->
            CannotServeNonOrderedFood food |> fail
        | AlreadyServedFood ipo _ ->
            CannotServeAlreadyServedFood food |> fail
        | UnpreparedFood ipo _ ->
            CannotServeNonPreparedFood food |> fail
        | _ -> [ FoodServed (food, tabId) ] |> ok
    | PlacedOrder _ -> CannotServeNonPreparedFood food |> fail
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | OpenedTab _ -> CannotServeForNonPlacedOrder |> fail
    | ClosedTab _ -> CannotServeWithClosedTab |> fail

let execute state command =
    match command with
    | OpenTab tab -> handleOpenTab tab state
    | PlaceOrder order -> handlePlaceOrder order state
    | ServeDrink (drink, tabId) -> handleServeDrink drink tabId state
    | PrepareFood (food, tabId) -> handlePrepareFood food tabId state
    | ServeFood (food, tabId) -> handleServeFood food tabId state
    | _ -> failwith "Todo"

let evolve state command =
    match execute state command with
    | Ok (events, _) ->
        let newState = events |> List.fold apply state
        (newState, events) |> ok
    | Bad err -> Bad err
