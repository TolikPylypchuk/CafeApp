module CafeApp.Persistence.InMemory.InMemory

open NEventStore

open CafeApp.Persistence.EventStore
open CafeApp.Persistence.Projections
open CafeApp.Persistence.Queries

open Table
open Waiter
open Chef
open Cashier
open Items

type InMemoryEventStore () =
    static member Instance =
        Wireup.Init()
              .UsingInMemoryPersistence()
              .Build()

let inMemoryEventStore () =
    let eventStoreInstance = InMemoryEventStore.Instance
    {
        GetState = getState eventStoreInstance
        SaveEvents = saveEvents eventStoreInstance
    }

let toDoQueries = {
    GetWaiterToDos = getWaiterToDos
    GetChefToDos = getChefToDos
    GetCashierToDos = getCashierToDos
}

let inMemoryQueries = {
    Table = tableQueries
    ToDo = toDoQueries
    Food = foodQueries
    Drink = drinkQueries
}

let inMemoryActions = {
    Table = tableActions
    Chef = chefActions
    Waiter = waiterActions
    Cashier = cashierActions
}
