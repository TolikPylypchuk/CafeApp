module CafeApp.Tests.OpenTabTests

open System

open NUnit.Framework

open CafeAppTestsDSL

open CafeApp.Domain
open CafeApp.Commands
open CafeApp.Events
open CafeApp.States
open CafeApp.Errors

[<Test>]
let ``Can Open a New Tab``() =
    let tab = { Id = Guid.NewGuid(); TableNumber = 1 }

    Given (ClosedTab None)
    |> When (OpenTab tab)
    |> ThenStateShouldBe (OpenedTab tab)
    |> WithEvents [ TabOpened tab ]

[<Test>]
let ``Cannot Open an Already Opened Tab``() =
    let tab = { Id = Guid.NewGuid(); TableNumber = 1 }

    Given (OpenedTab tab)
    |> When (OpenTab tab)
    |> ShouldFailWith TabAlreadyOpened
