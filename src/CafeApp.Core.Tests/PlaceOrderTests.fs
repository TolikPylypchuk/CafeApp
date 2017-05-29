module CafeApp.Core.Tests.PlaceOrderTests

open System

open Xunit

open CafeApp.Core.Commands
open CafeApp.Core.Domain
open CafeApp.Core.Errors
open CafeApp.Core.Events
open CafeApp.Core.States

open CafeAppTestsDSL
open TestData

[<Fact>]
let ``Can Place Only Drinks Order``() =
    let order = { order with Drinks = [ coke ] }

    Given (OpenedTab tab)
    |> When (PlaceOrder order)
    |> ThenStateShouldBe (PlacedOrder order)
    |> WithEvents [ OrderPlaced order ]

[<Fact>]
let ``Cannot Place Empty Order``() =
    Given (OpenedTab tab)
    |> When (PlaceOrder order)
    |> ShouldFailWith CannotPlaceEmptyOrder

[<Fact>]
let ``Cannot Place Order with a Closed Tab``() =
    let order = { order with Drinks = [ coke ] }

    Given (ClosedTab None)
    |> When (PlaceOrder order)
    |> ShouldFailWith CannotOrderWithClosedTab

[<Fact>]
let ``Cannot Place Order Multiple Times``() =
    let order = { order with Drinks = [ coke ] }

    Given (PlacedOrder order)
    |> When (PlaceOrder order)
    |> ShouldFailWith OrderAlreadyPlaced
