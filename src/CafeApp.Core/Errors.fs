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
