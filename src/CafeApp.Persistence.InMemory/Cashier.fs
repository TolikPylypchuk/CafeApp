module CafeApp.Persistence.InMemory.Cashier

open System
open System.Collections.Generic

open CafeApp.Core.Domain
open CafeApp.Persistence.ReadModels
open CafeApp.Persistence.Projections
open CafeApp.Persistence.Queries

open CafeApp.Persistence.InMemory.Table

let private cashierToDos = new Dictionary<Guid, Payment>()

let addTabAmount tabId amount =
    match getTableByTabId tabId with
    | Some table ->
        let payment = {
            Tab = { Id = tabId; TableNumber = table.Number }
            Amount = amount
        }
        cashierToDos.Add(tabId, payment) |> ignore
    | None -> ()
    async.Return ()

let remove tabId =
    cashierToDos.Remove(tabId) |> ignore
    async.Return ()

let cashierActions = {
    AddTabAmount = addTabAmount
    Remove = remove
}

let getCashierToDos () =
    cashierToDos.Values
    |> Seq.toList
    |> async.Return
