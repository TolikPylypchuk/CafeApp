module CafeApp.Persistence.ReadModels

open System

open CafeApp.Core.Domain

type TableStatus =
    | Open of Guid
    | InService of Guid
    | Closed

type Table = {
    Number: int
    Waiter: string
    Status: TableStatus
}

type ChefToDo = {
    Tab: Tab
    Foods: Food list
}

type WaiterToDo = {
    Tab: Tab
    Drinks: Drink list
    Foods: Food list
}
