module CafeApp.Tests.PlaceOrderTests

open System

open NUnit.Framework

open CafeAppTestsDSL

open CafeApp.Domain
open CafeApp.States
open CafeApp.Commands
open CafeApp.Events
open CafeApp.Errors

let tab = { Id = Guid.NewGuid(); TableNumber = 1 }

let coke = Drink {
    MenuNumber = 1;
    Name = "Coke"
    Price = 1.5M
}

let order = { Tab = tab; Foods = []; Drinks = [ ] }

[<Test>]
let ``Can Place Only Drinks Order``() =
    let order = { order with Drinks = [ coke ] }
    Given (OpenedTab tab)
    |> When (PlaceOrder order)
    |> ThenStateShouldBe (PlacedOrder order)
    |> WithEvents [ OrderPlaced order ]

[<Test>]
let ``Cannot Place Order Empty Order``() =
    Given (OpenedTab tab)
    |> When (PlaceOrder order)
    |> ShouldFailWith CannotPlaceEmptyOrder

[<Test>]
let ``Cannot Place Order with a Closed Tab``() =
    let order = { order with Drinks = [ coke ] }
    Given (ClosedTab None)
    |> When (PlaceOrder order)
    |> ShouldFailWith CannotOrderWithClosedTab

[<Test>]
let ``Cannot Place Order Multiple Times``() =
    let order = { order with Drinks = [ coke ] }
    Given (PlacedOrder order)
    |> When (PlaceOrder order)
    |> ShouldFailWith OrderAlreadyPlaced