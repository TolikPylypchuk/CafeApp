module CafeApp.Events

open System

open Domain

type Event =
    | TabOpened of Tab
    | OrderPlaced of Order
    | DrinkServed of Drink * Guid
    | FoodPrepared of Food * Guid
    | FoodServed of Food * Guid
    | OrderServed of Order * Payment
    | TabClosed of Payment
