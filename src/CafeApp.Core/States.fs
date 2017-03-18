module CafeApp.States

open System

open Domain

type State =
    | ClosedTab of Guid option
    | OpenedTab of Tab
    | PlacedOrder of Order
    | OrderInProgress of InProgressOrder
    | ServedOrder of Order
