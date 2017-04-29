module CafeApp.Errors

open Domain

type Error =
    | TabAlreadyOpened
    | CannotPlaceEmptyOrder
    | CannotOrderWithClosedTab
    | OrderAlreadyPlaced
    | OrderAlreadyServed
    | CannotServeForNonPlacedOrder
    | CannotServeWithClosedTab
    | CannotServeNonOrderedDrink of Drink    
