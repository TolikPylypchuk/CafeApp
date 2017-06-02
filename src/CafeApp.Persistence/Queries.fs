module CafeApp.Persistence.Queries

open System

open CafeApp.Core.Domain
open CafeApp.Core.States
open CafeApp.Persistence.ReadModels

type TableQueries = {
    GetTables: unit -> Async<Table list>
    GetTableByTabId: Guid -> Async<Table option>
    GetTableByTableNumber: int -> Async<Table option>
}

type ToDoQueries = {
    GetChefToDos: unit -> Async<ChefToDo list>
    GetWaiterToDos: unit -> Async<WaiterToDo list>
    GetCashierToDos: unit -> Async<Payment list>
}

type FoodQueries = {
    GetFoodsByMenuNumbers: int[] -> Async<Choice<Food list, int[]>>
}

type DrinkQueries = {
    GetDrinksByMenuNumbers: int[] -> Async<Choice<Drink list, int[]>>
}

type Queries = {
    Table: TableQueries
    ToDo: ToDoQueries
    Food: FoodQueries
    Drink: DrinkQueries
}
