module CafeApp.Core.Tests.CloseTabTests

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
let ``Can Close Tab by Paying Full Amount``() =
    let order = {
        order with
            Foods = [ salad; pizza ]
            Drinks = [ coke ]
    }
    let payment = { Tab = order.Tab; Amount = 10.5M }

    Given (ServedOrder order)
    |> When (CloseTab (payment))
    |> ThenStateShouldBe (ClosedTab (Some order.Tab.Id))
    |> WithEvents [ TabClosed payment ]

[<Fact>]
let ``Cannot Close Tab with Invalid Payment``() =
    let order = {
        order with
            Foods = [ salad; pizza ]
            Drinks = [ coke ]
    }

    Given (ServedOrder order)
    |> When (CloseTab { Tab = order.Tab; Amount = 9.5M })
    |> ShouldFailWith (InvalidPayment (10.5M, 9.5M))

[<Fact>]
let ``Cannot Pay for Non-Served Order``() =
    Given (PlacedOrder order)
    |> When (CloseTab { Tab = order.Tab; Amount = 10.5M })
    |> ShouldFailWith CannotPayForNonServedOrder

[<Fact>]
let ``Cannot Pay for Order in Progress``() =
    let inProgressOrder = {
        PlacedOrder = order
        PreparedFoods = []
        ServedDrinks = []
        ServedFoods = []
    }

    Given (OrderInProgress inProgressOrder)
    |> When (CloseTab { Tab = order.Tab; Amount = 10.5M })
    |> ShouldFailWith CannotPayForNonServedOrder

[<Fact>]
let ``Cannot Pay for Non-Placed Order``() =
    Given (OpenedTab order.Tab)
    |> When (CloseTab { Tab = order.Tab; Amount = 10.5M })
    |> ShouldFailWith CannotPayForNonServedOrder

[<Fact>]
let ``Cannot Pay with Closed Tab``() =
    Given (ClosedTab None)
    |> When (CloseTab { Tab = order.Tab; Amount = 10.5M })
    |> ShouldFailWith CannotPayForNonServedOrder
