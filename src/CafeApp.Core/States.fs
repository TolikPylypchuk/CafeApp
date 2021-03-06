module CafeApp.Core.States

open System

open Domain
open Events

type State =
    | ClosedTab of Guid option
    | OpenedTab of Tab
    | PlacedOrder of Order
    | OrderInProgress of InProgressOrder
    | ServedOrder of Order

let apply state event =
    match state, event with
    | ClosedTab _, TabOpened tab -> OpenedTab tab
    | OpenedTab _, OrderPlaced order -> PlacedOrder order
    | PlacedOrder order, DrinkServed (drink, _) ->
        {
            PlacedOrder = order
            ServedDrinks = [ drink ]
            ServedFoods = []
            PreparedFoods = []
        } |> OrderInProgress
    | PlacedOrder order, FoodPrepared (food, _) ->
        {
            PlacedOrder = order
            ServedDrinks = []
            ServedFoods = []
            PreparedFoods = [ food ]
        } |> OrderInProgress
    | OrderInProgress ipo, OrderServed (order, _) ->
        ServedOrder order
    | OrderInProgress ipo, DrinkServed (drink, _) ->
        { ipo with ServedDrinks = drink :: ipo.ServedDrinks }
        |> OrderInProgress
    | OrderInProgress ipo, FoodPrepared (food, _) ->
        { ipo with PreparedFoods = food :: ipo.PreparedFoods }
        |> OrderInProgress
    | OrderInProgress ipo, FoodServed (food, _) ->
        { ipo with ServedFoods = food :: ipo.ServedFoods }
        |> OrderInProgress
    | ServedOrder order, TabClosed payment ->
        ClosedTab (Some payment.Tab.Id)
    | _ -> state
