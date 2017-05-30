module CafeApp.Persistence.InMemory.Waiter

open System
open System.Collections.Generic

open CafeApp.Core.Domain
open CafeApp.Persistence.ReadModels
open CafeApp.Persistence.Projections
open CafeApp.Persistence.Queries

open CafeApp.Persistence.InMemory.Table

let private waiterToDos = new Dictionary<Guid, WaiterToDo>()

let private addDrinksToServe tabId drinks =
    match getTableByTabId tabId with
    | Some table ->
        let todo = {
            Tab = { Id = tabId; TableNumber = table.Number }
            Foods = []
            Drinks = drinks
        }
        waiterToDos.Add(tabId, todo)
    | None -> ()
    async.Return ()

let private addFoodToServe tabId food =
    if waiterToDos.ContainsKey tabId then
        let todo = waiterToDos.[tabId]
        waiterToDos.[tabId] <- { todo with Foods = food :: todo.Foods }
    else
        match getTableByTabId tabId with
        | Some table ->
            let todo = {
                Tab = { Id = tabId; TableNumber = table.Number }
                Foods = [ food ]
                Drinks = [ ]
            }
            waiterToDos.Add(tabId, todo)
        | None -> ()
    async.Return ()

let private markDrinkServed tabId drink =
    let todo = waiterToDos.[tabId]
    waiterToDos.[tabId] <- {
        todo with Drinks = todo.Drinks |> List.except [ drink ]
    }

    async.Return ()

let private markFoodServed tabId food =
    let todo = waiterToDos.[tabId]
    waiterToDos.[tabId] <- {
        todo with Foods = todo.Foods |> List.except [ food ]
    }

    async.Return ()

let private remove tabId =
    waiterToDos.Remove(tabId) |> ignore

    async.Return ()

let waiterActions = {
    AddDrinksToServe = addDrinksToServe
    MarkDrinkServed = markDrinkServed
    AddFoodToServe = addFoodToServe
    MarkFoodServed = markFoodServed
    Remove = remove
}

let getWaiterToDos () =
    waiterToDos.Values
    |> Seq.toList
    |> async.Return
