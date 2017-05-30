module CafeApp.Persistence.InMemory.Chef

open System
open System.Collections.Generic

open CafeApp.Core.Domain
open CafeApp.Persistence.ReadModels
open CafeApp.Persistence.Projections
open CafeApp.Persistence.Queries

open CafeApp.Persistence.InMemory.Table

let private chefToDos = new Dictionary<Guid, ChefToDo>()

let private addFoodsToPrepare tabId foods =
    match getTableByTabId tabId with
    | Some table ->
        let tab = { Id = tabId; TableNumber = table.Number }
        let todo: ChefToDo = { Tab = tab; Foods = foods }
        chefToDos.Add(tabId, todo)
    | None -> ()
    async.Return ()

let private removeFood tabId food =
    let todo = chefToDos.[tabId]
    let chefToDo = {
        todo with
            Foods = todo.Foods |> List.except [ food ]
    }

    chefToDos.[tabId] <- chefToDo
    async.Return ()

let private remove tabId =
    chefToDos.Remove(tabId) |> ignore
    async.Return ()

let chefActions = {
    AddFoodsToPrepare = addFoodsToPrepare
    RemoveFood = removeFood
    Remove = remove
}

let getChefToDos () =
    chefToDos.Values
    |> Seq.toList
    |> async.Return
