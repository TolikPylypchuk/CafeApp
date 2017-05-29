module CafeApp.Core.Tests.ServeDrinkTests

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
let ``Can Serve Drink``() =
    let order = { order with Drinks = [ coke; lemonade ] }
    let expected = {
        PlacedOrder = order
        ServedDrinks = [ coke ]
        PreparedFoods = []
        ServedFoods = []
    }

    Given (PlacedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [ DrinkServed (coke, order.Tab.Id) ]

[<Fact>]
let ``Can Serve Drink for Order Containing Only One Drink``() =
    let order = { order with Drinks = [ coke ] }
    let payment = { Tab = order.Tab; Amount = drinkPrice coke }

    Given (PlacedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ThenStateShouldBe (ServedOrder order)
    |> WithEvents [
        DrinkServed (coke, order.Tab.Id)
        OrderServed (order, payment)
    ]

[<Fact>]
let ``Remain in Order in Progress while Serving Drink``() =
    let order = { order with Drinks = [ coke; lemonade; appleJuice ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedDrinks = [ coke ]
        PreparedFoods = []
        ServedFoods = []
    }

    let expected = {
        inProgressOrder with
            ServedDrinks = lemonade :: inProgressOrder.ServedDrinks
    }

    Given (OrderInProgress inProgressOrder)
    |> When (ServeDrink (lemonade, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [ DrinkServed (lemonade, order.Tab.Id) ]

[<Fact>]
let ``Cannot Serve Non-Ordered Drink``() =
    let order = { order with Drinks = [ coke ] }

    Given (PlacedOrder order)
    |> When (ServeDrink (lemonade, order.Tab.Id))
    |> ShouldFailWith (CannotServeNonOrderedDrink lemonade)

[<Fact>]
let ``Cannot Serve Drink for an Already Served Order``() =
    Given (ServedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Fact>]
let ``Cannot Serve Drink for Non-Placed Order``() =
    Given (OpenedTab tab)
    |> When (ServeDrink (coke, tab.Id))
    |> ShouldFailWith CannotServeForNonPlacedOrder

[<Fact>]
let ``Cannot Serve Non-Ordered Drink During Order in Progress``() =
    let order = { order with Drinks = [ coke; lemonade ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedDrinks = [ coke ]
        PreparedFoods = []
        ServedFoods = []
    }

    Given (OrderInProgress inProgressOrder)
    |> When (ServeDrink (appleJuice, order.Tab.Id))
    |> ShouldFailWith (CannotServeNonOrderedDrink appleJuice)

[<Fact>]
let ``Cannot Serve an Already Served Drink``() =
    let order = { order with Drinks = [ coke; lemonade ] }
    let inProgressOrder = {
        PlacedOrder = order
        ServedDrinks = [ coke ]
        PreparedFoods = []
        ServedFoods = []
    }

    Given (OrderInProgress inProgressOrder)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ShouldFailWith (CannotServeAlreadyServedDrink coke)

[<Fact>]
let ``Cannot Serve Drink with Closed Tab``() =
    Given (ClosedTab None)
    |> When (ServeDrink (coke, tab.Id))
    |> ShouldFailWith CannotServeWithClosedTab
