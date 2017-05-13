module CafeApp.Errors

open Domain

type Error =
    | TabAlreadyOpened
    | CannotPlaceEmptyOrder
    | CannotOrderWithClosedTab
    | OrderAlreadyPlaced
    | OrderAlreadyServed
    | CannotServeForNonPlacedOrder
    | CannotPrepareForNonPlacedOrder
    | CannotServeWithClosedTab
    | CannotPrepareWithClosedTab
    | CannotServeNonOrderedDrink of Drink
    | CannotServeAlreadyServedDrink of Drink
    | CannotPrepareNonOrderedFood of Food
    | CannotPrepareAlreadyPreparedFood of Food
    | CannotServeAlreadyServedFood of Food
    | CannotServeNonPreparedFood of Food
    | CannotServeNonOrderedFood of Food
    | InvalidPayment of decimal * decimal
    | CannotPayForNonServedOrder

let toErrorString = function
    | TabAlreadyOpened -> "Tab already opened"
    | CannotPlaceEmptyOrder -> "Cannot place an empty order"
    | CannotOrderWithClosedTab -> "Cannot order as tab is closed"
    | OrderAlreadyPlaced -> "Order is already placed"
    | OrderAlreadyServed -> "Order is already served"
    | CannotServeForNonPlacedOrder -> "Cannot serve for non-placed order"
    | CannotPrepareForNonPlacedOrder -> "Cannot prepare for non-placed order"
    | CannotServeWithClosedTab -> "Cannot serve as tab is closed"
    | CannotPrepareWithClosedTab -> "Cannot prepare as tab is closed"
    | CannotServeNonOrderedDrink (Drink drink) ->
        sprintf "Drink %s (%d) is not ordered" drink.Name drink.MenuNumber
    | CannotServeAlreadyServedDrink (Drink drink) ->
        sprintf "Drink %s (%d) is already served" drink.Name drink.MenuNumber
    | CannotPrepareNonOrderedFood (Food food) ->
        sprintf "Food %s (%d) is not ordered" food.Name food.MenuNumber
    | CannotPrepareAlreadyPreparedFood (Food food) ->
        sprintf "Food %s (%d) is already prepared" food.Name food.MenuNumber
    | CannotServeAlreadyServedFood (Food food) ->
        sprintf "Food %s (%d) is already served" food.Name food.MenuNumber
    | CannotServeNonPreparedFood (Food food) ->
        sprintf "Food %s (%d) has not been prepared" food.Name food.MenuNumber
    | CannotServeNonOrderedFood (Food food) ->
        sprintf "Food %s (%d) is not ordered" food.Name food.MenuNumber
    | InvalidPayment (expected, actual) ->
        sprintf "Invalid payment - expected is %f but paid %f" expected actual
    | CannotPayForNonServedOrder -> "Cannot pay for non-served order"
