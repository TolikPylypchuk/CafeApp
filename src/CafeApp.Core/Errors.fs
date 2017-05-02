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
    | CannotPrepareNonOrderedFood of Food
