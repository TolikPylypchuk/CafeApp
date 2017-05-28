module CafeApp.Core.CommandHandlers

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

let (|ServeDrinkCompletesIPOrder|_|) ipo drink =
    if isServingDrinkCompletesIPOrder ipo drink then
        Some Drink
    else
        None

let (|ServeFoodCompletesIPOrder|_|) ipo food =
    if isServingFoodCompletesIPOrder ipo food then
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
        let drinkServed = DrinkServed (drink, order.Tab.Id)
        match drink with
        | NonOrderedDrink order _ ->
            CannotServeNonOrderedDrink drink |> fail
        | AlreadyServedDrink ipo _ ->
            CannotServeAlreadyServedDrink drink |> fail
        | ServeDrinkCompletesIPOrder ipo _ ->
            drinkServed :: [ OrderServed (order, payment order) ] |> ok
        | _ -> [ drinkServed ] |> ok
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
        let foodServed = FoodServed (food, tabId)
        match food with
        | NonOrderedFood order _ ->
            CannotServeNonOrderedFood food |> fail
        | AlreadyServedFood ipo _ ->
            CannotServeAlreadyServedFood food |> fail
        | UnpreparedFood ipo _ ->
            CannotServeNonPreparedFood food |> fail
        | ServeFoodCompletesIPOrder ipo _ ->
            foodServed :: [ OrderServed (order, payment order) ] |> ok
        | _ -> [ foodServed ] |> ok
    | PlacedOrder _ -> CannotServeNonPreparedFood food |> fail
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | OpenedTab _ -> CannotServeForNonPlacedOrder |> fail
    | ClosedTab _ -> CannotServeWithClosedTab |> fail

let handleCloseTab payment = function
    | ServedOrder order ->
        let orderAmount = orderAmount order
        if payment.Amount = orderAmount then
            [ TabClosed payment ] |> ok
        else
            InvalidPayment (orderAmount, payment.Amount) |> fail
    | _ -> CannotPayForNonServedOrder |> fail

let execute state command =
    match command with
    | OpenTab tab -> handleOpenTab tab state
    | PlaceOrder order -> handlePlaceOrder order state
    | ServeDrink (drink, tabId) -> handleServeDrink drink tabId state
    | PrepareFood (food, tabId) -> handlePrepareFood food tabId state
    | ServeFood (food, tabId) -> handleServeFood food tabId state
    | CloseTab payment -> handleCloseTab payment state

let evolve state command =
    match execute state command with
    | Ok (events, _) ->
        let newState = events |> List.fold apply state
        (newState, events) |> ok
    | Bad err -> Bad err
