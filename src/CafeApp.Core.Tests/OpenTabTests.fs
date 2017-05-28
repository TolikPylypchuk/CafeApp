module CafeApp.Core.Tests.OpenTabTests

open System

open Xunit

open CafeApp.Core.Commands
open CafeApp.Core.Domain
open CafeApp.Core.Events
open CafeApp.Core.States

open CafeAppTestsDSL

[<Fact>]
let ``Can Open a New Tab``() =
    let tab = { Id = Guid.NewGuid(); TableNumber = 1 }

    Given (ClosedTab None)
    |> When (OpenTab tab)
    |> ThenStateShouldBe (OpenedTab tab)
    |> WithEvents [ TabOpened tab ]
