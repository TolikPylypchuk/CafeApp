module CafeApp.Persistence.InMemory.Table

open System.Collections.Generic

open CafeApp.Core.Domain
open CafeApp.Persistence.ReadModels
open CafeApp.Persistence.Projections
open CafeApp.Persistence.Queries

let private tables =
    let dict = new Dictionary<int, Table>()
    dict.Add(1, { Number = 1; Waiter = "X"; Status = Closed })
    dict.Add(2, { Number = 2; Waiter = "Y"; Status = Closed })
    dict.Add(3, { Number = 3; Waiter = "Z"; Status = Closed })
    dict

let private openTab tab =
    let tableNumber = tab.TableNumber
    let table = tables.[tableNumber]
    tables.[tableNumber] <- { table with Status = Open tab.Id }

    async.Return ()

let private closeTab tab =
    let tableNumber = tab.TableNumber
    let table = tables.[tableNumber]
    tables.[tableNumber] <- { table with Status = Closed }

    async.Return ()

let getTableByTabId tabId =
    tables.Values
    |> Seq.tryFind (fun t ->
        match t.Status with
        | (Open id) | (InService id) -> id = tabId
        | _ -> false)

let getTableByTabIdAsync tabId =
    getTableByTabId tabId |> async.Return

let getTableByTableNumber tableNumber =
    if tables.ContainsKey tableNumber then
        tables.[tableNumber] |> Some |> async.Return
    else
        None |> async.Return

let private receivedOrder tabId =
    match getTableByTabId tabId with
    | Some table ->
        let tableNumber = table.Number
        tables.[tableNumber] <- { table with Status = InService tabId }
    | None -> ()
    async.Return ()

let tableActions = {
    OpenTab = openTab
    ReceivedOrder = receivedOrder
    CloseTab = closeTab
}

let getTables () =
    tables.Values
    |> Seq.toList
    |> async.Return

let tableQueries = {
    GetTables = getTables
    GetTableByTabId = getTableByTabIdAsync
    GetTableByTableNumber = getTableByTableNumber
}
