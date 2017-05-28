module CafeApp.Persistence.Queries

open System

open CafeApp.Core.Domain
open CafeApp.Core.States
open CafeApp.Persistence.ReadModels

type TableQueries = {
    GetTables: unit -> Async<Table list>
}

type ToDoQueries = {
    GetChefToDos: unit -> Async<ChefToDo list>
    GetWaiterToDos: unit -> Async<WaiterToDo list>
    GetCashierToDos: unit -> Async<Payment list>
}

type Queries = {
    Table: TableQueries
    ToDo: ToDoQueries
}
